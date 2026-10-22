using PatientService.Models.Entities;
using PatientService.Services.DTOs.SdsAssessment;

namespace PatientService.Services.Factories;

public interface ISdsAssessmentFactory
{
    SdsAssessment CreateFrom(SdsAssessmentCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(SdsAssessment entity, SdsAssessmentUpdateDto dto);
}