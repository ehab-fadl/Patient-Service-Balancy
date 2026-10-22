using PatientService.Models.Entities;
using PatientService.Services.DTOs.CssrsAssessment;

namespace PatientService.Services.Factories;

public interface ICssrsAssessmentFactory
{
    CssrsAssessment CreateFrom(CssrsAssessmentCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(CssrsAssessment entity, CssrsAssessmentUpdateDto dto);
}