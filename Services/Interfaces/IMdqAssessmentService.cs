using PatientService.Services.Common;
using PatientService.Services.DTOs.MdqAssessment;

namespace PatientService.Services.Interfaces;

public interface IMdqAssessmentService
{
    Task<ServiceResult<IEnumerable<MdqAssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<MdqAssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<MdqAssessmentResponseDto>> CreateAsync(MdqAssessmentCreateDto dto, Guid doctorId);
    Task<ServiceResult<MdqAssessmentResponseDto>> UpdateAsync(MdqAssessmentUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}