using PatientService.Models.Entities;
using PatientService.Services.DTOs.Gad7Assessment;

namespace PatientService.Services.Factories;

public class Gad7AssessmentFactory : IGad7AssessmentFactory
{
    public Gad7Assessment CreateFrom(Gad7AssessmentCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new Gad7Assessment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            TotalScore = dto.TotalScore,
            Severity = dto.Severity.Trim().ToLowerInvariant(),
            AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim()
        };
    }

    public void UpdateFrom(Gad7Assessment entity, Gad7AssessmentUpdateDto dto)
    {
        entity.TotalScore = dto.TotalScore;
        entity.Severity = dto.Severity.Trim().ToLowerInvariant();
        entity.AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim();
    }
}