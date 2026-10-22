using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class PatientPersonalInfoRepository
    : GenericRepository<PatientPersonalInfo>, IPatientPersonalInfoRepository
{
    public PatientPersonalInfoRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PatientPersonalInfo>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var term = searchTerm.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(p =>
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientPersonalInfo>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.DoctorId == doctorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientPersonalInfo>> SearchByDoctorIdAsync(Guid doctorId, string searchTerm)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(p => p.DoctorId == doctorId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public override async Task<IEnumerable<PatientPersonalInfo>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}