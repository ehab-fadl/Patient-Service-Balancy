using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.DepressionStory;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepressionStoryController : ControllerBase
{
    private readonly IDepressionStoryService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public DepressionStoryController(IDepressionStoryService service)
    {
        _service = service;
    }

    /// <summary>
    /// جلب قصة الاكتئاب لمريض محدد
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatientId(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.GetByPatientIdAsync(patientId, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// تحديث قصة الاكتئاب
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] DepressionStoryUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.UpdateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// حذف قصة الاكتئاب (Soft Delete)
    /// </summary>
    [HttpDelete("patient/{patientId:guid}")]
    public async Task<IActionResult> Delete(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.DeleteAsync(patientId, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;

        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue))
            return false;

        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}