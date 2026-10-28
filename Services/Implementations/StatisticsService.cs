using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Statistics;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(IUnitOfWork unitOfWork, ILogger<StatisticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // 1. تطبيق الطبيب: جلب الإحصائيات
    public async Task<ServiceResult<IEnumerable<MetricDataDto>>> GetMetricsAsync(Guid patientId, string range, DateTime? startDate, DateTime? endDate, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<IEnumerable<MetricDataDto>>.FailureResult("غير مصرح لك بالوصول إلى بيانات هذا المريض");

        var today = DateTime.UtcNow.Date;
        var fromDate = today;
        var toDate = today;

        switch (range)
        {
            case "week": fromDate = today.AddDays(-7); break;
            case "month": fromDate = today.AddDays(-30); break;
            case "threeMonths": fromDate = today.AddDays(-90); break;
            case "sixMonths": fromDate = today.AddDays(-180); break;
            case "custom":
                if (startDate.HasValue && endDate.HasValue)
                {
                    fromDate = startDate.Value.Date;
                    toDate = endDate.Value.Date;
                }
                else fromDate = today.AddDays(-7);
                break;
            default: fromDate = today.AddDays(-7); break;
        }

        var metrics = await _unitOfWork.DailyPatientMetrics.GetByPatientAndDateRangeAsync(patientId, fromDate, toDate);

        var result = metrics.Select(m => new MetricDataDto
        {
            Date = m.LogDate.ToString("yyyy-MM-dd"),
            Food = m.Food,
            Sleep = m.SleepHours,
            Medication = m.MedicationAdherence,
            Mood = m.Mood,
            Negative = m.NegativeThoughts,
            Health = m.PhysicalHealth,
            Energy = m.Energy,
            Motivation = m.Motivation,
            Concentration = m.Concentration,
            Guilt = m.Guilt,
            Hope = m.Hope,
            Suicidal = m.SuicidalIdeation
        });

        return ServiceResult<IEnumerable<MetricDataDto>>.SuccessResult(result);
    }

    // 2. تطبيق الموبايل: إدخال قراءة يومية
    public async Task<ServiceResult<MetricDataDto>> SubmitDailyMetricAsync(string patientCode, DailyMetricSubmitDto dto)
    {
        // البحث عن المريض باستخدام الكود
        var codeEntity = await _unitOfWork.PatientCodes.GetByCodeAsync(patientCode);
        if (codeEntity is null || !codeEntity.IsActive)
            return ServiceResult<MetricDataDto>.NotFoundResult("كود المريض غير صالح أو غير نشط");

        var patientId = codeEntity.PatientId;
        var logDate = dto.LogDate.Date;

        // التحقق: لا يمكن تسجيل قراءتين لنفس اليوم
        var existing = await _unitOfWork.DailyPatientMetrics.GetByPatientAndDateAsync(patientId, logDate);
        if (existing is not null)
            return ServiceResult<MetricDataDto>.FailureResult("لقد قمت بإدخال قراءات هذا اليوم بالفعل. يمكنك تعديلها بدلاً من إضافتها.");

        var entity = new DailyPatientMetric
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
        };

        await _unitOfWork.DailyPatientMetrics.AddAsync(entity);

        // التحقق من الخطر (بشكل تلقائي)
        if (entity.SuicidalIdeation >= 3)
        {
            _logger.LogWarning("HIGH SUICIDAL IDEATION reported by patient {PatientId} on {Date}", patientId, logDate);
        }

        return ServiceResult<MetricDataDto>.SuccessResult(new MetricDataDto
        {
            Date = entity.LogDate.ToString("yyyy-MM-dd"),
            Food = entity.Food,
            Sleep = entity.SleepHours,
            Medication = entity.MedicationAdherence,
            Mood = entity.Mood,
            Negative = entity.NegativeThoughts,
            Health = entity.PhysicalHealth,
            Energy = entity.Energy,
            Motivation = entity.Motivation,
            Concentration = entity.Concentration,
            Guilt = entity.Guilt,
            Hope = entity.Hope,
            Suicidal = entity.SuicidalIdeation
        }, "تم تسجيل قراءاتك اليومية بنجاح");
    }
}