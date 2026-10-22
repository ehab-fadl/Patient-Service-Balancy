using PatientService.Models.Entities;
using PatientService.Services.DTOs.MdqAssessment;

namespace PatientService.Services.Factories;

public interface IMdqAssessmentFactory
{
    MdqAssessment CreateFrom(MdqAssessmentCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(MdqAssessment entity, MdqAssessmentUpdateDto dto);
}