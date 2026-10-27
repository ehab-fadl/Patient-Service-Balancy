using PatientService.Models.Entities;
using PatientService.Services.DTOs.Visit;

namespace PatientService.Services.Factories;

public class VisitFactory : IVisitFactory
{
    public Visit CreateFrom(VisitCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new Visit
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Reason = dto.Reason.Trim(),
            VisitDate = dto.VisitDate.Date,
            VisitType = dto.VisitType.Trim(),
            VisitLocation = dto.VisitLocation.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            NextVisitDate = dto.NextVisitDate?.Date
        };
    }

    public void UpdateFrom(Visit entity, VisitUpdateDto dto)
    {
        entity.Reason = dto.Reason.Trim();
        entity.VisitDate = dto.VisitDate.Date;
        entity.VisitType = dto.VisitType.Trim();
        entity.VisitLocation = dto.VisitLocation.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
        entity.NextVisitDate = dto.NextVisitDate?.Date;
    }
}