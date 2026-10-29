using PatientService.Services.Common;
using PatientService.Services.DTOs.Clinic;

namespace PatientService.Services.Interfaces;

public interface IClinicService
{
    Task<ServiceResult<IEnumerable<PatientListDto>>> GetAllPatientsAsync(Guid doctorId);
    Task<ServiceResult<ClinicStatisticsDto>> GetStatisticsAsync(Guid doctorId);
}