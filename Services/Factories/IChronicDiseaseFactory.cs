using PatientService.Models.Entities;
using PatientService.Services.DTOs.ChronicDisease;

namespace PatientService.Services.Factories;

public interface IChronicDiseaseFactory
{
    ChronicDisease CreateFrom(ChronicDiseaseCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(ChronicDisease entity, ChronicDiseaseUpdateDto dto);
}