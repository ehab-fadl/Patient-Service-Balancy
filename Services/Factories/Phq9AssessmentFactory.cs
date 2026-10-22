using PatientService.Models.Entities;
using PatientService.Services.DTOs.Phq9Assessment;

namespace PatientService.Services.Factories;

public class Phq9AssessmentFactory : IPhq9AssessmentFactory
{
    public Phq9Assessment CreateFrom(Phq9AssessmentCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new Phq9Assessment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            TotalScore = dto.TotalScore,
            Severity = dto.Severity.Trim().ToLowerInvariant(),
            AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim()
        };
    }

    public void UpdateFrom(Phq9Assessment entity, Phq9AssessmentUpdateDto dto)
    {
        entity.TotalScore = dto.TotalScore;
        entity.Severity = dto.Severity.Trim().ToLowerInvariant();
        entity.AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim();
    }
}