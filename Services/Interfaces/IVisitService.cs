using PatientService.Services.Common;
using PatientService.Services.DTOs.Visit;

namespace PatientService.Services.Interfaces;

public interface IVisitService
{
    Task<ServiceResult<IEnumerable<VisitResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<VisitResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<VisitResponseDto>> CreateAsync(VisitCreateDto dto, Guid doctorId);
    Task<ServiceResult<VisitResponseDto>> UpdateAsync(VisitUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}