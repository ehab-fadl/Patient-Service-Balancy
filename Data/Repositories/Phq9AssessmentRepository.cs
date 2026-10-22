using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class Phq9AssessmentRepository
    : GenericRepository<Phq9Assessment>, IPhq9AssessmentRepository
{
    public Phq9AssessmentRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Phq9Assessment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.PatientId == patientId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Phq9Assessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.DoctorId == doctorId);
    }
}