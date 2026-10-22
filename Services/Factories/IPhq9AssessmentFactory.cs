using PatientService.Models.Entities;
using PatientService.Services.DTOs.Phq9Assessment;

namespace PatientService.Services.Factories;

public interface IPhq9AssessmentFactory
{
    Phq9Assessment CreateFrom(Phq9AssessmentCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(Phq9Assessment entity, Phq9AssessmentUpdateDto dto);
}