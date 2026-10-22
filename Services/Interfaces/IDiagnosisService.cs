using PatientService.Services.Common;
using PatientService.Services.DTOs.Diagnosis;

namespace PatientService.Services.Interfaces;

public interface IDiagnosisService
{
    Task<ServiceResult<IEnumerable<DiagnosisResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<DiagnosisResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<DiagnosisResponseDto>> CreateAsync(DiagnosisCreateDto dto, Guid doctorId);
    Task<ServiceResult<DiagnosisResponseDto>> UpdateAsync(DiagnosisUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}