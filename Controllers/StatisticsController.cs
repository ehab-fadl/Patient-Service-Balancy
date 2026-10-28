using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public StatisticsController(IStatisticsService service)
    {
        _service = service;
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetMetrics(
        Guid patientId,
        [FromQuery] string range = "week",
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });

        var result = await _service.GetMetricsAsync(patientId, range, startDate, endDate, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}