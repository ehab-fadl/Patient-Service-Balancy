using PatientService.Models.Entities;
using PatientService.Services.DTOs.ChronicDisease;

namespace PatientService.Services.Factories;

public class ChronicDiseaseFactory : IChronicDiseaseFactory
{
    public ChronicDisease CreateFrom(ChronicDiseaseCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new ChronicDisease
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Name = dto.Name.Trim(),
            DiagnosisDate = dto.DiagnosisDate.Date,
            Medications = dto.Medications.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(ChronicDisease entity, ChronicDiseaseUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.DiagnosisDate = dto.DiagnosisDate.Date;
        entity.Medications = dto.Medications.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}