using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Internal;

[ApiController]
[Route("api/internal/etl")]
public class EtlHealthController : ControllerBase
{
    private readonly IEtlService _service;

    public EtlHealthController(IEtlService service)
    {
        _service = service;
    }

    /// <summary>
    /// فحص صحة خدمة المريض للـ ETL
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> GetHealth()
    {
        var result = await _service.GetHealthAsync();
        return Ok(result);
    }
}