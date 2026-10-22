using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class ChronicDiseaseRepository
    : GenericRepository<ChronicDisease>, IChronicDiseaseRepository
{
    public ChronicDiseaseRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChronicDisease>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.DiagnosisDate)
            .ToListAsync();
    }

    public async Task<ChronicDisease?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.DoctorId == doctorId);
    }

    public async Task<int> CountByPatientAsync(Guid patientId)
    {
        return await _dbSet.CountAsync(c => c.PatientId == patientId);
    }
}