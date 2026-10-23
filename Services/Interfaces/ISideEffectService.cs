using PatientService.Services.Common;
using PatientService.Services.DTOs.SideEffect;

namespace PatientService.Services.Interfaces;

public interface ISideEffectService
{
    Task<ServiceResult<IEnumerable<SideEffectResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<SideEffectResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<SideEffectResponseDto>> CreateAsync(SideEffectCreateDto dto, Guid doctorId);
    Task<ServiceResult<SideEffectResponseDto>> UpdateAsync(SideEffectUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}