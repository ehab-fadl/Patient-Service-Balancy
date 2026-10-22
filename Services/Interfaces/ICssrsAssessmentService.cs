using PatientService.Services.Common;
using PatientService.Services.DTOs.CssrsAssessment;

namespace PatientService.Services.Interfaces;

public interface ICssrsAssessmentService
{
    Task<ServiceResult<IEnumerable<CssrsAssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<CssrsAssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<CssrsAssessmentResponseDto>> CreateAsync(CssrsAssessmentCreateDto dto, Guid doctorId);
    Task<ServiceResult<CssrsAssessmentResponseDto>> UpdateAsync(CssrsAssessmentUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}