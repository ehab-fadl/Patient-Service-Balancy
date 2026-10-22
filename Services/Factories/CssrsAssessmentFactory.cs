using PatientService.Models.Entities;
using PatientService.Services.DTOs.CssrsAssessment;

namespace PatientService.Services.Factories;

public class CssrsAssessmentFactory : ICssrsAssessmentFactory
{
    public CssrsAssessment CreateFrom(CssrsAssessmentCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new CssrsAssessment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            TotalScore = dto.TotalScore,
            Severity = dto.Severity.Trim().ToLowerInvariant(),
            IsCritical = dto.IsCritical,
            AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim()
        };
    }

    public void UpdateFrom(CssrsAssessment entity, CssrsAssessmentUpdateDto dto)
    {
        entity.TotalScore = dto.TotalScore;
        entity.Severity = dto.Severity.Trim().ToLowerInvariant();
        entity.IsCritical = dto.IsCritical;
        entity.AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim();
    }
}