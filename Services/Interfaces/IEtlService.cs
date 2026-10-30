using PatientService.Services.DTOs.ETL;

namespace PatientService.Services.Interfaces;

public interface IEtlService
{
    // Health
    Task<EtlResponse<EtlHealthDto>> GetHealthAsync();

    // Core entities
    Task<EtlResponse<List<EtlPatientDto>>> GetPatientsAsync(DateTime? updatedSince, int page, int pageSize);
    Task<EtlResponse<List<EtlAssessmentDto>>> GetAssessmentsAsync(DateTime? updatedSince, int page, int pageSize);
    Task<EtlResponse<List<EtlMedicationDto>>> GetMedicationsAsync(DateTime? updatedSince, int page, int pageSize);
    Task<EtlResponse<List<EtlDiagnosisDto>>> GetDiagnosesAsync(DateTime? updatedSince, int page, int pageSize);

    // Clinical
    Task<EtlResponse<List<EtlChronicConditionDto>>> GetChronicConditionsAsync(DateTime? updatedSince, int page, int pageSize);
    Task<EtlResponse<List<EtlFamilyHistoryDto>>> GetFamilyHistoryAsync(DateTime? updatedSince, int page, int pageSize);

    // Lifestyle
    Task<EtlResponse<List<EtlSubstanceUseDto>>> GetSubstanceUseAsync(DateTime? updatedSince, int page, int pageSize);
    Task<EtlResponse<List<EtlListeningSessionDto>>> GetListeningSessionsAsync(DateTime? updatedSince, int page, int pageSize);

    // Treatment
    Task<EtlResponse<List<EtlVisitDto>>> GetVisitsAsync(DateTime? updatedSince, int page, int pageSize);
    Task<EtlResponse<List<EtlTreatmentPlanGoalDto>>> GetTreatmentPlanGoalsAsync(DateTime? updatedSince, int page, int pageSize);
    Task<EtlResponse<List<EtlVitalSignDto>>> GetVitalSignsAsync(DateTime? updatedSince, int page, int pageSize);
}