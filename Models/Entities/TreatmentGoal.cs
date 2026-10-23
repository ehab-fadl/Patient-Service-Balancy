using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class TreatmentGoal : BaseEntity
{
    public Guid TreatmentProblemId { get; set; }
    public string Text { get; set; } = string.Empty;

    // إعدادات القياس
    public string? Unit { get; set; }
    public decimal? Baseline { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? CurrentValue { get; set; }

    // حالة الهدف
    public string Status { get; set; } = "pending"; // pending, achieved, partial, rejected
    public DateTime? AchievedAt { get; set; }
    public string? AchievedBy { get; set; } // doctor, patient
    public string? AchievementNote { get; set; }
    public bool PatientConfirmedByDoctor { get; set; } = false;

    // Navigation Properties
    public TreatmentProblem TreatmentProblem { get; set; } = null!;
    public ICollection<TreatmentMean> Means { get; set; } = new List<TreatmentMean>();
    public ICollection<TreatmentGoalHistory> ProgressHistory { get; set; } = new List<TreatmentGoalHistory>();
}