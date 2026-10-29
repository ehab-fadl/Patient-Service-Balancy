using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Appointments;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public AppointmentsController(IAppointmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetAppointmentsAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetByIdAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AppointmentCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.CreateAppointmentAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AppointmentUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.Id) return BadRequest("ID mismatch");
        var result = await _service.UpdateAppointmentAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.ConfirmAppointmentAsync(id, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] AppointmentCancelDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.Id) return BadRequest("ID mismatch");
        var result = await _service.CancelAppointmentAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/no-show")]
    public async Task<IActionResult> MarkNoShow(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.MarkNoShowAsync(id, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DeleteAppointmentAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}