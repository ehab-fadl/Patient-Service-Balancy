using PatientService.Models.Entities;
using PatientService.Services.DTOs.VitalSign;

namespace PatientService.Services.Factories;

public interface IVitalSignFactory
{
    VitalSign CreateFrom(VitalSignCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(VitalSign entity, VitalSignUpdateDto dto);
}