using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.TreatmentPlan;
using PatientService.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TreatmentPlansController : ControllerBase
{
    private readonly ITreatmentPlanService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public TreatmentPlansController(ITreatmentPlanService service) => _service = service;

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetByPatientIdAsync(patientId, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TreatmentPlanCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.CreatePlanAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TreatmentPlanUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.Id) return BadRequest("ID mismatch");
        var result = await _service.UpdatePlanAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DeletePlanAsync(id, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("problems")]
    public async Task<IActionResult> AddProblem([FromBody] TreatmentProblemCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.AddProblemAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("problems/{id:guid}")]
    public async Task<IActionResult> UpdateProblem(Guid id, [FromBody] TreatmentProblemUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.Id) return BadRequest("ID mismatch");
        var result = await _service.UpdateProblemAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("problems/{id:guid}")]
    public async Task<IActionResult> DeleteProblem(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DeleteProblemAsync(id, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("goals")]
    public async Task<IActionResult> AddGoal([FromBody] TreatmentGoalCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.AddGoalAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("goals/{id:guid}")]
    public async Task<IActionResult> UpdateGoalConfig(Guid id, [FromBody] TreatmentGoalUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.Id) return BadRequest("ID mismatch");
        var result = await _service.UpdateGoalConfigAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("goals/{id:guid}/progress")]
    public async Task<IActionResult> UpdateGoalProgress(Guid id, [FromBody] TreatmentGoalProgressDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.GoalId) return BadRequest("ID mismatch");
        var result = await _service.UpdateGoalProgressAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("goals/{id:guid}/achieve")]
    public async Task<IActionResult> AchieveGoal(Guid id, [FromBody] TreatmentGoalAchieveDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.GoalId) return BadRequest("ID mismatch");
        var result = await _service.AchieveGoalAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("goals/{id:guid}")]
    public async Task<IActionResult> DeleteGoal(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DeleteGoalAsync(id, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("means")]
    public async Task<IActionResult> AddMean([FromBody] TreatmentMeanCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.AddMeanAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("means/{id:guid}")]
    public async Task<IActionResult> UpdateMean(Guid id, [FromBody] TreatmentMeanUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.Id) return BadRequest("ID mismatch");
        var result = await _service.UpdateMeanAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("means/{id:guid}")]
    public async Task<IActionResult> DeleteMean(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DeleteMeanAsync(id, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }


    [HttpPut("goals/{id:guid}/unachieve")]
    public async Task<IActionResult> UnachieveGoal(Guid id, [FromBody] TreatmentGoalUnachieveDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.GoalId) return BadRequest("ID mismatch");

        var result = await _service.UnachieveGoalAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}