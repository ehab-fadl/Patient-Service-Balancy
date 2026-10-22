using PatientService.Models.Entities;
using PatientService.Services.DTOs.SdsAssessment;

namespace PatientService.Services.Factories;

public class SdsAssessmentFactory : ISdsAssessmentFactory
{
    public SdsAssessment CreateFrom(SdsAssessmentCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new SdsAssessment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            TotalScore = dto.TotalScore,
            Severity = dto.Severity.Trim().ToLowerInvariant(),
            AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim()
        };
    }

    public void UpdateFrom(SdsAssessment entity, SdsAssessmentUpdateDto dto)
    {
        entity.TotalScore = dto.TotalScore;
        entity.Severity = dto.Severity.Trim().ToLowerInvariant();
        entity.AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim();
    }
}