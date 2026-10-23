namespace PatientService.Services.DTOs.SideEffect;

public class SideEffectUpdateDto
{
    public Guid Id { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Symptom { get; set; } = string.Empty;
    public string? Notes { get; set; }
}