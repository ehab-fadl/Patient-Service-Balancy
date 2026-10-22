using PatientService.Models.Entities;
using PatientService.Services.DTOs.Diagnosis;

namespace PatientService.Services.Factories;

public interface IDiagnosisFactory
{
    Diagnosis CreateFrom(DiagnosisCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(Diagnosis entity, DiagnosisUpdateDto dto);
}