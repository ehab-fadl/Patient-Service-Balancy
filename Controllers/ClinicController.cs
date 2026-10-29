using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicController : ControllerBase
{
    private readonly IClinicService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public ClinicController(IClinicService service)
    {
        _service = service;
    }

    /// <summary>
    /// جلب جميع المرضى مع إحصائياتهم
    /// </summary>
    [HttpGet("patients")]
    public async Task<IActionResult> GetAllPatients()
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.GetAllPatientsAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// جلب إحصائيات العيادة الشاملة
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.GetStatisticsAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}