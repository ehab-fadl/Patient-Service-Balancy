using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Mobile;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/appointments")]
public class MobileAppointmentsController : MobileBaseController
{
    private readonly IMobileService _service;

    public MobileAppointmentsController(IMobileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments()
    {
        var patientId = GetPatientId();
        var result = await _service.GetAppointmentsAsync(patientId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id, [FromBody] MobileAppointmentCancelDto dto)
    {
        var patientId = GetPatientId();
        if (id != dto.AppointmentId) return BadRequest(new { message = "ID mismatch" });
        var result = await _service.CancelAppointmentAsync(patientId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/reschedule")]
    public async Task<IActionResult> RequestReschedule(Guid id, [FromBody] MobileRescheduleDto dto)
    {
        var patientId = GetPatientId();
        var result = await _service.RequestRescheduleAsync(patientId, id, dto.NewDate, dto.NewTime);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}