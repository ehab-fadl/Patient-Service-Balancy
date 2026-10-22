using PatientService.Services.Common;
using PatientService.Services.DTOs.VitalSign;

namespace PatientService.Services.Interfaces;

public interface IVitalSignService
{
    Task<ServiceResult<IEnumerable<VitalSignResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<VitalSignResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<VitalSignResponseDto>> CreateAsync(VitalSignCreateDto dto, Guid doctorId);
    Task<ServiceResult<VitalSignResponseDto>> UpdateAsync(VitalSignUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}