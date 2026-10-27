using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface ISubstanceUseRepository : IGenericRepository<SubstanceUse>
{
    Task<IEnumerable<SubstanceUse>> GetByPatientIdAsync(Guid patientId);
    Task<SubstanceUse?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}