using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class DailyPatientMetricRepository : GenericRepository<DailyPatientMetric>, IDailyPatientMetricRepository
{
    public DailyPatientMetricRepository(PatientDbContext context) : base(context) { }

    public async Task<DailyPatientMetric?> GetByPatientAndDateAsync(Guid patientId, DateTime date)
    {
        var targetDate = date.Date;
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(m => m.PatientId == patientId && m.LogDate == targetDate);
    }

    public async Task<IEnumerable<DailyPatientMetric>> GetByPatientAndDateRangeAsync(Guid patientId, DateTime fromDate, DateTime toDate)
    {
        var start = fromDate.Date;
        var end = toDate.Date;

        return await _dbSet.AsNoTracking()
            .Where(m => m.PatientId == patientId && m.LogDate >= start && m.LogDate <= end)
            .OrderBy(m => m.LogDate)
            .ToListAsync();
    }
}