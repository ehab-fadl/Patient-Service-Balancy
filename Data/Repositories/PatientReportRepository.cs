using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class PatientReportRepository : GenericRepository<PatientReport>, IPatientReportRepository
{
    public PatientReportRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<PatientReport>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .Where(r => r.PatientId == patientId && r.DoctorId == doctorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<PatientReport?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && r.DoctorId == doctorId);
    }
}