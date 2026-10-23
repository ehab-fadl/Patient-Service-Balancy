using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface ISideEffectRepository : IGenericRepository<SideEffect>
{
    Task<IEnumerable<SideEffect>> GetByPatientIdAsync(Guid patientId);
    Task<SideEffect?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}