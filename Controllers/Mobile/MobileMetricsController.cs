using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Mobile;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/metrics")]
public class MobileMetricsController : MobileBaseController
{
    private readonly IMobileService _service;

    public MobileMetricsController(IMobileService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitDailyMetric([FromBody] MobileDailyMetricDto dto)
    {
        var patientId = GetPatientId();
        var result = await _service.SubmitDailyMetricAsync(patientId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayMetric()
    {
        var patientId = GetPatientId();
        var result = await _service.GetTodayMetricAsync(patientId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}