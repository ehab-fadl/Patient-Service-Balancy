using PatientService.Services.Common;
using PatientService.Services.DTOs.Statistics;

namespace PatientService.Services.Interfaces;

public interface IStatisticsService
{
   
    Task<ServiceResult<IEnumerable<MetricDataDto>>> GetMetricsAsync(
        Guid patientId,
        string range,
        DateTime? startDate,
        DateTime? endDate,
        Guid doctorId);
}