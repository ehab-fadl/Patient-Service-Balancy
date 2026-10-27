using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class DoctorNoteRepository : GenericRepository<DoctorNote>, IDoctorNoteRepository
{
    public DoctorNoteRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<DoctorNote>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet.AsNoTracking()
            .Where(n => n.PatientId == patientId)
            .OrderByDescending(n => n.NoteDate)
            .ToListAsync();
    }

    public async Task<DoctorNote?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id && n.DoctorId == doctorId);
    }
}