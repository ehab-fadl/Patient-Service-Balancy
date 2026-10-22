using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class CssrsAssessmentRepository
    : GenericRepository<CssrsAssessment>, ICssrsAssessmentRepository
{
    public CssrsAssessmentRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CssrsAssessment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<CssrsAssessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.DoctorId == doctorId);
    }
}