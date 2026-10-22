using PatientService.Services.Common;
using PatientService.Services.DTOs.Gad7Assessment;

namespace PatientService.Services.Interfaces;

public interface IGad7AssessmentService
{
    Task<ServiceResult<IEnumerable<Gad7AssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<Gad7AssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<Gad7AssessmentResponseDto>> CreateAsync(Gad7AssessmentCreateDto dto, Guid doctorId);
    Task<ServiceResult<Gad7AssessmentResponseDto>> UpdateAsync(Gad7AssessmentUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}