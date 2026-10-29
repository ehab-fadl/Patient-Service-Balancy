using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Mobile;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class MobileService : IMobileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MobileService> _logger;

    public MobileService(IUnitOfWork unitOfWork, ILogger<MobileService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════════════════
    // 1. الملف الشخصي
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<MobilePatientProfileDto>> GetProfileAsync(Guid patientId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<MobilePatientProfileDto>.NotFoundResult("المريض غير موجود");

        var code = await _unitOfWork.PatientCodes.GetByPatientIdAsync(patientId);
        var diagnoses = await _unitOfWork.Diagnoses.GetByPatientIdAsync(patientId);
        var medications = await _unitOfWork.Medications.GetByPatientIdAsync(patientId);
        var visits = await _unitOfWork.Visits.GetByPatientIdAsync(patientId);
        var plans = await _unitOfWork.TreatmentPlans.GetFullTreeByPatientIdAsync(patientId);
        var cssrs = await _unitOfWork.CssrsAssessments.GetByPatientIdAsync(patientId);
        var appointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(patient.DoctorId);

        var latestDiagnosis = diagnoses.OrderByDescending(d => d.DiagnosisDate).FirstOrDefault()?.MainType;
        var activePlan = plans.FirstOrDefault(p => p.Status == "نشطة");
        var activeMeds = medications.Count(m => m.Status == "نشط");
        var latestVisit = visits.OrderByDescending(v => v.VisitDate).FirstOrDefault();
        var nextAppointment = appointments
            .Where(a => a.PatientId == patientId && a.ScheduledDate >= DateTime.UtcNow.Date && a.Status != "Cancelled")
            .OrderBy(a => a.ScheduledDate)
            .FirstOrDefault();
        var latestCssrs = cssrs.OrderByDescending(c => c.CreatedAt).FirstOrDefault();

        var age = DateTime.UtcNow.Date.Year - patient.DateOfBirth.Year;
        if (patient.DateOfBirth.Date > DateTime.UtcNow.Date.AddYears(-age)) age--;

        return ServiceResult<MobilePatientProfileDto>.SuccessResult(new MobilePatientProfileDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            PatientCode = code?.Code ?? "",
            Age = age,
            Gender = patient.Gender,
            Phone = patient.Phone,
            Email = patient.Email,
            CurrentDiagnosis = latestDiagnosis,
            ActiveTreatmentPlanName = activePlan?.Name,
            ActiveMedicationsCount = activeMeds,
            TotalVisits = visits.Count(),
            LastVisitDate = latestVisit?.VisitDate,
            NextAppointmentDate = nextAppointment?.ScheduledDate,
            RiskLevel = latestCssrs?.Severity ?? "low"
        });
    }

    // ═══════════════════════════════════════════════════════════════
    // 2. القراءات اليومية
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<MobileDailyMetricDto>> SubmitDailyMetricAsync(Guid patientId, MobileDailyMetricDto dto)
    {
        var logDate = dto.LogDate.Date;
        var existing = await _unitOfWork.DailyPatientMetrics.GetByPatientAndDateAsync(patientId, logDate);

        if (existing is not null)
        {
            existing.Food = dto.Food;
            existing.SleepHours = dto.SleepHours;
            existing.MedicationAdherence = dto.MedicationAdherence;
            existing.Mood = dto.Mood;
            existing.NegativeThoughts = dto.NegativeThoughts;
            existing.PhysicalHealth = dto.PhysicalHealth;
            existing.Energy = dto.Energy;
            existing.Motivation = dto.Motivation;
            existing.Concentration = dto.Concentration;
            existing.Guilt = dto.Guilt;
            existing.Hope = dto.Hope;
            existing.SuicidalIdeation = dto.SuicidalIdeation;

            await _unitOfWork.DailyPatientMetrics.UpdateAsync(existing);
        }
        else
        {
            await _unitOfWork.DailyPatientMetrics.AddAsync(new DailyPatientMetric
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                LogDate = logDate,
                Food = dto.Food,
                SleepHours = dto.SleepHours,
                MedicationAdherence = dto.MedicationAdherence,
                Mood = dto.Mood,
                NegativeThoughts = dto.NegativeThoughts,
                PhysicalHealth = dto.PhysicalHealth,
                Energy = dto.Energy,
                Motivation = dto.Motivation,
                Concentration = dto.Concentration,
                Guilt = dto.Guilt,
                Hope = dto.Hope,
                SuicidalIdeation = dto.SuicidalIdeation
            });
        }

        if (dto.SuicidalIdeation >= 3)
            _logger.LogWarning("HIGH SUICIDAL IDEATION from patient {PatientId} on {Date}", patientId, logDate);

        return ServiceResult<MobileDailyMetricDto>.SuccessResult(dto, "تم حفظ قراءاتك اليومية بنجاح");
    }

    public async Task<ServiceResult<MobileDailyMetricDto?>> GetTodayMetricAsync(Guid patientId)
    {
        var metric = await _unitOfWork.DailyPatientMetrics.GetByPatientAndDateAsync(patientId, DateTime.UtcNow.Date);
        if (metric is null)
            return ServiceResult<MobileDailyMetricDto?>.SuccessResult(null, "لم يتم تسجيل قراءات اليوم بعد");

        return ServiceResult<MobileDailyMetricDto?>.SuccessResult(new MobileDailyMetricDto
        {
            LogDate = metric.LogDate,
            Food = metric.Food,
            SleepHours = metric.SleepHours,
            MedicationAdherence = metric.MedicationAdherence,
            Mood = metric.Mood,
            NegativeThoughts = metric.NegativeThoughts,
            PhysicalHealth = metric.PhysicalHealth,
            Energy = metric.Energy,
            Motivation = metric.Motivation,
            Concentration = metric.Concentration,
            Guilt = metric.Guilt,
            Hope = metric.Hope,
            SuicidalIdeation = metric.SuicidalIdeation
        });
    }

    // ═══════════════════════════════════════════════════════════════
    // 3. التقييمات النفسية
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<MobileAssessmentHistoryDto>> SubmitAssessmentAsync(Guid patientId, MobileAssessmentSubmitDto dto)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<MobileAssessmentHistoryDto>.NotFoundResult("المريض غير موجود");

        switch (dto.AssessmentType.ToUpperInvariant())
        {
            case "GAD7":
                await _unitOfWork.Gad7Assessments.AddAsync(new Gad7Assessment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    DoctorId = patient.DoctorId,
                    TotalScore = dto.TotalScore,
                    Severity = dto.Severity,
                    AnswersJson = dto.AnswersJson
                });
                break;
            case "PHQ9":
                await _unitOfWork.Phq9Assessments.AddAsync(new Phq9Assessment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    DoctorId = patient.DoctorId,
                    TotalScore = dto.TotalScore,
                    Severity = dto.Severity,
                    AnswersJson = dto.AnswersJson
                });
                break;
            case "MDQ":
                await _unitOfWork.MdqAssessments.AddAsync(new MdqAssessment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    DoctorId = patient.DoctorId,
                    TotalScore = dto.TotalScore,
                    Severity = dto.Severity,
                    AnswersJson = dto.AnswersJson
                });
                break;
            case "SDS":
                await _unitOfWork.SdsAssessments.AddAsync(new SdsAssessment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    DoctorId = patient.DoctorId,
                    TotalScore = dto.TotalScore,
                    Severity = dto.Severity,
                    AnswersJson = dto.AnswersJson
                });
                break;
            case "CSSRS":
                await _unitOfWork.CssrsAssessments.AddAsync(new CssrsAssessment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    DoctorId = patient.DoctorId,
                    TotalScore = dto.TotalScore,
                    Severity = dto.Severity,
                    AnswersJson = dto.AnswersJson,
                    IsCritical = dto.TotalScore >= 3
                });
                break;
            default:
                return ServiceResult<MobileAssessmentHistoryDto>.FailureResult("نوع التقييم غير معروف");
        }

        return ServiceResult<MobileAssessmentHistoryDto>.SuccessResult(new MobileAssessmentHistoryDto
        {
            AssessmentType = dto.AssessmentType,
            TotalScore = dto.TotalScore,
            Severity = dto.Severity,
            CreatedAt = DateTime.UtcNow
        }, "تم حفظ نتيجة التقييم بنجاح");
    }

    public async Task<ServiceResult<IEnumerable<MobileAssessmentHistoryDto>>> GetAssessmentHistoryAsync(Guid patientId)
    {
        var history = new List<MobileAssessmentHistoryDto>();

        var gad7 = await _unitOfWork.Gad7Assessments.GetByPatientIdAsync(patientId);
        history.AddRange(gad7.Select(a => new MobileAssessmentHistoryDto
        {
            AssessmentType = "GAD7",
            TotalScore = a.TotalScore,
            Severity = a.Severity,
            CreatedAt = a.CreatedAt
        }));

        var phq9 = await _unitOfWork.Phq9Assessments.GetByPatientIdAsync(patientId);
        history.AddRange(phq9.Select(a => new MobileAssessmentHistoryDto
        {
            AssessmentType = "PHQ9",
            TotalScore = a.TotalScore,
            Severity = a.Severity,
            CreatedAt = a.CreatedAt
        }));

        var mdq = await _unitOfWork.MdqAssessments.GetByPatientIdAsync(patientId);
        history.AddRange(mdq.Select(a => new MobileAssessmentHistoryDto
        {
            AssessmentType = "MDQ",
            TotalScore = a.TotalScore,
            Severity = a.Severity,
            CreatedAt = a.CreatedAt
        }));

        var sds = await _unitOfWork.SdsAssessments.GetByPatientIdAsync(patientId);
        history.AddRange(sds.Select(a => new MobileAssessmentHistoryDto
        {
            AssessmentType = "SDS",
            TotalScore = a.TotalScore,
            Severity = a.Severity,
            CreatedAt = a.CreatedAt
        }));

        var cssrs = await _unitOfWork.CssrsAssessments.GetByPatientIdAsync(patientId);
        history.AddRange(cssrs.Select(a => new MobileAssessmentHistoryDto
        {
            AssessmentType = "CSSRS",
            TotalScore = a.TotalScore,
            Severity = a.Severity,
            CreatedAt = a.CreatedAt
        }));

        return ServiceResult<IEnumerable<MobileAssessmentHistoryDto>>.SuccessResult(
            history.OrderByDescending(h => h.CreatedAt));
    }

    // ═══════════════════════════════════════════════════════════════
    // 4. الخطة العلاجية
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<MobileTreatmentPlanDto?>> GetActiveTreatmentPlanAsync(Guid patientId)
    {
        var plans = await _unitOfWork.TreatmentPlans.GetFullTreeByPatientIdAsync(patientId);
        var activePlan = plans.FirstOrDefault(p => p.Status == "نشطة");
        if (activePlan is null)
            return ServiceResult<MobileTreatmentPlanDto?>.SuccessResult(null, "لا توجد خطة علاجية نشطة");

        return ServiceResult<MobileTreatmentPlanDto?>.SuccessResult(new MobileTreatmentPlanDto
        {
            Id = activePlan.Id,
            Name = activePlan.Name,
            StartDate = activePlan.StartDate,
            EndDate = activePlan.EndDate,
            Status = activePlan.Status,
            Notes = activePlan.Notes,
            Problems = activePlan.Problems.Select(p => new MobileProblemDto
            {
                Id = p.Id,
                Text = p.Text,
                Goals = p.Goals.Select(g => new MobileGoalDto
                {
                    Id = g.Id,
                    Text = g.Text,
                    Unit = g.Unit,
                    Baseline = g.Baseline,
                    TargetValue = g.TargetValue,
                    CurrentValue = g.CurrentValue,
                    Status = g.Status,
                    PatientConfirmedByDoctor = g.PatientConfirmedByDoctor,
                    Means = g.Means.Select(m => new MobileMeanDto { Id = m.Id, Text = m.Text }).ToList()
                }).ToList()
            }).ToList()
        });
    }

    public async Task<ServiceResult<bool>> UpdateGoalProgressAsync(Guid patientId, Guid goalId, decimal value, string? note)
    {
        var goal = await _unitOfWork.TreatmentGoals.GetByIdAsync(goalId);
        if (goal is null) return ServiceResult<bool>.NotFoundResult("الهدف غير موجود");

        var problem = await _unitOfWork.TreatmentProblems.GetByIdAsync(goal.TreatmentProblemId);
        if (problem is null) return ServiceResult<bool>.NotFoundResult("المشكلة غير موجودة");

        var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(problem.TreatmentPlanId);
        if (plan is null || plan.PatientId != patientId)
            return ServiceResult<bool>.FailureResult("غير مصرح");

        goal.CurrentValue = value;
        await _unitOfWork.TreatmentGoals.UpdateAsync(goal);

        await _unitOfWork.TreatmentGoalHistories.AddAsync(new TreatmentGoalHistory
        {
            Id = Guid.NewGuid(),
            TreatmentGoalId = goalId,
            RecordedAt = DateTime.UtcNow,
            Value = value.ToString(),
            Note = note
        });

        return ServiceResult<bool>.SuccessResult(true, "تم تحديث التقدم بنجاح");
    }

    // ═══════════════════════════════════════════════════════════════
    // 5. الأدوية والأعراض الجانبية
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<IEnumerable<MobileMedicationDto>>> GetMedicationsAsync(Guid patientId)
    {
        var meds = await _unitOfWork.Medications.GetByPatientIdAsync(patientId);
        var result = meds
            .Where(m => m.Status == "نشط")
            .OrderByDescending(m => m.StartDate)
            .Select(m => new MobileMedicationDto
            {
                Id = m.Id,
                Name = m.Name,
                Dose = m.Dose,
                Frequency = m.Frequency,
                IntakePeriod = m.IntakePeriod,
                Status = m.Status,
                StartDate = m.StartDate,
                EndDate = m.EndDate
            });

        return ServiceResult<IEnumerable<MobileMedicationDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<bool>> SubmitSideEffectAsync(Guid patientId, MobileSideEffectSubmitDto dto)
    {
        var medication = await _unitOfWork.Medications.GetByIdAsync(dto.MedicationId);
        if (medication is null || medication.PatientId != patientId)
            return ServiceResult<bool>.NotFoundResult("الدواء غير موجود أو غير مصرح");

        await _unitOfWork.SideEffects.AddAsync(new SideEffect
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = medication.DoctorId,
            MedicationName = medication.Name,
            Symptom = dto.Symptom,
            Notes = dto.Notes
        });

        _logger.LogInformation("Side effect reported by patient {PatientId} for medication {MedId}", patientId, dto.MedicationId);
        return ServiceResult<bool>.SuccessResult(true, "تم تسجيل العرض الجانبي. سيراجعه الطبيب قريباً.");
    }

    // ═══════════════════════════════════════════════════════════════
    // 6. المواعيد
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<IEnumerable<MobileAppointmentDto>>> GetAppointmentsAsync(Guid patientId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<MobileAppointmentDto>>.NotFoundResult("المريض غير موجود");

        var allAppointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(patient.DoctorId);
        var patientAppointments = allAppointments
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.ScheduledDate);

        return ServiceResult<IEnumerable<MobileAppointmentDto>>.SuccessResult(
            patientAppointments.Select(a => new MobileAppointmentDto
            {
                Id = a.Id,
                ScheduledDate = a.ScheduledDate,
                ScheduledTime = a.ScheduledTime,
                Reason = a.Reason,
                Notes = a.Notes,
                Status = a.Status,
                VisitLocation = a.VisitLocation
            }));
    }

    public async Task<ServiceResult<bool>> CancelAppointmentAsync(Guid patientId, MobileAppointmentCancelDto dto)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId);
        if (appointment is null || appointment.PatientId != patientId)
            return ServiceResult<bool>.NotFoundResult("الموعد غير موجود أو غير مصرح");

        if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
            return ServiceResult<bool>.FailureResult("لا يمكن إلغاء هذا الموعد");

        appointment.Status = "Cancelled";
        appointment.CancellationReason = dto.CancellationReason;
        appointment.CancelledAt = DateTime.UtcNow;
        await _unitOfWork.Appointments.UpdateAsync(appointment);

        return ServiceResult<bool>.SuccessResult(true, "تم إلغاء الموعد");
    }

    public async Task<ServiceResult<bool>> RequestRescheduleAsync(Guid patientId, Guid appointmentId, DateTime newDate, string? newTime)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
        if (appointment is null || appointment.PatientId != patientId)
            return ServiceResult<bool>.NotFoundResult("الموعد غير موجود أو غير مصرح");

        if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
            return ServiceResult<bool>.FailureResult("لا يمكن تعديل هذا الموعد");

        appointment.Notes = $"{appointment.Notes}\n[طلب المريض إعادة جدولة إلى {newDate:yyyy-MM-dd} {newTime}]".Trim();
        appointment.Status = "Pending";
        await _unitOfWork.Appointments.UpdateAsync(appointment);

        return ServiceResult<bool>.SuccessResult(true, "تم إرسال طلب إعادة الجدولة للطبيب");
    }

    // ═══════════════════════════════════════════════════════════════
    // 7. التنبيهات
    // ═══════════════════════════════════════════════════════════════
    public async Task<ServiceResult<IEnumerable<MobileAlertDto>>> GetAlertsAsync(Guid patientId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<MobileAlertDto>>.NotFoundResult("المريض غير موجود");

        var allAlerts = await _unitOfWork.SystemAlerts.GetActiveByDoctorIdAsync(patient.DoctorId);
        var patientAlerts = allAlerts
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.Level == "critical" ? 0 : a.Level == "warning" ? 1 : 2)
            .ThenByDescending(a => a.CreatedAt);

        return ServiceResult<IEnumerable<MobileAlertDto>>.SuccessResult(
            patientAlerts.Select(a => new MobileAlertDto
            {
                Id = a.Id,
                Level = a.Level,
                Title = a.Title,
                Message = a.Message,
                CreatedAt = a.CreatedAt
            }));
    }

    public async Task<ServiceResult<bool>> DismissAlertAsync(Guid patientId, Guid alertId)
    {
        var alert = await _unitOfWork.SystemAlerts.GetByIdAsync(alertId);
        if (alert is null || alert.PatientId != patientId)
            return ServiceResult<bool>.NotFoundResult("التنبيه غير موجود أو غير مصرح");

        alert.IsDismissed = true;
        alert.DismissedAt = DateTime.UtcNow;
        await _unitOfWork.SystemAlerts.UpdateAsync(alert);

        return ServiceResult<bool>.SuccessResult(true, "تم تجاهل التنبيه");
    }
}