using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Alerts;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class AlertService : IAlertService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AlertService> _logger;

    public AlertService(IUnitOfWork unitOfWork, ILogger<AlertService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResult<AlertsSummaryDto>> GetActiveAlertsAsync(Guid doctorId)
    {
        var alerts = await _unitOfWork.SystemAlerts.GetActiveByDoctorIdAsync(doctorId);
        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();

        var result = new AlertsSummaryDto
        {
            Alerts = alerts.Select(a => MapToDto(a, patients, codes)).ToList(),
            LastUpdated = DateTime.UtcNow
        };

        result.CriticalCount = result.Alerts.Count(a => a.Level == "critical");
        result.WarningCount = result.Alerts.Count(a => a.Level == "warning");
        result.InfoCount = result.Alerts.Count(a => a.Level == "info");

        return ServiceResult<AlertsSummaryDto>.SuccessResult(result);
    }

    // ═══════════════════════════════════════════════════════════════
    // محرك التنبيهات: يعمل على كل المرضى ويولّد التنبيهات الجديدة
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<AlertsSummaryDto>> RefreshAlertsAsync(Guid doctorId)
    {
        var patients = (await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId)).ToList();
        var visits = (await _unitOfWork.Visits.FindAsync(v => v.DoctorId == doctorId)).ToList();
        var diagnoses = (await _unitOfWork.Diagnoses.FindAsync(d => d.DoctorId == doctorId)).ToList();
        var medications = (await _unitOfWork.Medications.FindAsync(m => m.DoctorId == doctorId)).ToList();
        var cssrs = (await _unitOfWork.CssrsAssessments.FindAsync(c => c.DoctorId == doctorId)).ToList();
        var phq9 = (await _unitOfWork.Phq9Assessments.FindAsync(p => p.DoctorId == doctorId)).ToList();

        var newAlerts = new List<SystemAlert>();
        var today = DateTime.UtcNow.Date;

        foreach (var patient in patients)
        {
            // ─────────────────────────────────────────────
            // 1. قاعدة: خطر انتحار مرتفع (C-SSRS >= 3)
            // ─────────────────────────────────────────────
            var latestCssrs = cssrs.Where(c => c.PatientId == patient.Id).OrderByDescending(c => c.CreatedAt).FirstOrDefault();
            if (latestCssrs != null && latestCssrs.TotalScore >= 3 && latestCssrs.CreatedAt >= today.AddDays(-30))
            {
                if (!await _unitOfWork.SystemAlerts.AlertExistsForTodayAsync(patient.Id, "cssrs-high"))
                {
                    newAlerts.Add(CreateAlert(patient, doctorId, "cssrs-high", "critical",
                        "🚨 خطر انتحار مرتفع",
                        $"المريض لديه تقييم C-SSRS = {latestCssrs.TotalScore}. يحتاج تدخل فوري.",
                        "مراجعة فورية"));
                }
            }

            // ─────────────────────────────────────────────
            // 2. قاعدة: تدهور سريع في PHQ-9 (زيادة >= 10 نقاط خلال شهر)
            // ─────────────────────────────────────────────
            var patientPhq9 = phq9.Where(p => p.PatientId == patient.Id).OrderByDescending(p => p.CreatedAt).ToList();
            if (patientPhq9.Count >= 2)
            {
                var latest = patientPhq9[0].TotalScore;
                var previous = patientPhq9[1].TotalScore;
                if (latest - previous >= 10)
                {
                    if (!await _unitOfWork.SystemAlerts.AlertExistsForTodayAsync(patient.Id, "rapid-deterioration"))
                    {
                        newAlerts.Add(CreateAlert(patient, doctorId, "rapid-deterioration", "critical",
                            "🚨 تدهور سريع في الحالة",
                            $"ارتفع مؤشر PHQ-9 بمقدار {latest - previous} نقطة. يحتاج مراجعة عاجلة.",
                            "مراجعة الخطة"));
                    }
                }
            }

            // ─────────────────────────────────────────────
            // 3. قاعدة: موعد اليوم
            // ─────────────────────────────────────────────
            var patientVisits = visits.Where(v => v.PatientId == patient.Id).ToList();
            var latestVisit = patientVisits.OrderByDescending(v => v.VisitDate).FirstOrDefault();
            if (latestVisit?.NextVisitDate?.Date == today)
            {
                if (!await _unitOfWork.SystemAlerts.AlertExistsForTodayAsync(patient.Id, "appointment-today"))
                {
                    newAlerts.Add(CreateAlert(patient, doctorId, "appointment-today", "warning",
                        "📅 موعد اليوم",
                        "موعد المريض اليوم. تأكد من تأكيد الحضور.",
                        "عرض الموعد"));
                }
            }

            // ─────────────────────────────────────────────
            // 4. قاعدة: مريض متأخر عن المتابعة (> 45 يوم)
            // ─────────────────────────────────────────────
            if (latestVisit != null && (today - latestVisit.VisitDate.Date).Days > 45)
            {
                if (!await _unitOfWork.SystemAlerts.AlertExistsForTodayAsync(patient.Id, "overdue-visit"))
                {
                    newAlerts.Add(CreateAlert(patient, doctorId, "overdue-visit", "warning",
                        "⚠️ مريض متأخر عن المتابعة",
                        $"آخر زيارة منذ {(today - latestVisit.VisitDate.Date).Days} يوم. يُنصح بالتواصل.",
                        "اتصال"));
                }
            }

            // ─────────────────────────────────────────────
            // 5. قاعدة: لا يوجد دواء نشط
            // ─────────────────────────────────────────────
            var activeMeds = medications.Count(m => m.PatientId == patient.Id && m.Status == "نشط");
            if (activeMeds == 0 && diagnoses.Any(d => d.PatientId == patient.Id))
            {
                if (!await _unitOfWork.SystemAlerts.AlertExistsForTodayAsync(patient.Id, "no-medications"))
                {
                    newAlerts.Add(CreateAlert(patient, doctorId, "no-medications", "warning",
                        "⚠️ لا يوجد دواء نشط",
                        "المريض لديه تشخيص لكن لا يتناول أي أدوية حالياً. تأكد من مراجعة الخطة.",
                        "مراجعة الأدوية"));
                }
            }

            // ─────────────────────────────────────────────
            // 6. قاعدة: عيد ميلاد المريض
            // ─────────────────────────────────────────────
            if (patient.DateOfBirth.Day == today.Day && patient.DateOfBirth.Month == today.Month)
            {
                if (!await _unitOfWork.SystemAlerts.AlertExistsForTodayAsync(patient.Id, "birthday"))
                {
                    newAlerts.Add(CreateAlert(patient, doctorId, "birthday", "info",
                        "🎂 عيد ميلاد المريض",
                        $"اليوم عيد ميلاد {patient.FirstName} {patient.LastName}. يمكنك إرسال تهنئة.",
                        "إرسال تهنئة"));
                }
            }
        }

        // ─────────────────────────────────────────────
        // حفظ كل التنبيهات الجديدة دفعة واحدة
        // ─────────────────────────────────────────────
        foreach (var alert in newAlerts)
        {
            await _unitOfWork.SystemAlerts.AddAsync(alert);
        }

        _logger.LogInformation("Generated {Count} new alerts for doctor {DoctorId}", newAlerts.Count, doctorId);

        return await GetActiveAlertsAsync(doctorId);
    }

    public async Task<ServiceResult<bool>> DismissAlertAsync(Guid alertId, Guid doctorId)
    {
        var alert = await _unitOfWork.SystemAlerts.GetByIdAndDoctorAsync(alertId, doctorId);
        if (alert is null) return ServiceResult<bool>.NotFoundResult("التنبيه غير موجود");

        alert.IsDismissed = true;
        alert.DismissedAt = DateTime.UtcNow;
        await _unitOfWork.SystemAlerts.UpdateAsync(alert);

        return ServiceResult<bool>.SuccessResult(true, "تم تجاهل التنبيه");
    }

    public async Task<ServiceResult<bool>> DismissAllAlertsAsync(Guid doctorId)
    {
        var alerts = await _unitOfWork.SystemAlerts.GetActiveByDoctorIdAsync(doctorId);
        foreach (var alert in alerts)
        {
            alert.IsDismissed = true;
            alert.DismissedAt = DateTime.UtcNow;
            await _unitOfWork.SystemAlerts.UpdateAsync(alert);
        }
        return ServiceResult<bool>.SuccessResult(true, $"تم تجاهل {alerts.Count()} تنبيه");
    }

    // ─────────────────────────────────────────────
    // Mapper
    // ─────────────────────────────────────────────
    private static SystemAlert CreateAlert(
        PatientPersonalInfo patient, Guid doctorId, string ruleId, string level,
        string title, string message, string actionLabel)
    {
        return new SystemAlert
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            DoctorId = doctorId,
            RuleId = ruleId,
            Level = level,
            Title = title,
            Message = message,
            ActionLabel = actionLabel,
            IsDismissed = false
        };
    }

    private static AlertDto MapToDto(SystemAlert alert, IEnumerable<PatientPersonalInfo> patients, IEnumerable<PatientCode> codes)
    {
        var patient = patients.FirstOrDefault(p => p.Id == alert.PatientId);
        var code = codes.FirstOrDefault(c => c.PatientId == alert.PatientId);

        return new AlertDto
        {
            Id = alert.Id,
            RuleId = alert.RuleId,
            Level = alert.Level,
            Title = alert.Title,
            Message = alert.Message,
            ActionLabel = alert.ActionLabel,
            PatientId = alert.PatientId,
            PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "غير معروف",
            PatientCode = code?.Code ?? "—",
            IsDismissed = alert.IsDismissed,
            CreatedAt = alert.CreatedAt
        };
    }
}