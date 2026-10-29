namespace PatientService.Services.DTOs.Clinic;

public class ClinicStatisticsDto
{
    // ─── KPIs الأساسية ───
    public int TotalPatients { get; set; }
    public int ActivePatients { get; set; }
    public int CriticalPatients { get; set; }
    public int TodayVisits { get; set; }

    // ─── KPIs الثانوية ───
    public int WeekVisits { get; set; }
    public int NewPatientsThisMonth { get; set; }
    public int AdherenceRate { get; set; }    // نسبة الالتزام (0-100)
    public int AvgSessionDuration { get; set; } // متوسط مدة الجلسة بالدقائق

    // ─── الزيارات حسب الفترة ───
    public VisitsByPeriodDto VisitsByPeriod { get; set; } = new();

    // ─── التوزيعات ───
    public DistributionDto DiagnosesDistribution { get; set; } = new();
    public DistributionDto AgeDistribution { get; set; } = new();
    public DistributionDto GenderDistribution { get; set; } = new();
    public DistributionDto HealthDistribution { get; set; } = new();

    // ─── متوسطات التقييمات النفسية ───
    public AssessmentAveragesDto AssessmentAverages { get; set; } = new();

    // ─── القوائم العلوية ───
    public List<TopDiagnosisDto> TopDiagnoses { get; set; } = new();
    public List<TopPatientDto> TopPatients { get; set; } = new();
    public List<ActivityDto> RecentActivities { get; set; } = new();

    // ─── الرؤى الذكية (Insights) ───
    public List<InsightDto> Insights { get; set; } = new();
}

// ────────────────────────────────────────────
public class VisitsByPeriodDto
{
    public ChartDataDto Last7Days { get; set; } = new();
    public ChartDataDto Last30Days { get; set; } = new();
    public ChartDataDto Last90Days { get; set; } = new();
    public ChartDataDto Last365Days { get; set; } = new();
}

public class ChartDataDto
{
    public List<string> Labels { get; set; } = new();
    public List<int> Values { get; set; } = new();
}

public class DistributionDto
{
    public List<string> Labels { get; set; } = new();
    public List<int> Values { get; set; } = new();
}

public class AssessmentAveragesDto
{
    public List<string> Labels { get; set; } = new();
    public List<double> Gad7 { get; set; } = new();
    public List<double> Phq9 { get; set; } = new();
    public List<double> Sds { get; set; } = new();
}

public class TopDiagnosisDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percent { get; set; }
}

public class TopPatientDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Visits { get; set; }
}

public class ActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Type { get; set; } = "primary"; // primary, success, warning, danger
}

public class InsightDto
{
    public string Type { get; set; } = "info"; // success, warning, danger, info
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
}