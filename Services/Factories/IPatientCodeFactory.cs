using PatientService.Models.Entities;

namespace PatientService.Services.Factories;

public interface IPatientCodeFactory
{
    PatientCode Create(Guid patientId, string code);
}