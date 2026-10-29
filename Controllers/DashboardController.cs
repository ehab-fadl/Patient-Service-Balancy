using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.GetDashboardAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}