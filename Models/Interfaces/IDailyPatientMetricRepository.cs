using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IDailyPatientMetricRepository : IGenericRepository<DailyPatientMetric>
{
    Task<DailyPatientMetric?> GetByPatientAndDateAsync(Guid patientId, DateTime date);
    Task<IEnumerable<DailyPatientMetric>> GetByPatientAndDateRangeAsync(Guid patientId, DateTime fromDate, DateTime toDate);
}