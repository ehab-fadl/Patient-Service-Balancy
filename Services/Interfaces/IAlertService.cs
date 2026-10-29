using PatientService.Services.Common;
using PatientService.Services.DTOs.Alerts;

namespace PatientService.Services.Interfaces;

public interface IAlertService
{
    Task<ServiceResult<AlertsSummaryDto>> GetActiveAlertsAsync(Guid doctorId);
    Task<ServiceResult<AlertsSummaryDto>> RefreshAlertsAsync(Guid doctorId);
    Task<ServiceResult<bool>> DismissAlertAsync(Guid alertId, Guid doctorId);
    Task<ServiceResult<bool>> DismissAllAlertsAsync(Guid doctorId);
}