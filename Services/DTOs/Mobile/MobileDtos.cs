namespace PatientService.Services.DTOs.Mobile;

// ═══════════════════════════════════════════════════════════════
// 1. Auth & Profile
// ═══════════════════════════════════════════════════════════════
public class MobileLoginDto
{
    public string PatientCode { get; set; } = string.Empty;
}

public class MobileLoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public MobilePatientProfileDto? Patient { get; set; }
}

public class MobilePatientProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string PatientCode { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string? CurrentDiagnosis { get; set; }
    public string? ActiveTreatmentPlanName { get; set; }
    public int ActiveMedicationsCount { get; set; }
    public int TotalVisits { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public DateTime? NextAppointmentDate { get; set; }
    public string? RiskLevel { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 2. Daily Metrics
// ═══════════════════════════════════════════════════════════════
public class MobileDailyMetricDto
{
    public DateTime LogDate { get; set; }
    public int Food { get; set; }
    public decimal SleepHours { get; set; }
    public int MedicationAdherence { get; set; }
    public int Mood { get; set; }
    public int NegativeThoughts { get; set; }
    public int PhysicalHealth { get; set; }
    public int Energy { get; set; }
    public int Motivation { get; set; }
    public int Concentration { get; set; }
    public int Guilt { get; set; }
    public int Hope { get; set; }
    public int SuicidalIdeation { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 3. Assessments
// ═══════════════════════════════════════════════════════════════
public class MobileAssessmentSubmitDto
{
    public string AssessmentType { get; set; } = string.Empty; // GAD7, PHQ9, MDQ, SDS, CSSRS
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? AnswersJson { get; set; }
}

public class MobileAssessmentHistoryDto
{
    public string AssessmentType { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 4. Treatment Plan
// ═══════════════════════════════════════════════════════════════
public class MobileTreatmentPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<MobileProblemDto> Problems { get; set; } = new();
}

public class MobileProblemDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<MobileGoalDto> Goals { get; set; } = new();
}

public class MobileGoalDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal? Baseline { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? CurrentValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool PatientConfirmedByDoctor { get; set; }
    public List<MobileMeanDto> Means { get; set; } = new();
}

public class MobileMeanDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class MobileGoalProgressDto
{
    public decimal Value { get; set; }
    public string? Note { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 5. Medications & Side Effects
// ═══════════════════════════════════════════════════════════════
public class MobileMedicationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dose { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string IntakePeriod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class MobileSideEffectSubmitDto
{
    public Guid MedicationId { get; set; }
    public string Symptom { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 6. Appointments
// ═══════════════════════════════════════════════════════════════
public class MobileAppointmentDto
{
    public Guid Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? ScheduledTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string VisitLocation { get; set; } = string.Empty;
}

public class MobileAppointmentCancelDto
{
    public Guid AppointmentId { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}

public class MobileRescheduleDto
{
    public DateTime NewDate { get; set; }
    public string? NewTime { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// 7. Alerts
// ═══════════════════════════════════════════════════════════════
public class MobileAlertDto
{
    public Guid Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}