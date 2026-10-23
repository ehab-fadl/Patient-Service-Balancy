using PatientService.Models.Entities;
using PatientService.Services.DTOs.SideEffect;

namespace PatientService.Services.Factories;

public class SideEffectFactory : ISideEffectFactory
{
    public SideEffect CreateFrom(SideEffectCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new SideEffect
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            MedicationName = dto.MedicationName.Trim(),
            Symptom = dto.Symptom.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(SideEffect entity, SideEffectUpdateDto dto)
    {
        entity.MedicationName = dto.MedicationName.Trim();
        entity.Symptom = dto.Symptom.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}