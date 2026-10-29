using Microsoft.Extensions.Logging;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Dashboard;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResult<DashboardDto>> GetDashboardAsync(Guid doctorId)
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var monthAgo = today.AddDays(-30);
        var twoMonthsAgo = today.AddDays(-60);

        // ─── جلب كل البيانات (Bulk) لتجنب N+1 ───
        var patients = (await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId)).ToList();
        var appointments = (await _unitOfWork.Appointments.GetByDoctorIdAsync(doctorId)).ToList();
        var alerts = (await _unitOfWork.SystemAlerts.GetActiveByDoctorIdAsync(doctorId)).ToList();
        var visits = (await _unitOfWork.Visits.FindAsync(v => v.DoctorId == doctorId)).ToList();
        var diagnoses = (await _unitOfWork.Diagnoses.FindAsync(d => d.DoctorId == doctorId)).ToList();
        var cssrs = (await _unitOfWork.CssrsAssessments.FindAsync(c => c.DoctorId == doctorId)).ToList();
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();

        // ─── 1. الترحيب ───
        var dto = new DashboardDto
        {
            DoctorName = "الطبيب",
            TodayDate = today,
            GreetingText = GetGreeting(DateTime.UtcNow.Hour)
        };

        // ─── 2. Quick Stats ───
        var todayApts = appointments.Where(a => a.ScheduledDate.Date == today).ToList();
        var yesterdayApts = appointments.Where(a => a.ScheduledDate.Date == yesterday).ToList();
        dto.TodayAppointmentsCount = todayApts.Count;
        dto.TodayAppointmentsChangeFromYesterday = todayApts.Count - yesterdayApts.Count;

        dto.TotalPatients = patients.Count;
        var patientsLastMonth = patients.Count(p => p.CreatedAt < monthAgo);
        dto.TotalPatientsChangePercent = patientsLastMonth > 0
            ? (int)Math.Round((double)(patients.Count - patientsLastMonth) / patientsLastMonth * 100)
            : 0;

        // مرضى حرجون (آخر C-SSRS)
        var criticalPatientIds = cssrs
            .GroupBy(c => c.PatientId)
            .Where(g => {
                var latest = g.OrderByDescending(c => c.CreatedAt).First();
                return latest.IsCritical || latest.Severity == "high" || latest.Severity == "critical";
            })
            .Select(g => g.Key)
            .Distinct()
            .ToList();
        dto.CriticalPatients = criticalPatientIds.Count;

        // مرضى جدد هذا الشهر
        dto.NewPatientsThisMonth = patients.Count(p => p.CreatedAt >= monthAgo);
        var newLastMonth = patients.Count(p => p.CreatedAt >= twoMonthsAgo && p.CreatedAt < monthAgo);
        dto.NewPatientsChangeFromLastMonth = dto.NewPatientsThisMonth - newLastMonth;

        // ─── 3. Today's Appointments ───
        dto.TodayAppointments = todayApts
            .OrderBy(a => a.ScheduledTime)
            .Select(a => {
                var patient = patients.FirstOrDefault(p => p.Id == a.PatientId);
                return new DashboardAppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "غير معروف",
                    PatientInitials = patient != null ? $"{patient.FirstName?.FirstOrDefault()}{patient.LastName?.FirstOrDefault()}" : "؟",
                    ScheduledTime = a.ScheduledTime,
                    Reason = a.Reason,
                    Status = a.Status
                };
            })
            .ToList();

        // ─── 4. Active Alerts ───
        dto.TotalActiveAlerts = alerts.Count;
        dto.ActiveAlerts = alerts
            .OrderBy(a => a.Level == "critical" ? 0 : a.Level == "warning" ? 1 : 2)
            .ThenByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => {
                var patient = patients.FirstOrDefault(p => p.Id == a.PatientId);
                return new DashboardAlertDto
                {
                    Id = a.Id,
                    Level = a.Level,
                    Title = a.Title,
                    Message = a.Message,
                    PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "غير معروف",
                    CreatedAt = a.CreatedAt
                };
            })
            .ToList();

        // ─── 5. Recent Patients ───
        dto.RecentPatients = patients
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => {
                var code = codes.FirstOrDefault(c => c.PatientId == p.Id);
                var latestDiagnosis = diagnoses
                    .Where(d => d.PatientId == p.Id)
                    .OrderByDescending(d => d.DiagnosisDate)
                    .FirstOrDefault()?.MainType;

                return new DashboardPatientDto
                {
                    Id = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}",
                    PatientCode = code?.Code ?? "—",
                    Initials = $"{p.FirstName?.FirstOrDefault()}{p.LastName?.FirstOrDefault()}",
                    CreatedAt = p.CreatedAt,
                    LastDiagnosis = latestDiagnosis
                };
            })
            .ToList();

        _logger.LogInformation("Dashboard generated for doctor {DoctorId}", doctorId);
        return ServiceResult<DashboardDto>.SuccessResult(dto);
    }

    private static string GetGreeting(int hour)
    {
        if (hour < 12) return "صباح الخير، أتمنى لك يوماً موفقاً";
        if (hour < 17) return "مساء الخير، أتمنى لك يوماً موفقاً";
        return "مساء الخير، أتمنى لك ليلة هادئة";
    }
}