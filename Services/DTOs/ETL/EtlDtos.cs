namespace PatientService.Services.DTOs.ETL;

// ═══════════════════════════════════════════════════════════════
// Base Response
// ═══════════════════════════════════════════════════════════════
public class EtlResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public PaginationMeta Pagination { get; set; } = new();
    public SyncMetadata SyncMetadata { get; set; } = new();
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}

public class SyncMetadata
{
    public DateTime DataAsOf { get; set; }
    public string DeidentificationVersion { get; set; } = "1.0";
    public string? ETLSessionId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 1. Health
// ═══════════════════════════════════════════════════════════════
public class EtlHealthDto
{
    public string Status { get; set; } = "healthy";
    public DateTime ServerTimeUtc { get; set; }
    public string DbVersion { get; set; } = "1.0";
    public DateTime? LastSuccessfulSync { get; set; }
    public List<string> SupportedEntities { get; set; } = new();
}

// ═══════════════════════════════════════════════════════════════
// 2. Patients
// ═══════════════════════════════════════════════════════════════
public class EtlPatientDto
{
    public string PatientHash { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? MaritalStatus { get; set; }
    public string CreatedQuarter { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 3. Assessments (5 types merged)
// ═══════════════════════════════════════════════════════════════
public class EtlAssessmentDto
{
    public string AssessmentHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string AssessmentType { get; set; } = string.Empty; // GAD7, PHQ9, MDQ, SDS, CSSRS
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string AssessmentQuarter { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 4. Medications
// ═══════════════════════════════════════════════════════════════
public class EtlMedicationDto
{
    public string MedicationHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string MedicationClass { get; set; } = string.Empty;
    public string DoseCategory { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StartQuarter { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 5. Diagnoses
// ═══════════════════════════════════════════════════════════════
public class EtlDiagnosisDto
{
    public string DiagnosisHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string MddType { get; set; } = string.Empty;
    public string? MddSubtype { get; set; }
    public string DiagnosisQuarter { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 6. Chronic Conditions (physical + psychiatric)
// ═══════════════════════════════════════════════════════════════
public class EtlChronicConditionDto
{
    public string ConditionHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string ConditionCategory { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty; // "chronic_physical" | "psychiatric"
    public string? Severity { get; set; }
    public string? TreatmentResponse { get; set; }
    public bool HasHospitalization { get; set; }
    public string DiagnosisQuarter { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 7. Family History
// ═══════════════════════════════════════════════════════════════
public class EtlFamilyHistoryDto
{
    public string FamilyHistoryHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty;
    public string? Severity { get; set; }
    public bool HasCondition { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 8. Substance Use
// ═══════════════════════════════════════════════════════════════
public class EtlSubstanceUseDto
{
    public string SubstanceUseHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string SubstanceCategory { get; set; } = string.Empty;
    public string? Severity { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
    public int CravingLevel { get; set; }
    public bool HasPhysicalDependence { get; set; }
    public bool HasPsychologicalDependence { get; set; }
    public bool HasImpactWork { get; set; }
    public bool HasImpactHealth { get; set; }
    public int? QuitAttempts { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 9. Listening Sessions
// ═══════════════════════════════════════════════════════════════
public class EtlListeningSessionDto
{
    public string SessionHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string? TherapyType { get; set; }
    public string? Format { get; set; }
    public int MoodBefore { get; set; }
    public int MoodAfter { get; set; }
    public int MoodChange => MoodAfter - MoodBefore;
    public int? DurationMinutes { get; set; }
    public string? RiskLevel { get; set; }
    public bool HomeworkCompleted { get; set; }
    public string SessionQuarter { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 10. Visits
// ═══════════════════════════════════════════════════════════════
public class EtlVisitDto
{
    public string VisitHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string VisitType { get; set; } = string.Empty;
    public string VisitLocation { get; set; } = string.Empty;
    public string VisitQuarter { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 11. Treatment Plans (flattened)
// ═══════════════════════════════════════════════════════════════
public class EtlTreatmentPlanGoalDto
{
    public string PlanHash { get; set; } = string.Empty;
    public string GoalHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string PlanStatus { get; set; } = string.Empty;
    public int PlanDurationDays { get; set; }
    public string? ProblemCategory { get; set; }
    public string GoalStatus { get; set; } = string.Empty;
    public decimal? GoalBaseline { get; set; }
    public decimal? GoalTarget { get; set; }
    public decimal? GoalCurrent { get; set; }
    public int? GoalProgressPercentage { get; set; }
    public DateTime? AchievedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 12. Vital Signs
// ═══════════════════════════════════════════════════════════════
public class EtlVitalSignDto
{
    public string VitalHash { get; set; } = string.Empty;
    public string PatientHash { get; set; } = string.Empty;
    public string VitalType { get; set; } = string.Empty;
    public decimal Value1 { get; set; }
    public decimal? Value2 { get; set; }
    public string Status { get; set; } = string.Empty;
    public string MeasurementQuarter { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}