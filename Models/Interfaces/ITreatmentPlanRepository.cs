using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface ITreatmentPlanRepository : IGenericRepository<TreatmentPlan>
{
    Task<IEnumerable<TreatmentPlan>> GetFullTreeByPatientIdAsync(Guid patientId);
    Task<TreatmentPlan?> GetFullTreeByIdAsync(Guid id, Guid doctorId);
}