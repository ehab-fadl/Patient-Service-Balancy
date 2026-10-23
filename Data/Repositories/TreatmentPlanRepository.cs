using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class TreatmentPlanRepository : GenericRepository<TreatmentPlan>, ITreatmentPlanRepository
{
    public TreatmentPlanRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TreatmentPlan>> GetFullTreeByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Problems)
                .ThenInclude(pr => pr.Goals)
                    .ThenInclude(g => g.Means)
            .Include(p => p.Problems)
                .ThenInclude(pr => pr.Goals)
                    .ThenInclude(g => g.ProgressHistory)
            .Where(p => p.PatientId == patientId)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();
    }

    public async Task<TreatmentPlan?> GetFullTreeByIdAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Problems)
                .ThenInclude(pr => pr.Goals)
                    .ThenInclude(g => g.Means)
            .Include(p => p.Problems)
                .ThenInclude(pr => pr.Goals)
                    .ThenInclude(g => g.ProgressHistory)
            .FirstOrDefaultAsync(p => p.Id == id && p.DoctorId == doctorId);
    }
}