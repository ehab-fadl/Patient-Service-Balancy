namespace PatientService.Services.DTOs.Documents;

public class DocumentResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public string? Issuer { get; set; }
    public string? Category { get; set; }
    public string? RefNo { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsSigned { get; set; }
    public bool IsCritical { get; set; }
    public bool IsFavorite { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }

    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long FileSize { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}