using PatientService.Services.Common;
using PatientService.Services.DTOs.ChronicPsychDisease;

namespace PatientService.Services.Interfaces;

public interface IChronicPsychDiseaseService
{
    Task<ServiceResult<IEnumerable<ChronicPsychDiseaseResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<ChronicPsychDiseaseResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<ChronicPsychDiseaseResponseDto>> CreateAsync(ChronicPsychDiseaseCreateDto dto, Guid doctorId);
    Task<ServiceResult<ChronicPsychDiseaseResponseDto>> UpdateAsync(ChronicPsychDiseaseUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}