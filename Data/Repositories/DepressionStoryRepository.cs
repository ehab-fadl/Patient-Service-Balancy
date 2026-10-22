using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class DepressionStoryRepository
    : GenericRepository<DepressionStory>, IDepressionStoryRepository
{
    public DepressionStoryRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<DepressionStory?> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.PatientId == patientId);
    }

    public async Task<bool> ExistsForPatientAsync(Guid patientId)
    {
        return await _dbSet.AnyAsync(s => s.PatientId == patientId);
    }
}