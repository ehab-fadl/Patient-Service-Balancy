using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Mobile;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/treatment")]
public class MobileTreatmentController : MobileBaseController
{
    private readonly IMobileService _service;

    public MobileTreatmentController(IMobileService service)
    {
        _service = service;
    }

    [HttpGet("active-plan")]
    public async Task<IActionResult> GetActivePlan()
    {
        var patientId = GetPatientId();
        var result = await _service.GetActiveTreatmentPlanAsync(patientId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("goals/{goalId:guid}/progress")]
    public async Task<IActionResult> UpdateGoalProgress(Guid goalId, [FromBody] MobileGoalProgressDto dto)
    {
        var patientId = GetPatientId();
        var result = await _service.UpdateGoalProgressAsync(patientId, goalId, dto.Value, dto.Note);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}