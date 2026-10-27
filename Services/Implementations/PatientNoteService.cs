using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.PatientNote;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class PatientNoteService : IPatientNoteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPatientNoteFactory _factory;
    private readonly ILogger<PatientNoteService> _logger;

    public PatientNoteService(
        IUnitOfWork unitOfWork,
        IPatientNoteFactory factory,
        ILogger<PatientNoteService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<PatientNoteResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<PatientNoteResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<PatientNoteResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var notes = await _unitOfWork.PatientNotes.GetByPatientIdAsync(patientId);
        var result = notes.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<PatientNoteResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<PatientNoteResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var note = await _unitOfWork.PatientNotes.GetByIdAndDoctorAsync(id, doctorId);
        if (note is null) return ServiceResult<PatientNoteResponseDto>.NotFoundResult("الملاحظة غير موجودة أو غير مصرح لك بالوصول إليها");
        return ServiceResult<PatientNoteResponseDto>.SuccessResult(MapToResponse(note));
    }

    public async Task<ServiceResult<PatientNoteResponseDto>> CreateAsync(PatientNoteCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<PatientNoteResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<PatientNoteResponseDto>.FailureResult("غير مصرح لك بإضافة ملاحظة لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.PatientNotes.AddAsync(entity);

            // تسجيل تنبيه في السجل إذا كان الخطر حرجاً
            if (entity.RiskLevel == "حرج")
            {
                _logger.LogWarning("CRITICAL NOTE ADDED for patient {PatientId}. Title: {Title}", entity.PatientId, entity.Title);
            }
            else
            {
                _logger.LogInformation("Patient note created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            }

            return ServiceResult<PatientNoteResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة الملاحظة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient note for patient {PatientId}", dto.PatientId);
            return ServiceResult<PatientNoteResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<PatientNoteResponseDto>> UpdateAsync(PatientNoteUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.PatientNotes.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<PatientNoteResponseDto>.NotFoundResult("الملاحظة غير موجودة أو غير مصرح لك بتعديلها");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.PatientNotes.UpdateAsync(existing);

            _logger.LogInformation("Patient note updated. Id={Id}", dto.Id);
            return ServiceResult<PatientNoteResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث الملاحظة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient note {Id}", dto.Id);
            return ServiceResult<PatientNoteResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.PatientNotes.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("الملاحظة غير موجودة أو غير مصرح لك بحذفها");

            await _unitOfWork.PatientNotes.DeleteAsync(existing);

            _logger.LogInformation("Patient note deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف الملاحظة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting patient note {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static PatientNoteResponseDto MapToResponse(PatientNote entity)
    {
        return new PatientNoteResponseDto
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