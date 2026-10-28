using PatientService.Services.Common;
using PatientService.Services.DTOs.Statistics;

namespace PatientService.Services.Interfaces;

public interface IStatisticsService
{
    // للطبيب
    Task<ServiceResult<IEnumerable<MetricDataDto>>> GetMetricsAsync(Guid patientId, string range, DateTime? startDate, DateTime? endDate, Guid doctorId);

    // للموبايل (عبر كود المريض)
    Task<ServiceResult<MetricDataDto>> SubmitDailyMetricAsync(string patientCode, DailyMetricSubmitDto dto);
}