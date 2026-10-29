using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/alerts")]
public class MobileAlertsController : MobileBaseController
{
    private readonly IMobileService _service;

    public MobileAlertsController(IMobileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlerts()
    {
        var patientId = GetPatientId();
        var result = await _service.GetAlertsAsync(patientId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/dismiss")]
    public async Task<IActionResult> DismissAlert(Guid id)
    {
        var patientId = GetPatientId();
        var result = await _service.DismissAlertAsync(patientId, id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}