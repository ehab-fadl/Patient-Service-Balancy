using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Documents;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DocumentService> _logger;

    // مجلد تخزين الملفات (يجب إضافته إلى appsettings.json)
    private const string UploadFolder = "Uploads/PatientDocuments";

    public DocumentService(IUnitOfWork unitOfWork, ILogger<DocumentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // 1. جلب الوثائق
    public async Task<ServiceResult<IEnumerable<DocumentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<IEnumerable<DocumentResponseDto>>.FailureResult("غير مصرح");

        var docs = await _unitOfWork.PatientDocuments.GetByPatientIdAsync(patientId);
        return ServiceResult<IEnumerable<DocumentResponseDto>>.SuccessResult(docs.Select(MapToResponse).ToList());
    }

    public async Task<ServiceResult<DocumentResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var doc = await _unitOfWork.PatientDocuments.GetByIdAndDoctorAsync(id, doctorId);
        if (doc is null) return ServiceResult<DocumentResponseDto>.NotFoundResult("الوثيقة غير موجودة");
        return ServiceResult<DocumentResponseDto>.SuccessResult(MapToResponse(doc));
    }

    // 2. رفع وثيقة (حفظ الملف على السيرفر)
    public async Task<ServiceResult<DocumentResponseDto>> UploadDocumentAsync(
        Guid patientId, IFormFile file,
        string name, string type, string sourceType, DateTime documentDate,
        string? issuer, string? category, string? refNo, DateTime? expiryDate,
        bool isSigned, bool isCritical, string? notes, string? tags,
        Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<DocumentResponseDto>.FailureResult("غير مصرح");

        // 1. إنشاء مجلد مخصص للمريض إذا لم يكن موجوداً
        var patientFolder = Path.Combine(UploadFolder, patientId.ToString());
        if (!Directory.Exists(patientFolder))
            Directory.CreateDirectory(patientFolder);

        // 2. توليد اسم فريد للملف
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(patientFolder, uniqueFileName);

        // 3. حفظ الملف على القرص
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 4. حفظ البيانات في قاعدة البيانات
        var entity = new PatientDocument
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Name = name,
            Type = type,
            SourceType = sourceType,
            DocumentDate = documentDate,
            Issuer = issuer,
            Category = category,
            RefNo = refNo,
            ExpiryDate = expiryDate,
            IsSigned = isSigned,
            IsCritical = isCritical,
            Notes = notes,
            Tags = tags,
            FileName = file.FileName,
            FilePath = filePath.Replace("\\", "/"), // حفظ المسار بشكل موحد
            FileSize = file.Length
        };

        await _unitOfWork.PatientDocuments.AddAsync(entity);

        _logger.LogInformation("Document uploaded. Id={Id}, PatientId={PatientId}, File={File}", entity.Id, patientId, file.FileName);
        return ServiceResult<DocumentResponseDto>.SuccessResult(MapToResponse(entity), "تم رفع الوثيقة بنجاح");
    }

    // 3. تعديل البيانات الوصفية
    public async Task<ServiceResult<DocumentResponseDto>> UpdateDocumentAsync(DocumentUpdateDto dto, Guid doctorId)
    {
        var doc = await _unitOfWork.PatientDocuments.GetByIdAndDoctorAsync(dto.Id, doctorId);
        if (doc is null) return ServiceResult<DocumentResponseDto>.NotFoundResult("الوثيقة غير موجودة");

        doc.Name = dto.Name;
        doc.Type = dto.Type;
        doc.SourceType = dto.SourceType;
        doc.DocumentDate = dto.DocumentDate;
        doc.Issuer = dto.Issuer;
        doc.Category = dto.Category;
        doc.RefNo = dto.RefNo;
        doc.ExpiryDate = dto.ExpiryDate;
        doc.IsSigned = dto.IsSigned;
        doc.IsCritical = dto.IsCritical;
        doc.Notes = dto.Notes;
        doc.Tags = dto.Tags;

        await _unitOfWork.PatientDocuments.UpdateAsync(doc);
        return ServiceResult<DocumentResponseDto>.SuccessResult(MapToResponse(doc), "تم تحديث بيانات الوثيقة");
    }

    // 4. المفضلة
    public async Task<ServiceResult<bool>> ToggleFavoriteAsync(Guid id, Guid doctorId)
    {
        var doc = await _unitOfWork.PatientDocuments.GetByIdAndDoctorAsync(id, doctorId);
        if (doc is null) return ServiceResult<bool>.NotFoundResult("الوثيقة غير موجودة");

        doc.IsFavorite = !doc.IsFavorite;
        await _unitOfWork.PatientDocuments.UpdateAsync(doc);
        return ServiceResult<bool>.SuccessResult(doc.IsFavorite);
    }

    // 5. حذف وثيقة (حذف ناعم + حذف الملف الفيزيائي اختياري)
    public async Task<ServiceResult<bool>> DeleteDocumentAsync(Guid id, Guid doctorId)
    {
        var doc = await _unitOfWork.PatientDocuments.GetByIdAndDoctorAsync(id, doctorId);
        if (doc is null) return ServiceResult<bool>.NotFoundResult("الوثيقة غير موجودة");

        await _unitOfWork.PatientDocuments.DeleteAsync(doc);
        return ServiceResult<bool>.SuccessResult(true, "تم حذف الوثيقة");
    }

    // 6. تحميل الملف
    public async Task<ServiceResult<byte[]>> DownloadFileAsync(Guid id, Guid doctorId)
    {
        var doc = await _unitOfWork.PatientDocuments.GetByIdAndDoctorAsync(id, doctorId);
        if (doc is null || string.IsNullOrEmpty(doc.FilePath))
            return ServiceResult<byte[]>.NotFoundResult("الملف غير موجود");

        if (!File.Exists(doc.FilePath))
            return ServiceResult<byte[]>.NotFoundResult("الملف الفيزيائي غير موجود على السيرفر");

        var fileBytes = await File.ReadAllBytesAsync(doc.FilePath);
        return ServiceResult<byte[]>.SuccessResult(fileBytes);
    }

    private static DocumentResponseDto MapToResponse(PatientDocument entity)
    {
        return new DocumentResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Name = entity.Name,
            Type = entity.Type,
            SourceType = entity.SourceType,
            DocumentDate = entity.DocumentDate,
            Issuer = entity.Issuer,
            Category = entity.Category,
            RefNo = entity.RefNo,
            ExpiryDate = entity.ExpiryDate,
            IsSigned = entity.IsSigned,
            IsCritical = entity.IsCritical,
            IsFavorite = entity.IsFavorite,
            Notes = entity.Notes,
            Tags = entity.Tags,
            FileName = entity.FileName,
            FilePath = entity.FilePath,
            FileSize = entity.FileSize,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}