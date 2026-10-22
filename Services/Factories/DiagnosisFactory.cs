using PatientService.Models.Entities;
using PatientService.Services.DTOs.Diagnosis;

namespace PatientService.Services.Factories;

public class DiagnosisFactory : IDiagnosisFactory
{
    public Diagnosis CreateFrom(DiagnosisCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new Diagnosis
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            MainType = dto.MainType.Trim(),
            SubType = dto.SubType.Trim(),
            DiagnosisDate = dto.DiagnosisDate.Date,
            Severity = dto.Severity.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(Diagnosis entity, DiagnosisUpdateDto dto)
    {
        entity.MainType = dto.MainType.Trim();
        entity.SubType = dto.SubType.Trim();
        entity.DiagnosisDate = dto.DiagnosisDate.Date;
        entity.Severity = dto.Severity.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}