using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IPatientReportRepository : IGenericRepository<PatientReport>
{
    Task<IEnumerable<PatientReport>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<PatientReport?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}