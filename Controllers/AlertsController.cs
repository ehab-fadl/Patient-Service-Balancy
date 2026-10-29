using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public AlertsController(IAlertService service)
    {
        _service = service;
    }

    /// <summary>
    /// جلب التنبيهات النشطة للطبيب
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetActiveAlerts()
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.GetActiveAlertsAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إعادة توليد التنبيهات (تحديث بناءً على آخر البيانات)
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAlerts()
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.RefreshAlertsAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تجاهل تنبيه واحد
    /// </summary>
    [HttpPut("{id:guid}/dismiss")]
    public async Task<IActionResult> DismissAlert(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.DismissAlertAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// تجاهل كل التنبيهات النشطة
    /// </summary>
    [HttpPut("dismiss-all")]
    public async Task<IActionResult> DismissAll()
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.DismissAllAlertsAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}