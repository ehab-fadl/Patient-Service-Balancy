using PatientService.Models.Entities;

namespace PatientService.Services.Factories;

public class PatientCodeFactory : IPatientCodeFactory
{
    public PatientCode Create(Guid patientId, string code)
    {
        return new PatientCode
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Code = code,
            GeneratedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}