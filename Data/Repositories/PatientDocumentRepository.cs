using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class PatientDocumentRepository : GenericRepository<PatientDocument>, IPatientDocumentRepository
{
    public PatientDocumentRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<PatientDocument>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet.AsNoTracking()
            .Where(d => d.PatientId == patientId)
            .OrderByDescending(d => d.DocumentDate)
            .ToListAsync();
    }

    public async Task<PatientDocument?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && d.DoctorId == doctorId);
    }
}