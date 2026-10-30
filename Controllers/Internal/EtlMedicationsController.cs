using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Internal;

[ApiController]
[Route("api/internal/etl/medications")]
public class EtlMedicationsController : ControllerBase
{
    private readonly IEtlService _service;
    private const int MaxPageSize = 1000;

    public EtlMedicationsController(IEtlService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? updatedSince,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 500)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        page = Math.Max(page, 1);

        var result = await _service.GetMedicationsAsync(updatedSince, page, pageSize);
        return Ok(result);
    }
}