using PatientService.Models.Entities;
using PatientService.Services.DTOs.Medication;

namespace PatientService.Services.Factories;

public class MedicationFactory : IMedicationFactory
{
    public Medication CreateFrom(MedicationCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new Medication
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Name = dto.Name.Trim(),
            Dose = dto.Dose.Trim(),
            Frequency = dto.Frequency.Trim(),
            IntakePeriod = dto.IntakePeriod.Trim(),
            Route = dto.Route.Trim(),
            StartDate = dto.StartDate.Date,
            EndDate = dto.EndDate?.Date,
            Status = dto.Status.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(Medication entity, MedicationUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Dose = dto.Dose.Trim();
        entity.Frequency = dto.Frequency.Trim();
        entity.IntakePeriod = dto.IntakePeriod.Trim();
        entity.Route = dto.Route.Trim();
        entity.StartDate = dto.StartDate.Date;
        entity.EndDate = dto.EndDate?.Date;
        entity.Status = dto.Status.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}