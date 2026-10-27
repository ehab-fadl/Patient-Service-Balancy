using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.ListeningSession;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class ListeningSessionService : IListeningSessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IListeningSessionFactory _factory;
    private readonly ILogger<ListeningSessionService> _logger;

    public ListeningSessionService(
        IUnitOfWork unitOfWork,
        IListeningSessionFactory factory,
        ILogger<ListeningSessionService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ListeningSessionResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<ListeningSessionResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<ListeningSessionResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var sessions = await _unitOfWork.ListeningSessions.GetByPatientIdAsync(patientId);
        var result = sessions.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<ListeningSessionResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<ListeningSessionResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var session = await _unitOfWork.ListeningSessions.GetByIdAndDoctorAsync(id, doctorId);
        if (session is null) return ServiceResult<ListeningSessionResponseDto>.NotFoundResult("الجلسة غير موجودة أو غير مصرح لك بالوصول إليها");
        return ServiceResult<ListeningSessionResponseDto>.SuccessResult(MapToResponse(session));
    }

    public async Task<ServiceResult<ListeningSessionResponseDto>> CreateAsync(ListeningSessionCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<ListeningSessionResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<ListeningSessionResponseDto>.FailureResult("غير مصرح لك بإضافة جلسة لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.ListeningSessions.AddAsync(entity);

            _logger.LogInformation("Listening session created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            return ServiceResult<ListeningSessionResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة الجلسة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating listening session for patient {PatientId}", dto.PatientId);
            return ServiceResult<ListeningSessionResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<ListeningSessionResponseDto>> UpdateAsync(ListeningSessionUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.ListeningSessions.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<ListeningSessionResponseDto>.NotFoundResult("الجلسة غير موجودة أو غير مصرح لك بتعديلها");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.ListeningSessions.UpdateAsync(existing);

            _logger.LogInformation("Listening session updated. Id={Id}", dto.Id);
            return ServiceResult<ListeningSessionResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث الجلسة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating listening session {Id}", dto.Id);
            return ServiceResult<ListeningSessionResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.ListeningSessions.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("الجلسة غير موجودة أو غير مصرح لك بحذفها");

            await _unitOfWork.ListeningSessions.DeleteAsync(existing);

            _logger.LogInformation("Listening session deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف الجلسة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting listening session {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static ListeningSessionResponseDto MapToResponse(ListeningSession entity)
    {
        return new ListeningSessionResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Title = entity.Title,
            SessionDate = entity.SessionDate,
            SessionTime = entity.SessionTime,
            DurationMinutes = entity.DurationMinutes,
            SessionType = entity.SessionType,
            Status = entity.Status,
            TherapyType = entity.TherapyType,
            Format = entity.Format,
            TherapistName = entity.TherapistName,
            LinkedTreatmentPlanId = entity.LinkedTreatmentPlanId,
            MoodBefore = entity.MoodBefore,
            MoodAfter = entity.MoodAfter,
            TopicsDiscussed = entity.TopicsDiscussed,
            TechniquesUsed = entity.TechniquesUsed,
            RiskLevel = entity.RiskLevel,
            HomeworkCompleted = entity.HomeworkCompleted,
            HomeworkAssigned = entity.HomeworkAssigned,
            NextSessionPlan = entity.NextSessionPlan,
            Notes = entity.Notes,
            ClinicalNotes = entity.ClinicalNotes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}