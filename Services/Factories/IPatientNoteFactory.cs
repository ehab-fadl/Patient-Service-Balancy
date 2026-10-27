using PatientService.Models.Entities;
using PatientService.Services.DTOs.PatientNote;

namespace PatientService.Services.Factories;

public interface IPatientNoteFactory
{
    PatientNote CreateFrom(PatientNoteCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(PatientNote entity, PatientNoteUpdateDto dto);
}