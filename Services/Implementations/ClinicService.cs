using Microsoft.Extensions.Logging;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Clinic;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class ClinicService : IClinicService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClinicService> _logger;

    public ClinicService(IUnitOfWork unitOfWork, ILogger<ClinicService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════════════════
    // 1. جلب جميع المرضى (من الرد السابق)
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<IEnumerable<PatientListDto>>> GetAllPatientsAsync(Guid doctorId)
    {
        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var result = new List<PatientListDto>();
        var allVisits = await _unitOfWork.Visits.FindAsync(v => v.DoctorId == doctorId);
        var allDiagnoses = await _unitOfWork.Diagnoses.FindAsync(d => d.DoctorId == doctorId);
        var allMedications = await _unitOfWork.Medications.FindAsync(m => m.DoctorId == doctorId);
        var allCssrs = await _unitOfWork.CssrsAssessments.FindAsync(c => c.DoctorId == doctorId);
        var allCodes = await _unitOfWork.PatientCodes.GetAllAsync();

        foreach (var patient in patients)
        {
            var code = allCodes.FirstOrDefault(c => c.PatientId == patient.Id);
            var latestDiagnosis = allDiagnoses.Where(d => d.PatientId == patient.Id).OrderByDescending(d => d.DiagnosisDate).FirstOrDefault()?.MainType;
            var activeMeds = allMedications.Count(m => m.PatientId == patient.Id && m.Status == "نشط");
            var patientVisits = allVisits.Where(v => v.PatientId == patient.Id).OrderByDescending(v => v.VisitDate).ToList();
            var latestCssrs = allCssrs.Where(c => c.PatientId == patient.Id).OrderByDescending(c => c.CreatedAt).FirstOrDefault();

            var riskLevel = latestCssrs?.Severity ?? "low";
            var status = "active";
            if (riskLevel == "critical" || riskLevel == "high") status = "critical";
            else if (patientVisits.Count == 0 || (DateTime.UtcNow - patientVisits.First().VisitDate).Days > 90) status = "inactive";

            result.Add(new PatientListDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Code = code?.Code ?? "—",
                Age = CalculateAge(patient.DateOfBirth),
                Gender = patient.Gender,
                Phone = patient.Phone,
                City = patient.City,
                MaritalStatus = patient.MaritalStatus,
                Address = patient.Address,
                Email = patient.Email,
                CreatedAt = patient.CreatedAt,
                CurrentDiagnosis = latestDiagnosis,
                ActiveMedications = activeMeds,
                TotalVisits = patientVisits.Count,
                LastVisitDate = patientVisits.FirstOrDefault()?.VisitDate,
                NextVisitDate = patientVisits.FirstOrDefault()?.NextVisitDate,
                Status = status,
                RiskLevel = riskLevel
            });
        }
        return ServiceResult<IEnumerable<PatientListDto>>.SuccessResult(result);
    }

    // ═══════════════════════════════════════════════════════════════
    // 2. إحصائيات العيادة الشاملة
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<ClinicStatisticsDto>> GetStatisticsAsync(Guid doctorId)
    {
        var stats = new ClinicStatisticsDto();

        // ─── جلب كل البيانات الخام مرة واحدة (لتجنب N+1) ───
        var patients = (await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId)).ToList();
        var visits = (await _unitOfWork.Visits.FindAsync(v => v.DoctorId == doctorId)).ToList();
        var diagnoses = (await _unitOfWork.Diagnoses.FindAsync(d => d.DoctorId == doctorId)).ToList();
        var medications = (await _unitOfWork.Medications.FindAsync(m => m.DoctorId == doctorId)).ToList();
        var cssrsList = (await _unitOfWork.CssrsAssessments.FindAsync(c => c.DoctorId == doctorId)).ToList();
        var sessions = (await _unitOfWork.ListeningSessions.FindAsync(s => s.DoctorId == doctorId)).ToList();

        var gad7 = (await _unitOfWork.Gad7Assessments.FindAsync(g => g.DoctorId == doctorId)).ToList();
        var phq9 = (await _unitOfWork.Phq9Assessments.FindAsync(p => p.DoctorId == doctorId)).ToList();
        var sds = (await _unitOfWork.SdsAssessments.FindAsync(s => s.DoctorId == doctorId)).ToList();

        var today = DateTime.UtcNow.Date;
        var weekAgo = today.AddDays(-7);
        var monthAgo = today.AddDays(-30);

        // ═══ 1. KPIs الأساسية ═══
        stats.TotalPatients = patients.Count;

        var activePatientIds = visits.Where(v => v.VisitDate >= monthAgo).Select(v => v.PatientId).Distinct();
        stats.ActivePatients = activePatientIds.Count();

        // مرضى حرجون (آخر C-SSRS حرج/عالي)
        var criticalPatientIds = cssrsList
            .GroupBy(c => c.PatientId)
            .Where(g => {
                var latest = g.OrderByDescending(c => c.CreatedAt).First();
                return latest.IsCritical || latest.Severity == "high" || latest.Severity == "critical";
            })
            .Select(g => g.Key).Distinct();
        stats.CriticalPatients = criticalPatientIds.Count();

        stats.TodayVisits = visits.Count(v => v.VisitDate.Date == today);
        stats.WeekVisits = visits.Count(v => v.VisitDate >= weekAgo);
        stats.NewPatientsThisMonth = patients.Count(p => p.CreatedAt >= monthAgo);

        // نسبة الالتزام (نسبة المرضى النشطين من الإجمالي)
        stats.AdherenceRate = stats.TotalPatients > 0
            ? (int)Math.Round((double)stats.ActivePatients / stats.TotalPatients * 100)
            : 0;

        // متوسط مدة الجلسة
        var durationsWithValue = sessions.Where(s => s.DurationMinutes.HasValue && s.DurationMinutes.Value > 0).ToList();
        stats.AvgSessionDuration = durationsWithValue.Count > 0
            ? (int)Math.Round(durationsWithValue.Average(s => s.DurationMinutes!.Value))
            : 0;

        // ═══ 2. الزيارات حسب الفترات ═══
        stats.VisitsByPeriod.Last7Days = BuildVisitsLast7Days(visits, today);
        stats.VisitsByPeriod.Last30Days = BuildVisitsLast30Days(visits, today);
        stats.VisitsByPeriod.Last90Days = BuildVisitsLast3Months(visits, today);
        stats.VisitsByPeriod.Last365Days = BuildVisitsLastYear(visits, today);

        // ═══ 3. توزيع التشخيصات ═══
        stats.DiagnosesDistribution = BuildDiagnosesDistribution(patients, diagnoses);

        // ═══ 4. توزيع الأعمار ═══
        stats.AgeDistribution = BuildAgeDistribution(patients);

        // ═══ 5. توزيع الجنس ═══
        stats.GenderDistribution = BuildGenderDistribution(patients);

        // ═══ 6. مؤشرات الصحة النفسية ═══
        stats.HealthDistribution = BuildHealthDistribution(visits, cssrsList, patients, today);

        // ═══ 7. متوسطات التقييمات النفسية ═══
        stats.AssessmentAverages = BuildAssessmentAverages(gad7, phq9, sds);

        // ═══ 8. القوائم العلوية ═══
        stats.TopDiagnoses = BuildTopDiagnoses(diagnoses, patients);
        stats.TopPatients = BuildTopPatients(visits, patients);
        stats.RecentActivities = BuildRecentActivities(patients, visits, sessions, diagnoses);

        // ═══ 9. الرؤى الذكية ═══
        stats.Insights = BuildInsights(stats, cssrsList, visits, today);

        _logger.LogInformation("Statistics generated for doctor {DoctorId}: {Patients} patients, {Visits} visits",
            doctorId, stats.TotalPatients, visits.Count);

        return ServiceResult<ClinicStatisticsDto>.SuccessResult(stats);
    }

    // ═══════════════════════════════════════════════════════════════
    // Helper Builders
    // ═══════════════════════════════════════════════════════════════

    private static ChartDataDto BuildVisitsLast7Days(List<Models.Entities.Visit> visits, DateTime today)
    {
        var dayNames = new[] { "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت" };
        var weekAgo = today.AddDays(-7);
        var counts = new int[7];
        foreach (var v in visits.Where(v => v.VisitDate >= weekAgo))
            counts[(int)v.VisitDate.DayOfWeek]++;

        return new ChartDataDto
        {
            Labels = dayNames.ToList(),
            Values = counts.ToList()
        };
    }

    private static ChartDataDto BuildVisitsLast30Days(List<Models.Entities.Visit> visits, DateTime today)
    {
        var labels = new List<string> { "أسبوع 1", "أسبوع 2", "أسبوع 3", "أسبوع 4" };
        var values = new int[4];
        foreach (var v in visits.Where(v => v.VisitDate >= today.AddDays(-30)))
        {
            var daysAgo = (today - v.VisitDate.Date).Days;
            if (daysAgo >= 0 && daysAgo < 28)
                values[3 - (daysAgo / 7)]++;
        }
        return new ChartDataDto { Labels = labels, Values = values.ToList() };
    }

    private static ChartDataDto BuildVisitsLast3Months(List<Models.Entities.Visit> visits, DateTime today)
    {
        var labels = new List<string>();
        var values = new List<int>();
        for (int i = 2; i >= 0; i--)
        {
            var month = today.AddMonths(-i);
            labels.Add(month.ToString("MMMM", new System.Globalization.CultureInfo("ar-EG")));
            values.Add(visits.Count(v => v.VisitDate.Year == month.Year && v.VisitDate.Month == month.Month));
        }
        return new ChartDataDto { Labels = labels, Values = values };
    }

    private static ChartDataDto BuildVisitsLastYear(List<Models.Entities.Visit> visits, DateTime today)
    {
        var labels = new List<string>();
        var values = new List<int>();
        var arCulture = new System.Globalization.CultureInfo("ar-EG");
        for (int i = 11; i >= 0; i--)
        {
            var month = today.AddMonths(-i);
            labels.Add(month.ToString("MMMM", arCulture));
            values.Add(visits.Count(v => v.VisitDate.Year == month.Year && v.VisitDate.Month == month.Month));
        }
        return new ChartDataDto { Labels = labels, Values = values };
    }

    private static DistributionDto BuildDiagnosesDistribution(
        List<Models.Entities.PatientPersonalInfo> patients,
        List<Models.Entities.Diagnosis> diagnoses)
    {
        var grouped = diagnoses
            .Where(d => patients.Any(p => p.Id == d.PatientId))
            .GroupBy(d => d.MainType)
            .OrderByDescending(g => g.Select(x => x.PatientId).Distinct().Count())
            .Take(6)
            .ToList();

        if (!grouped.Any())
            return new DistributionDto { Labels = new() { "لا توجد بيانات" }, Values = new() { 100 } };

        var total = patients.Count;
        return new DistributionDto
        {
            Labels = grouped.Select(g => g.Key).ToList(),
            Values = grouped.Select(g => total > 0
                ? (int)Math.Round((double)g.Select(x => x.PatientId).Distinct().Count() / total * 100)
                : 0).ToList()
        };
    }

    private static DistributionDto BuildAgeDistribution(List<Models.Entities.PatientPersonalInfo> patients)
    {
        var groups = new Dictionary<string, int>
        {
            { "18-25", 0 }, { "26-35", 0 }, { "36-45", 0 },
            { "46-55", 0 }, { "56-65", 0 }, { "65+", 0 }
        };
        foreach (var p in patients)
        {
            var age = CalculateAge(p.DateOfBirth);
            if (age <= 25) groups["18-25"]++;
            else if (age <= 35) groups["26-35"]++;
            else if (age <= 45) groups["36-45"]++;
            else if (age <= 55) groups["46-55"]++;
            else if (age <= 65) groups["56-65"]++;
            else groups["65+"]++;
        }
        return new DistributionDto { Labels = groups.Keys.ToList(), Values = groups.Values.ToList() };
    }

    private static DistributionDto BuildGenderDistribution(List<Models.Entities.PatientPersonalInfo> patients)
    {
        var males = patients.Count(p => p.Gender == "ذكر");
        var females = patients.Count(p => p.Gender == "أنثى");
        return new DistributionDto
        {
            Labels = new() { "ذكور", "إناث" },
            Values = new() { males, females }
        };
    }

    private static DistributionDto BuildHealthDistribution(
        List<Models.Entities.Visit> visits, List<Models.Entities.CssrsAssessment> cssrs,
        List<Models.Entities.PatientPersonalInfo> patients, DateTime today)
    {
        var improved = 0; var stable = 0; var worsened = 0;
        foreach (var p in patients)
        {
            var patientCssrs = cssrs.Where(c => c.PatientId == p.Id).OrderByDescending(c => c.CreatedAt).ToList();
            if (patientCssrs.Count >= 2)
            {
                var latest = patientCssrs[0].TotalScore;
                var previous = patientCssrs[1].TotalScore;
                if (latest < previous) improved++;
                else if (latest > previous) worsened++;
                else stable++;
            }
            else
            {
                // إذا كان هناك زيارة حديثة، اعتبره مستقراً
                var lastVisit = visits.Where(v => v.PatientId == p.Id).OrderByDescending(v => v.VisitDate).FirstOrDefault();
                if (lastVisit != null && (today - lastVisit.VisitDate).Days < 60) stable++;
                else worsened++;
            }
        }
        var total = improved + stable + worsened;
        if (total == 0) total = 1;
        return new DistributionDto
        {
            Labels = new() { "تحسن", "مستقر", "تراجع" },
            Values = new()
            {
                (int)Math.Round((double)improved / total * 100),
                (int)Math.Round((double)stable / total * 100),
                (int)Math.Round((double)worsened / total * 100)
            }
        };
    }

    private static AssessmentAveragesDto BuildAssessmentAverages(
        List<Models.Entities.Gad7Assessment> gad7,
        List<Models.Entities.Phq9Assessment> phq9,
        List<Models.Entities.SdsAssessment> sds)
    {
        var weeks = new List<string> { "أسبوع 1", "أسبوع 2", "أسبوع 3", "أسبوع 4", "أسبوع 5", "أسبوع 6" };
        var now = DateTime.UtcNow;
        var gad7Values = new List<double>();
        var phq9Values = new List<double>();
        var sdsValues = new List<double>();

        for (int i = 5; i >= 0; i--)
        {
            var start = now.AddDays(-i * 7 - 7);
            var end = now.AddDays(-i * 7);

            var gad7Week = gad7.Where(g => g.CreatedAt >= start && g.CreatedAt < end).ToList();
            var phq9Week = phq9.Where(p => p.CreatedAt >= start && p.CreatedAt < end).ToList();
            var sdsWeek = sds.Where(s => s.CreatedAt >= start && s.CreatedAt < end).ToList();

            gad7Values.Add(gad7Week.Any() ? Math.Round(gad7Week.Average(g => g.TotalScore), 1) : 0);
            phq9Values.Add(phq9Week.Any() ? Math.Round(phq9Week.Average(p => p.TotalScore), 1) : 0);
            sdsValues.Add(sdsWeek.Any() ? Math.Round(sdsWeek.Average(s => s.TotalScore), 1) : 0);
        }

        return new AssessmentAveragesDto
        {
            Labels = weeks,
            Gad7 = gad7Values,
            Phq9 = phq9Values,
            Sds = sdsValues
        };
    }

    private static List<TopDiagnosisDto> BuildTopDiagnoses(
        List<Models.Entities.Diagnosis> diagnoses, List<Models.Entities.PatientPersonalInfo> patients)
    {
        var total = patients.Count > 0 ? patients.Count : 1;
        return diagnoses
            .GroupBy(d => d.MainType)
            .OrderByDescending(g => g.Select(x => x.PatientId).Distinct().Count())
            .Take(5)
            .Select(g => new TopDiagnosisDto
            {
                Name = g.Key,
                Count = g.Select(x => x.PatientId).Distinct().Count(),
                Percent = (int)Math.Round((double)g.Select(x => x.PatientId).Distinct().Count() / total * 100)
            })
            .ToList();
    }

    private static List<TopPatientDto> BuildTopPatients(
        List<Models.Entities.Visit> visits, List<Models.Entities.PatientPersonalInfo> patients)
    {
        return visits
            .GroupBy(v => v.PatientId)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g =>
            {
                var patient = patients.FirstOrDefault(p => p.Id == g.Key);
                return new TopPatientDto
                {
                    Name = patient != null ? $"{patient.FirstName} {patient.LastName}" : "غير معروف",
                    Code = "-",
                    Visits = g.Count()
                };
            })
            .ToList();
    }

    private static List<ActivityDto> BuildRecentActivities(
        List<Models.Entities.PatientPersonalInfo> patients,
        List<Models.Entities.Visit> visits,
        List<Models.Entities.ListeningSession> sessions,
        List<Models.Entities.Diagnosis> diagnoses)
    {
        var activities = new List<ActivityDto>();

        foreach (var p in patients.OrderByDescending(p => p.CreatedAt).Take(2))
        {
            activities.Add(new ActivityDto
            {
                Title = $"إضافة مريض جديد: {p.FirstName} {p.LastName}",
                Time = FormatTimeAgo(p.CreatedAt),
                Type = "primary"
            });
        }
        foreach (var v in visits.OrderByDescending(v => v.VisitDate).Take(2))
        {
            activities.Add(new ActivityDto
            {
                Title = $"زيارة: {v.Reason}",
                Time = FormatTimeAgo(v.VisitDate),
                Type = "success"
            });
        }
        foreach (var s in sessions.OrderByDescending(s => s.SessionDate).Take(2))
        {
            activities.Add(new ActivityDto
            {
                Title = $"جلسة استماع: {s.Title}",
                Time = FormatTimeAgo(s.SessionDate),
                Type = "warning"
            });
        }
        foreach (var d in diagnoses.OrderByDescending(d => d.DiagnosisDate).Take(1))
        {
            activities.Add(new ActivityDto
            {
                Title = $"تشخيص جديد: {d.MainType}",
                Time = FormatTimeAgo(d.DiagnosisDate),
                Type = "info"
            });
        }

        return activities.OrderByDescending(a => a.Time).Take(6).ToList();
    }

    private static List<InsightDto> BuildInsights(
        ClinicStatisticsDto stats, List<Models.Entities.CssrsAssessment> cssrs,
        List<Models.Entities.Visit> visits, DateTime today)
    {
        var insights = new List<InsightDto>();

        // رؤية 1: نسبة الالتزام
        if (stats.AdherenceRate >= 70)
            insights.Add(new InsightDto
            {
                Type = "success",
                Title = "التزام علاجي مرتفع",
                Text = $"{stats.AdherenceRate}% من المرضى لديهم زيارات حديثة",
                Badge = "إيجابي"
            });

        // رؤية 2: مرضى يحتاجون متابعة
        var stalePatients = visits
            .GroupBy(v => v.PatientId)
            .Where(g => (today - g.Max(v => v.VisitDate)).Days > 45)
            .Count();
        if (stalePatients > 0)
            insights.Add(new InsightDto
            {
                Type = "warning",
                Title = "مرضى يحتاجون متابعة",
                Text = $"{stalePatients} مريض لم يزوروا العيادة منذ أكثر من 45 يوماً",
                Badge = "يحتاج إجراء"
            });

        // رؤية 3: حالات حرجة
        if (stats.CriticalPatients > 0)
            insights.Add(new InsightDto
            {
                Type = "danger",
                Title = "تنبيه: حالات حرجة",
                Text = $"{stats.CriticalPatients} حالة بتقييم C-SSRS مرتفع تحتاج مراجعة عاجلة",
                Badge = "عاجل"
            });

        // رؤية 4: ذروة الزيارات
        if (visits.Any())
        {
            var busiestDay = visits
                .GroupBy(v => v.VisitDate.DayOfWeek)
                .OrderByDescending(g => g.Count())
                .First();
            var dayNames = new[] { "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت" };
            insights.Add(new InsightDto
            {
                Type = "info",
                Title = "ذروة الزيارات",
                Text = $"أكثر يوم ازدحاماً: {dayNames[(int)busiestDay.Key]} ({busiestDay.Count()} زيارة)",
                Badge = "معلومة"
            });
        }

        return insights;
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    private static string FormatTimeAgo(DateTime date)
    {
        var diff = DateTime.UtcNow - date;
        if (diff.TotalSeconds < 60) return "الآن";
        if (diff.TotalMinutes < 60) return $"قبل {(int)diff.TotalMinutes} دقيقة";
        if (diff.TotalHours < 24) return $"قبل {(int)diff.TotalHours} ساعة";
        if (diff.TotalDays < 7) return $"قبل {(int)diff.TotalDays} يوم";
        return date.ToString("yyyy-MM-dd");
    }
}