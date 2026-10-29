namespace PatientService.Services.DTOs.Dashboard;

public class DashboardDto
{
    // ─── ترحيب ───
    public string DoctorName { get; set; } = "الطبيب";
    public DateTime TodayDate { get; set; }
    public string GreetingText { get; set; } = string.Empty;

    // ─── Quick Stats ───
    public int TodayAppointmentsCount { get; set; }
    public int TodayAppointmentsChangeFromYesterday { get; set; }
    public int TotalPatients { get; set; }
    public int TotalPatientsChangePercent { get; set; }
    public int CriticalPatients { get; set; }
    public int CriticalPatientsChangeFromYesterday { get; set; }
    public int NewPatientsThisMonth { get; set; }
    public int NewPatientsChangeFromLastMonth { get; set; }

    // ─── Today's Appointments ───
    public List<DashboardAppointmentDto> TodayAppointments { get; set; } = new();

    // ─── Active Alerts ───
    public List<DashboardAlertDto> ActiveAlerts { get; set; } = new();
    public int TotalActiveAlerts { get; set; }

    // ─── Recent Patients ───
    public List<DashboardPatientDto> RecentPatients { get; set; } = new();
}

public class DashboardAppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? ScheduledTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending, Confirmed
    public string PatientInitials { get; set; } = string.Empty;
}

public class DashboardAlertDto
{
    public Guid Id { get; set; }
    public string Level { get; set; } = string.Empty; // critical, warning, info
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DashboardPatientDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PatientCode { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? LastDiagnosis { get; set; }
}