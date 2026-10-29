using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId);
    Task<Appointment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
    Task<IEnumerable<Appointment>> GetByStatusAsync(Guid doctorId, string status);
    Task<bool> HasConflictAsync(Guid doctorId, DateTime date, string? time, Guid? excludeAppointmentId = null);
}