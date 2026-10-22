using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class FamilyHistoryRepository
    : GenericRepository<FamilyHistory>, IFamilyHistoryRepository
{
    public FamilyHistoryRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<FamilyHistory>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<FamilyHistory?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.DoctorId == doctorId);
    }
}