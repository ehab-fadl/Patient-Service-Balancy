using PatientService.Services.Common;
using PatientService.Services.DTOs.PatientPersonalInfo;

namespace PatientService.Services.Interfaces;

public interface IPatientPersonalInfoService
{
    Task<ServiceResult<IEnumerable<PatientPersonalInfoResponseDto>>> GetAllAsync(Guid doctorId);
    Task<ServiceResult<PatientPersonalInfoResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<IEnumerable<PatientPersonalInfoResponseDto>>> SearchAsync(Guid doctorId, string searchTerm);
    Task<ServiceResult<PatientPersonalInfoResponseDto>> CreateAsync(PatientPersonalInfoCreateDto dto, Guid doctorId);
    Task<ServiceResult<PatientPersonalInfoResponseDto>> UpdateAsync(PatientPersonalInfoUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}