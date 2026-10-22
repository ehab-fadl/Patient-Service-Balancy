using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IPatientPersonalInfoRepository : IGenericRepository<PatientPersonalInfo>
{
    Task<IEnumerable<PatientPersonalInfo>> SearchAsync(string searchTerm);
    Task<IEnumerable<PatientPersonalInfo>> GetByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<PatientPersonalInfo>> SearchByDoctorIdAsync(Guid doctorId, string searchTerm);
}