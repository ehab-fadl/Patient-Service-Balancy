using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class MedicationRepository
    : GenericRepository<Medication>, IMedicationRepository
{
    public MedicationRepository(PatientDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Medication>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.StartDate)
            .ToListAsync();
    }

    public async Task<Medication?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctorId);
    }
}