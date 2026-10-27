using PatientService.Models.Entities;
using PatientService.Services.DTOs.SubstanceUse;

namespace PatientService.Services.Factories;

public interface ISubstanceUseFactory
{
    SubstanceUse CreateFrom(SubstanceUseCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(SubstanceUse entity, SubstanceUseUpdateDto dto);
}