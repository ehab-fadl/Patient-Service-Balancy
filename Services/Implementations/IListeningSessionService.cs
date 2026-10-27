using PatientService.Services.Common;
using PatientService.Services.DTOs.ListeningSession;

namespace PatientService.Services.Interfaces;

public interface IListeningSessionService
{
    Task<ServiceResult<IEnumerable<ListeningSessionResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<ListeningSessionResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<ListeningSessionResponseDto>> CreateAsync(ListeningSessionCreateDto dto, Guid doctorId);
    Task<ServiceResult<ListeningSessionResponseDto>> UpdateAsync(ListeningSessionUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}