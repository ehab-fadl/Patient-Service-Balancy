using System;
using System.Collections.Generic;

namespace PatientService.Services.DTOs.TreatmentPlan;

public class TreatmentPlanResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PlanDoctorName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } // <-- تم إضافة هذا السطر
    public List<TreatmentProblemResponseDto> Problems { get; set; } = new();
}

public class TreatmentProblemResponseDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<TreatmentGoalResponseDto> Goals { get; set; } = new();
}

public class TreatmentGoalResponseDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal? Baseline { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? CurrentValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? AchievedAt { get; set; }
    public string? AchievedBy { get; set; }
    public string? AchievementNote { get; set; }
    public bool PatientConfirmedByDoctor { get; set; }
    public List<TreatmentMeanResponseDto> Means { get; set; } = new();
    public List<TreatmentGoalHistoryResponseDto> ProgressHistory { get; set; } = new();
}

public class TreatmentMeanResponseDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class TreatmentGoalHistoryResponseDto
{
    public Guid Id { get; set; }
    public DateTime RecordedAt { get; set; }
    public string? Value { get; set; }
    public string? Note { get; set; }
}