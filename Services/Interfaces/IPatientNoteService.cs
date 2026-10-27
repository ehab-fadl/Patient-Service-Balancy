using PatientService.Services.Common;
using PatientService.Services.DTOs.PatientNote;

namespace PatientService.Services.Interfaces;

public interface IPatientNoteService
{
    Task<ServiceResult<IEnumerable<PatientNoteResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<PatientNoteResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<PatientNoteResponseDto>> CreateAsync(PatientNoteCreateDto dto, Guid doctorId);
    Task<ServiceResult<PatientNoteResponseDto>> UpdateAsync(PatientNoteUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}