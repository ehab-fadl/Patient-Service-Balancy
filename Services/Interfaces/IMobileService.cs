using PatientService.Services.Common;
using PatientService.Services.DTOs.Mobile;

namespace PatientService.Services.Interfaces;

public interface IMobileService
{
    Task<ServiceResult<MobilePatientProfileDto>> GetProfileAsync(Guid patientId);
    Task<ServiceResult<MobileDailyMetricDto>> SubmitDailyMetricAsync(Guid patientId, MobileDailyMetricDto dto);
    Task<ServiceResult<MobileDailyMetricDto?>> GetTodayMetricAsync(Guid patientId);
    Task<ServiceResult<MobileAssessmentHistoryDto>> SubmitAssessmentAsync(Guid patientId, MobileAssessmentSubmitDto dto);
    Task<ServiceResult<IEnumerable<MobileAssessmentHistoryDto>>> GetAssessmentHistoryAsync(Guid patientId);
    Task<ServiceResult<MobileTreatmentPlanDto?>> GetActiveTreatmentPlanAsync(Guid patientId);
    Task<ServiceResult<bool>> UpdateGoalProgressAsync(Guid patientId, Guid goalId, decimal value, string? note);
    Task<ServiceResult<IEnumerable<MobileMedicationDto>>> GetMedicationsAsync(Guid patientId);
    Task<ServiceResult<bool>> SubmitSideEffectAsync(Guid patientId, MobileSideEffectSubmitDto dto);
    Task<ServiceResult<IEnumerable<MobileAppointmentDto>>> GetAppointmentsAsync(Guid patientId);
    Task<ServiceResult<bool>> CancelAppointmentAsync(Guid patientId, MobileAppointmentCancelDto dto);
    Task<ServiceResult<bool>> RequestRescheduleAsync(Guid patientId, Guid appointmentId, DateTime newDate, string? newTime);
    Task<ServiceResult<IEnumerable<MobileAlertDto>>> GetAlertsAsync(Guid patientId);
    Task<ServiceResult<bool>> DismissAlertAsync(Guid patientId, Guid alertId);
}