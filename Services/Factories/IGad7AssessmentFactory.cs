using PatientService.Models.Entities;
using PatientService.Services.DTOs.Gad7Assessment;

namespace PatientService.Services.Factories;

public interface IGad7AssessmentFactory
{
    Gad7Assessment CreateFrom(Gad7AssessmentCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(Gad7Assessment entity, Gad7AssessmentUpdateDto dto);
}