using PatientService.Models.Entities;
using PatientService.Services.DTOs.SubstanceUse;

namespace PatientService.Services.Factories;

public class SubstanceUseFactory : ISubstanceUseFactory
{
    public SubstanceUse CreateFrom(SubstanceUseCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new SubstanceUse
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Name = dto.Name.Trim(),
            Type = dto.Type.Trim(),
            Route = dto.Route.Trim(),
            Frequency = dto.Frequency.Trim(),
            Amount = dto.Amount.Trim(),
            DurationYears = dto.DurationYears,
            AgeOfOnset = dto.AgeOfOnset,
            Reason = dto.Reason?.Trim(),
            LastUseDate = dto.LastUseDate.Date,
            CurrentStatus = dto.CurrentStatus.Trim(),
            CravingLevel = dto.CravingLevel,
            Severity = dto.Severity.Trim(),
            PhysicalDependence = dto.PhysicalDependence,
            PsychologicalDependence = dto.PsychologicalDependence,
            ImpactWork = dto.ImpactWork,
            ImpactRelationships = dto.ImpactRelationships,
            ImpactHealth = dto.ImpactHealth,
            ImpactLegal = dto.ImpactLegal,
            QuitAttempts = dto.QuitAttempts,
            LastQuitDate = dto.LastQuitDate?.Date,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(SubstanceUse entity, SubstanceUseUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Type = dto.Type.Trim();
        entity.Route = dto.Route.Trim();
        entity.Frequency = dto.Frequency.Trim();
        entity.Amount = dto.Amount.Trim();
        entity.DurationYears = dto.DurationYears;
        entity.AgeOfOnset = dto.AgeOfOnset;
        entity.Reason = dto.Reason?.Trim();
        entity.LastUseDate = dto.LastUseDate.Date;
        entity.CurrentStatus = dto.CurrentStatus.Trim();
        entity.CravingLevel = dto.CravingLevel;
        entity.Severity = dto.Severity.Trim();
        entity.PhysicalDependence = dto.PhysicalDependence;
        entity.PsychologicalDependence = dto.PsychologicalDependence;
        entity.ImpactWork = dto.ImpactWork;
        entity.ImpactRelationships = dto.ImpactRelationships;
        entity.ImpactHealth = dto.ImpactHealth;
        entity.ImpactLegal = dto.ImpactLegal;
        entity.QuitAttempts = dto.QuitAttempts;
        entity.LastQuitDate = dto.LastQuitDate?.Date;
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}