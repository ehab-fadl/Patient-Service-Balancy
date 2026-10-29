using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(PatientDbContext context) : base(context) { }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.Status != "Cancelled")
            .OrderBy(a => a.ScheduledDate)
            .ThenBy(a => a.ScheduledTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == doctorId);
    }

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(Guid doctorId, string status)
    {
        return await _dbSet.AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.Status == status)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(Guid doctorId, DateTime date, string? time, Guid? excludeAppointmentId = null)
    {
        var query = _dbSet.Where(a =>
            a.DoctorId == doctorId &&
            a.ScheduledDate.Date == date.Date &&
            a.Status != "Cancelled" &&
            a.Status != "Completed");

        if (!string.IsNullOrEmpty(time))
            query = query.Where(a => a.ScheduledTime == time);

        if (excludeAppointmentId.HasValue)
            query = query.Where(a => a.Id != excludeAppointmentId.Value);

        return await query.AnyAsync();
    }
}