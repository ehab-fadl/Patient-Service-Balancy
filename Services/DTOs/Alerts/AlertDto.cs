namespace PatientService.Services.DTOs.Alerts;

public class AlertDto
{
    public Guid Id { get; set; }
    public string RuleId { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ActionLabel { get; set; } = string.Empty;

    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientCode { get; set; } = string.Empty;

    public bool IsDismissed { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AlertsSummaryDto
{
    public List<AlertDto> Alerts { get; set; } = new();
    public int CriticalCount { get; set; }
    public int WarningCount { get; set; }
    public int InfoCount { get; set; }
    public DateTime LastUpdated { get; set; }
}