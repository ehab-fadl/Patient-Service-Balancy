namespace PatientService.Services.DTOs.VitalSign;

public class VitalSignUpdateDto
{
    public Guid Id { get; set; }
    public string VitalType { get; set; } = string.Empty;
    public DateTime MeasurementDate { get; set; }
    public decimal Value1 { get; set; }
    public decimal? Value2 { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}