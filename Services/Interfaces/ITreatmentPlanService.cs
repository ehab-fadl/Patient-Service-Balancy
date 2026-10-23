using PatientService.Services.Common;
using PatientService.Services.DTOs.TreatmentPlan;

namespace PatientService.Services.Interfaces;

public interface ITreatmentPlanService
{
    Task<ServiceResult<IEnumerable<TreatmentPlanResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<TreatmentPlanResponseDto>> CreatePlanAsync(TreatmentPlanCreateDto dto, Guid doctorId);
    Task<ServiceResult<TreatmentPlanResponseDto>> UpdatePlanAsync(TreatmentPlanUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeletePlanAsync(Guid id, Guid doctorId);

    Task<ServiceResult<TreatmentProblemResponseDto>> AddProblemAsync(TreatmentProblemCreateDto dto, Guid doctorId);
    Task<ServiceResult<TreatmentProblemResponseDto>> UpdateProblemAsync(TreatmentProblemUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteProblemAsync(Guid id, Guid doctorId);

    Task<ServiceResult<TreatmentGoalResponseDto>> AddGoalAsync(TreatmentGoalCreateDto dto, Guid doctorId);
    Task<ServiceResult<TreatmentGoalResponseDto>> UpdateGoalConfigAsync(TreatmentGoalUpdateDto dto, Guid doctorId);
    Task<ServiceResult<TreatmentGoalResponseDto>> UpdateGoalProgressAsync(TreatmentGoalProgressDto dto, Guid doctorId);
    Task<ServiceResult<TreatmentGoalResponseDto>> AchieveGoalAsync(TreatmentGoalAchieveDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteGoalAsync(Guid id, Guid doctorId);

    Task<ServiceResult<TreatmentMeanResponseDto>> AddMeanAsync(TreatmentMeanCreateDto dto, Guid doctorId);
    Task<ServiceResult<TreatmentMeanResponseDto>> UpdateMeanAsync(TreatmentMeanUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteMeanAsync(Guid id, Guid doctorId);
    Task<ServiceResult<TreatmentGoalResponseDto>> UnachieveGoalAsync(TreatmentGoalUnachieveDto dto, Guid doctorId);
}