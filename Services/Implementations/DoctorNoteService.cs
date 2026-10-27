using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.DoctorNote;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class DoctorNoteService : IDoctorNoteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDoctorNoteFactory _factory;
    private readonly ILogger<DoctorNoteService> _logger;

    public DoctorNoteService(
        IUnitOfWork unitOfWork,
        IDoctorNoteFactory factory,
        ILogger<DoctorNoteService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<DoctorNoteResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<DoctorNoteResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<DoctorNoteResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var notes = await _unitOfWork.DoctorNotes.GetByPatientIdAsync(patientId);
        var result = notes.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<DoctorNoteResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<DoctorNoteResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var note = await _unitOfWork.DoctorNotes.GetByIdAndDoctorAsync(id, doctorId);
        if (note is null) return ServiceResult<DoctorNoteResponseDto>.NotFoundResult("الملاحظة غير موجودة أو غير مصرح لك بالوصول إليها");
        return ServiceResult<DoctorNoteResponseDto>.SuccessResult(MapToResponse(note));
    }

    public async Task<ServiceResult<DoctorNoteResponseDto>> CreateAsync(DoctorNoteCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<DoctorNoteResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<DoctorNoteResponseDto>.FailureResult("غير مصرح لك بإضافة ملاحظة لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.DoctorNotes.AddAsync(entity);

            if (entity.RiskLevel == "حرج")
            {
                _logger.LogWarning("CRITICAL DOCTOR NOTE ADDED for patient {PatientId}. Title: {Title}", entity.PatientId, entity.Title);
            }
            else
            {
                _logger.LogInformation("Doctor note created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            }

            return ServiceResult<DoctorNoteResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة الملاحظة الطبية بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor note for patient {PatientId}", dto.PatientId);
            return ServiceResult<DoctorNoteResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<DoctorNoteResponseDto>> UpdateAsync(DoctorNoteUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.DoctorNotes.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<DoctorNoteResponseDto>.NotFoundResult("الملاحظة غير موجودة أو غير مصرح لك بتعديلها");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.DoctorNotes.UpdateAsync(existing);

            _logger.LogInformation("Doctor note updated. Id={Id}", dto.Id);
            return ServiceResult<DoctorNoteResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث الملاحظة الطبية بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor note {Id}", dto.Id);
            return ServiceResult<DoctorNoteResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.DoctorNotes.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("الملاحظة غير موجودة أو غير مصرح لك بحذفها");

            await _unitOfWork.DoctorNotes.DeleteAsync(existing);

            _logger.LogInformation("Doctor note deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف الملاحظة الطبية بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor note {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static DoctorNoteResponseDto MapToResponse(DoctorNote entity)
    {
        return new DoctorNoteResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Title = entity.Title,
            NoteDate = entity.NoteDate,
            NoteType = entity.NoteType,
            Category = entity.Category,
            Author = entity.Author,
            AuthorRole = entity.AuthorRole,
            Priority = entity.Priority,
            RiskLevel = entity.RiskLevel,
            Icd10 = entity.Icd10,
            MoodAtNote = entity.MoodAtNote,
            Visibility = entity.Visibility,
            Symptoms = entity.Symptoms,
            LinkType = entity.LinkType,
            LinkItemId = entity.LinkItemId,
            Content = entity.Content,
            NextPlan = entity.NextPlan,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}