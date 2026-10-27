using PatientService.Services.Common;
using PatientService.Services.DTOs.DoctorNote;

namespace PatientService.Services.Interfaces;

public interface IDoctorNoteService
{
    Task<ServiceResult<IEnumerable<DoctorNoteResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<DoctorNoteResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<DoctorNoteResponseDto>> CreateAsync(DoctorNoteCreateDto dto, Guid doctorId);
    Task<ServiceResult<DoctorNoteResponseDto>> UpdateAsync(DoctorNoteUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}