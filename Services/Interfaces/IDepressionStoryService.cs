using PatientService.Services.Common;
using PatientService.Services.DTOs.DepressionStory;

namespace PatientService.Services.Interfaces;

public interface IDepressionStoryService
{
    Task<ServiceResult<DepressionStoryResponseDto>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<DepressionStoryResponseDto>> UpdateAsync(DepressionStoryUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid patientId, Guid doctorId);
}