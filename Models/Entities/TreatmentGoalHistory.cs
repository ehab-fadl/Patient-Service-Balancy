using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class TreatmentGoalHistory : BaseEntity
{
    public Guid TreatmentGoalId { get; set; }
    public DateTime RecordedAt { get; set; }
    public string? Value { get; set; } // يمكن أن يكون رقماً أو نصاً (مثل "محقق ✅")
    public string? Note { get; set; }

    // Navigation Property
    public TreatmentGoal TreatmentGoal { get; set; } = null!;
}