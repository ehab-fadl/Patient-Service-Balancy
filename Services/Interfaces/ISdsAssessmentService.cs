using PatientService.Services.Common;
using PatientService.Services.DTOs.SdsAssessment;

namespace PatientService.Services.Interfaces;

public interface ISdsAssessmentService
{
    Task<ServiceResult<IEnumerable<SdsAssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<SdsAssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<SdsAssessmentResponseDto>> CreateAsync(SdsAssessmentCreateDto dto, Guid doctorId);
    Task<ServiceResult<SdsAssessmentResponseDto>> UpdateAsync(SdsAssessmentUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}