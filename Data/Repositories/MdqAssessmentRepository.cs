using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class MdqAssessmentRepository
    : GenericRepository<MdqAssessment>, IMdqAssessmentRepository
{
    public MdqAssessmentRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MdqAssessment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<MdqAssessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctorId);
    }
}