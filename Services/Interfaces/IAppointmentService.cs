using PatientService.Services.Common;
using PatientService.Services.DTOs.Appointments;

namespace PatientService.Services.Interfaces;

public interface IAppointmentService
{
    Task<ServiceResult<AppointmentsSummaryDto>> GetAppointmentsAsync(Guid doctorId);
    Task<ServiceResult<AppointmentDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<AppointmentDto>> CreateAppointmentAsync(AppointmentCreateDto dto, Guid doctorId);
    Task<ServiceResult<AppointmentDto>> UpdateAppointmentAsync(AppointmentUpdateDto dto, Guid doctorId);
    Task<ServiceResult<AppointmentDto>> ConfirmAppointmentAsync(Guid id, Guid doctorId);
    Task<ServiceResult<AppointmentDto>> CancelAppointmentAsync(AppointmentCancelDto dto, Guid doctorId);
    Task<ServiceResult<AppointmentDto>> MarkNoShowAsync(Guid id, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAppointmentAsync(Guid id, Guid doctorId);
}