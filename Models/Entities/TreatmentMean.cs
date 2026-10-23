using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class TreatmentMean : BaseEntity
{
    public Guid TreatmentGoalId { get; set; }
    public string Text { get; set; } = string.Empty;

    // Navigation Property
    public TreatmentGoal TreatmentGoal { get; set; } = null!;
}