using PatientService.Services.Common;
using PatientService.Services.DTOs.SubstanceUse;

namespace PatientService.Services.Interfaces;

public interface ISubstanceUseService
{
    Task<ServiceResult<IEnumerable<SubstanceUseResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<SubstanceUseResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<SubstanceUseResponseDto>> CreateAsync(SubstanceUseCreateDto dto, Guid doctorId);
    Task<ServiceResult<SubstanceUseResponseDto>> UpdateAsync(SubstanceUseUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}