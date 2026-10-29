using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Mobile;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/medications")]
public class MobileMedicationsController : MobileBaseController
{
    private readonly IMobileService _service;

    public MobileMedicationsController(IMobileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMedications()
    {
        var patientId = GetPatientId();
        var result = await _service.GetMedicationsAsync(patientId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("side-effects")]
    public async Task<IActionResult> SubmitSideEffect([FromBody] MobileSideEffectSubmitDto dto)
    {
        var patientId = GetPatientId();
        var result = await _service.SubmitSideEffectAsync(patientId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}