using PatientService.Models.Entities;
using System.Linq.Expressions;

namespace PatientService.Models.Interfaces;

public interface ITreatmentPlanRepository : IGenericRepository<TreatmentPlan>
{
    Task<IEnumerable<TreatmentPlan>> GetFullTreeByPatientIdAsync(Guid patientId);
    Task<TreatmentPlan?> GetFullTreeByIdAsync(Guid id, Guid doctorId);
    Task<IEnumerable<TreatmentPlan>> FindFullTreesAsync(Expression<Func<TreatmentPlan, bool>> predicate);
}