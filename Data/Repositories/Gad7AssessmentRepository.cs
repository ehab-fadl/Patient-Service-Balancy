using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class Gad7AssessmentRepository
    : GenericRepository<Gad7Assessment>, IGad7AssessmentRepository
{
    public Gad7AssessmentRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Gad7Assessment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(g => g.PatientId == patientId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<Gad7Assessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id && g.DoctorId == doctorId);
    }
}