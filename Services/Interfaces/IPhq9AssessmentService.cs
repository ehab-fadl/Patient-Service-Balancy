using PatientService.Services.Common;
using PatientService.Services.DTOs.Phq9Assessment;

namespace PatientService.Services.Interfaces;

public interface IPhq9AssessmentService
{
    Task<ServiceResult<IEnumerable<Phq9AssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<Phq9AssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<Phq9AssessmentResponseDto>> CreateAsync(Phq9AssessmentCreateDto dto, Guid doctorId);
    Task<ServiceResult<Phq9AssessmentResponseDto>> UpdateAsync(Phq9AssessmentUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}