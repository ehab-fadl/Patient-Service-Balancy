using PatientService.Models.Entities;
using PatientService.Services.DTOs.MdqAssessment;

namespace PatientService.Services.Factories;

public class MdqAssessmentFactory : IMdqAssessmentFactory
{
    public MdqAssessment CreateFrom(MdqAssessmentCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new MdqAssessment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            TotalScore = dto.TotalScore,
            Severity = dto.Severity.Trim().ToLowerInvariant(),
            AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim()
        };
    }

    public void UpdateFrom(MdqAssessment entity, MdqAssessmentUpdateDto dto)
    {
        entity.TotalScore = dto.TotalScore;
        entity.Severity = dto.Severity.Trim().ToLowerInvariant();
        entity.AnswersJson = string.IsNullOrWhiteSpace(dto.AnswersJson) ? null : dto.AnswersJson.Trim();
    }
}