using System;

namespace PatientService.Services.DTOs.TreatmentPlan;

// --- Problems ---
public class TreatmentProblemCreateDto
{
    public Guid TreatmentPlanId { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class TreatmentProblemUpdateDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

// --- Goals ---
public class TreatmentGoalCreateDto
{
    public Guid TreatmentProblemId { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class TreatmentGoalUpdateDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal? Baseline { get; set; }
    public decimal? TargetValue { get; set; }
}

public class TreatmentGoalProgressDto
{
    public Guid GoalId { get; set; }
    public decimal Value { get; set; }
    public string? Note { get; set; }
}

public class TreatmentGoalAchieveDto
{
    public Guid GoalId { get; set; }
    public string? Note { get; set; }
}

// --- Means ---
public class TreatmentMeanCreateDto
{
    public Guid TreatmentGoalId { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class TreatmentMeanUpdateDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
public class TreatmentGoalUnachieveDto
{
    public Guid GoalId { get; set; }
}