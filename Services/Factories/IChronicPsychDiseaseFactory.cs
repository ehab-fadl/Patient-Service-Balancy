using PatientService.Models.Entities;
using PatientService.Services.DTOs.ChronicPsychDisease;

namespace PatientService.Services.Factories;

public interface IChronicPsychDiseaseFactory
{
    ChronicPsychDisease CreateFrom(ChronicPsychDiseaseCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(ChronicPsychDisease entity, ChronicPsychDiseaseUpdateDto dto);
}