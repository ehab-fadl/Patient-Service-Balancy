using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class ListeningSessionRepository : GenericRepository<ListeningSession>, IListeningSessionRepository
{
    public ListeningSessionRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<ListeningSession>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.SessionDate)
            .ToListAsync();
    }

    public async Task<ListeningSession?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId);
    }
}