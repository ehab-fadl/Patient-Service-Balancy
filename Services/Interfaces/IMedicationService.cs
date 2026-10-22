using PatientService.Services.Common;
using PatientService.Services.DTOs.Medication;

namespace PatientService.Services.Interfaces;

public interface IMedicationService
{
    Task<ServiceResult<IEnumerable<MedicationResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<MedicationResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<MedicationResponseDto>> CreateAsync(MedicationCreateDto dto, Guid doctorId);
    Task<ServiceResult<MedicationResponseDto>> UpdateAsync(MedicationUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}