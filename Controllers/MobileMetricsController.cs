using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Statistics;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/mobile/metrics")]
public class MobileMetricsController : ControllerBase
{
    private readonly IStatisticsService _service;

    public MobileMetricsController(IStatisticsService service)
    {
        _service = service;
    }

    /// <summary>
    /// يقوم تطبيق المريض بإرسال القراءات اليومية باستخدام كود المريض (PatientCode)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubmitDailyMetric([FromBody] DailyMetricSubmitDto dto, [FromHeader(Name = "X-Patient-Code")] string patientCode)
    {
        if (string.IsNullOrWhiteSpace(patientCode))
            return Unauthorized(new { message = "كود المريض (X-Patient-Code) مطلوب في الترويسة" });

        var result = await _service.SubmitDailyMetricAsync(patientCode, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}