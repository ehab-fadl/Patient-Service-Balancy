using PatientService.Models.Entities;
using PatientService.Services.DTOs.Medication;

namespace PatientService.Services.Factories;

public interface IMedicationFactory
{
    Medication CreateFrom(MedicationCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(Medication entity, MedicationUpdateDto dto);
}