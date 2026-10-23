using PatientService.Models.Entities;
using PatientService.Services.DTOs.SideEffect;

namespace PatientService.Services.Factories;

public interface ISideEffectFactory
{
    SideEffect CreateFrom(SideEffectCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(SideEffect entity, SideEffectUpdateDto dto);
}