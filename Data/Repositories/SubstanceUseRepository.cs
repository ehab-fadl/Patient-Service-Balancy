using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class SubstanceUseRepository : GenericRepository<SubstanceUse>, ISubstanceUseRepository
{
    public SubstanceUseRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<SubstanceUse>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.LastUseDate)
            .ToListAsync();
    }

    public async Task<SubstanceUse?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId);
    }
}