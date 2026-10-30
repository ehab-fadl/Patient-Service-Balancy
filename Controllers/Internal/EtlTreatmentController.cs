using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Internal;

[ApiController]
[Route("api/internal/etl")]
public class EtlTreatmentController : ControllerBase
{
    private readonly IEtlService _service;
    private const int MaxPageSize = 1000;

    public EtlTreatmentController(IEtlService service)
    {
        _service = service;
    }

    [HttpGet("visits")]
    public async Task<IActionResult> GetVisits(
        [FromQuery] DateTime? updatedSince,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        page = Math.Max(page, 1);

        var result = await _service.GetVisitsAsync(updatedSince, page, pageSize);
        return Ok(result);
    }

    [HttpGet("treatment-plan-goals")]
    public async Task<IActionResult> GetTreatmentPlanGoals(
        [FromQuery] DateTime? updatedSince,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        page = Math.Max(page, 1);

        var result = await _service.GetTreatmentPlanGoalsAsync(updatedSince, page, pageSize);
        return Ok(result);
    }

    [HttpGet("vital-signs")]
    public async Task<IActionResult> GetVitalSigns(
        [FromQuery] DateTime? updatedSince,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        page = Math.Max(page, 1);

        var result = await _service.GetVitalSignsAsync(updatedSince, page, pageSize);
        return Ok(result);
    }
}