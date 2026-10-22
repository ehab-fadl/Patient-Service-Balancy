namespace PatientService.Services.DTOs.FamilyHistory;

public class FamilyHistoryUpdateDto
{
    public Guid Id { get; set; }
    public string Relation { get; set; } = string.Empty;
    public string? RelativeName { get; set; }
    public int? Age { get; set; }
    public string? Status { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty;
    public string Conditions { get; set; } = string.Empty;
    public string? Notes { get; set; }
}