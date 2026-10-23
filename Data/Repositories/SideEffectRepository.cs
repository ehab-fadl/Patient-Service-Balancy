using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class SideEffectRepository
    : GenericRepository<SideEffect>, ISideEffectRepository
{
    public SideEffectRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SideEffect>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<SideEffect?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId);
    }
}