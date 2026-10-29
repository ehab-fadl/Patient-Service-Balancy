using Microsoft.Extensions.Logging;
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

    public async Task<ServiceResult<IEnumerable<MetricDataDto>>> GetMetricsAsync(
        Guid patientId, string range, DateTime? startDate, DateTime? endDate, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<IEnumerable<MetricDataDto>>.FailureResult("غير مصرح");

        var today = DateTime.UtcNow.Date;
        var fromDate = today.AddDays(-7);
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
                break;
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
}