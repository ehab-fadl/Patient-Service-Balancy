using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class TreatmentProblem : BaseEntity
{
    public Guid TreatmentPlanId { get; set; }
    public string Text { get; set; } = string.Empty;

    // Navigation Properties
    public TreatmentPlan TreatmentPlan { get; set; } = null!;
    public ICollection<TreatmentGoal> Goals { get; set; } = new List<TreatmentGoal>();
}