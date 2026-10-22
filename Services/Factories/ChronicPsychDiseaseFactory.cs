using PatientService.Models.Entities;
using PatientService.Services.DTOs.ChronicPsychDisease;

namespace PatientService.Services.Factories;

public class ChronicPsychDiseaseFactory : IChronicPsychDiseaseFactory
{
    public ChronicPsychDisease CreateFrom(ChronicPsychDiseaseCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new ChronicPsychDisease
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Name = dto.Name.Trim(),
            DiagnosisDate = dto.DiagnosisDate.Date,
            Hospitalization = dto.Hospitalization.Trim(),
            Medications = dto.Medications.Trim(),
            Severity = dto.Severity.Trim(),
            Response = dto.Response.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(ChronicPsychDisease entity, ChronicPsychDiseaseUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.DiagnosisDate = dto.DiagnosisDate.Date;
        entity.Hospitalization = dto.Hospitalization.Trim();
        entity.Medications = dto.Medications.Trim();
        entity.Severity = dto.Severity.Trim();
        entity.Response = dto.Response.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}