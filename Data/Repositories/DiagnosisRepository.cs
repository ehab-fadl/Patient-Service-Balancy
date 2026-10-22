using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class DiagnosisRepository
    : GenericRepository<Diagnosis>, IDiagnosisRepository
{
    public DiagnosisRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(d => d.PatientId == patientId)
            .OrderByDescending(d => d.DiagnosisDate)
            .ToListAsync();
    }

    public async Task<Diagnosis?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && d.DoctorId == doctorId);
    }
}