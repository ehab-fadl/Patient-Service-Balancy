using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IPatientCodeRepository : IGenericRepository<PatientCode>
{
    Task<PatientCode?> GetByPatientIdAsync(Guid patientId);
    Task<PatientCode?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code);
}