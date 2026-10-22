using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class VitalSignRepository
    : GenericRepository<VitalSign>, IVitalSignRepository
{
    public VitalSignRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<VitalSign>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.MeasurementDate)
            .ToListAsync();
    }

    public async Task<VitalSign?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id && v.DoctorId == doctorId);
    }
}