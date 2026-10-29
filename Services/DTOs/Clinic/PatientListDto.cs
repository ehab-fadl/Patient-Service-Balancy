namespace PatientService.Services.DTOs.Clinic;

public class PatientListDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Code { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    // إحصائيات
    public string? CurrentDiagnosis { get; set; }
    public int ActiveMedications { get; set; }
    public int TotalVisits { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public DateTime? NextVisitDate { get; set; }

    // مؤشرات الحالة
    public string Status { get; set; } = "active"; // active, critical, inactive
    public string RiskLevel { get; set; } = "low"; // high, medium, low
}