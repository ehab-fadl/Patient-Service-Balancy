using PatientService.Models.Entities;
using PatientService.Services.DTOs.PatientPersonalInfo;

namespace PatientService.Services.Factories;

public interface IPatientPersonalInfoFactory
{
    PatientPersonalInfo CreateFrom(PatientPersonalInfoCreateDto dto, Guid doctorId);
    void UpdateFrom(PatientPersonalInfo entity, PatientPersonalInfoUpdateDto dto);
}