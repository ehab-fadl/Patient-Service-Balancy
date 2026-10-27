using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class PatientNoteRepository : GenericRepository<PatientNote>, IPatientNoteRepository
{
    public PatientNoteRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<PatientNote>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet.AsNoTracking()
            .Where(n => n.PatientId == patientId)
            .OrderByDescending(n => n.NoteDate)
            .ToListAsync();
    }

    public async Task<PatientNote?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id && n.DoctorId == doctorId);
    }
}