using PatientService.Models.Entities;
using PatientService.Services.DTOs.FamilyHistory;

namespace PatientService.Services.Factories;

public class FamilyHistoryFactory : IFamilyHistoryFactory
{
    public FamilyHistory CreateFrom(FamilyHistoryCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new FamilyHistory
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Relation = dto.Relation.Trim(),
            RelativeName = string.IsNullOrWhiteSpace(dto.RelativeName) ? null : dto.RelativeName.Trim(),
            Age = dto.Age,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? null : dto.Status.Trim(),
            Severity = dto.Severity.Trim(),
            ConditionType = dto.ConditionType.Trim(),
            Conditions = dto.Conditions.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(FamilyHistory entity, FamilyHistoryUpdateDto dto)
    {
        entity.Relation = dto.Relation.Trim();
        entity.RelativeName = string.IsNullOrWhiteSpace(dto.RelativeName) ? null : dto.RelativeName.Trim();
        entity.Age = dto.Age;
        entity.Status = string.IsNullOrWhiteSpace(dto.Status) ? null : dto.Status.Trim();
        entity.Severity = dto.Severity.Trim();
        entity.ConditionType = dto.ConditionType.Trim();
        entity.Conditions = dto.Conditions.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}