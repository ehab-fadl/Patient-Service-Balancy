using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class PatientCodeRepository
    : GenericRepository<PatientCode>, IPatientCodeRepository
{
    public PatientCodeRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<PatientCode?> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.PatientId == patientId);
    }

    public async Task<PatientCode?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _dbSet.AnyAsync(c => c.Code == code);
    }
}