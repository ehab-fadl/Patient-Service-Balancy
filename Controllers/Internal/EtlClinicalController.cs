using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Internal;

[ApiController]
[Route("api/internal/etl")]
public class EtlClinicalController : ControllerBase
{
    private readonly IEtlService _service;
    private const int MaxPageSize = 1000;

    public EtlClinicalController(IEtlService service)
    {
        _service = service;
    }

    [HttpGet("diagnoses")]
    public async Task<IActionResult> GetDiagnoses(
        [FromQuery] DateTime? updatedSince,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        page = Math.Max(page, 1);

        var result = await _service.GetDiagnosesAsync(updatedSince, page, pageSize);
        return Ok(result);
    }

    [HttpGet("chronic-conditions")]
    public async Task<IActionResult> GetChronicConditions(
        [FromQuery] DateTime? updatedSince,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        page = Math.Max(page, 1);

        var result = await _service.GetChronicConditionsAsync(updatedSince, page, pageSize);
        return Ok(result);
    }

    [HttpGet("family-history")]
    public async Task<IActionResult> GetFamilyHistory(
        [FromQuery] DateTime? updatedSince,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        page = Math.Max(page, 1);

        var result = await _service.GetFamilyHistoryAsync(updatedSince, page, pageSize);
        return Ok(result);
    }
}