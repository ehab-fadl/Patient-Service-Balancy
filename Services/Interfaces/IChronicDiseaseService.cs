using PatientService.Services.Common;
using PatientService.Services.DTOs.ChronicDisease;

namespace PatientService.Services.Interfaces;

public interface IChronicDiseaseService
{
    Task<ServiceResult<IEnumerable<ChronicDiseaseResponseDto>>> GetByPatientIdAsync(
        Guid patientId, Guid doctorId);

    Task<ServiceResult<ChronicDiseaseResponseDto>> GetByIdAsync(
        Guid id, Guid doctorId);

    Task<ServiceResult<ChronicDiseaseResponseDto>> CreateAsync(
        ChronicDiseaseCreateDto dto, Guid doctorId);

    Task<ServiceResult<ChronicDiseaseResponseDto>> UpdateAsync(
        ChronicDiseaseUpdateDto dto, Guid doctorId);

    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}