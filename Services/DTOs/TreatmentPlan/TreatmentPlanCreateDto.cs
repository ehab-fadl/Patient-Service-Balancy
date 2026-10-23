using System;

namespace PatientService.Services.DTOs.TreatmentPlan;

public class TreatmentPlanCreateDto
{
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PlanDoctorName { get; set; }
    public string? Notes { get; set; }
}

public class TreatmentPlanUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PlanDoctorName { get; set; }
    public string? Notes { get; set; }
}