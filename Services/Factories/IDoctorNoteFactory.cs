using PatientService.Models.Entities;
using PatientService.Services.DTOs.DoctorNote;

namespace PatientService.Services.Factories;

public interface IDoctorNoteFactory
{
    DoctorNote CreateFrom(DoctorNoteCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(DoctorNote entity, DoctorNoteUpdateDto dto);
}