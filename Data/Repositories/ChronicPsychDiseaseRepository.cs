using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class ChronicPsychDiseaseRepository
    : GenericRepository<ChronicPsychDisease>, IChronicPsychDiseaseRepository
{
    public ChronicPsychDiseaseRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChronicPsychDisease>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.DiagnosisDate)
            .ToListAsync();
    }

    public async Task<ChronicPsychDisease?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.DoctorId == doctorId);
    }
}