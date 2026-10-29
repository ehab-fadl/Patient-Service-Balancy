using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Mobile;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/assessments")]
public class MobileAssessmentsController : MobileBaseController
{
    private readonly IMobileService _service;

    public MobileAssessmentsController(IMobileService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAssessment([FromBody] MobileAssessmentSubmitDto dto)
    {
        var patientId = GetPatientId();
        var result = await _service.SubmitAssessmentAsync(patientId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var patientId = GetPatientId();
        var result = await _service.GetAssessmentHistoryAsync(patientId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}