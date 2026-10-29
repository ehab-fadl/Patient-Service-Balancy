using Microsoft.AspNetCore.Mvc;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/profile")]
public class MobileProfileController : MobileBaseController
{
    private readonly IMobileService _service;

    public MobileProfileController(IMobileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var patientId = GetPatientId();
        var result = await _service.GetProfileAsync(patientId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}