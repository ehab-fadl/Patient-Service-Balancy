using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class SystemAlertRepository : GenericRepository<SystemAlert>, ISystemAlertRepository
{
    public SystemAlertRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<SystemAlert>> GetActiveByDoctorIdAsync(Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .Where(a => a.DoctorId == doctorId && !a.IsDismissed)
            .OrderBy(a => a.Level == "critical" ? 0 : a.Level == "warning" ? 1 : 2)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<SystemAlert?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == doctorId);
    }

    public async Task<bool> AlertExistsForTodayAsync(Guid patientId, string ruleId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet.AnyAsync(a =>
            a.PatientId == patientId &&
            a.RuleId == ruleId &&
            a.CreatedAt >= today);
    }
}