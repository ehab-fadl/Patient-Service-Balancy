using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface ISystemAlertRepository : IGenericRepository<SystemAlert>
{
    Task<IEnumerable<SystemAlert>> GetActiveByDoctorIdAsync(Guid doctorId);
    Task<SystemAlert?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
    Task<bool> AlertExistsForTodayAsync(Guid patientId, string ruleId);
}