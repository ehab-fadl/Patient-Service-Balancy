namespace PatientService.Services.DTOs.Appointments;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientCode { get; set; } = string.Empty;

    public DateTime ScheduledDate { get; set; }
    public string? ScheduledTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string VisitType { get; set; } = string.Empty;
    public string VisitLocation { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Guid? VisitId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AppointmentCreateDto
{
    public Guid PatientId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? ScheduledTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string VisitType { get; set; } = "متابعة";
    public string VisitLocation { get; set; } = "عيادة";
    public string? Notes { get; set; }
}

public class AppointmentUpdateDto
{
    public Guid Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? ScheduledTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string VisitType { get; set; } = string.Empty;
    public string VisitLocation { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class AppointmentCancelDto
{
    public Guid Id { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}

public class AppointmentsSummaryDto
{
    public List<AppointmentDto> Appointments { get; set; } = new();
    public int TodayCount { get; set; }
    public int WeekCount { get; set; }
    public int MonthCount { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int CompletedCount { get; set; }
}