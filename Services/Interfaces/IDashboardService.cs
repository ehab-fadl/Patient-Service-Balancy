using PatientService.Services.Common;
using PatientService.Services.DTOs.Dashboard;

namespace PatientService.Services.Interfaces;

public interface IDashboardService
{
    Task<ServiceResult<DashboardDto>> GetDashboardAsync(Guid doctorId);
}