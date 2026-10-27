using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.PatientNote;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientNotesController : ControllerBase
{
    private readonly IPatientNoteService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public PatientNotesController(IPatientNoteService service)
    {
        _service = service;
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });
        var result = await _service.GetByPatientIdAsync(patientId, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });
        var result = await _service.GetByIdAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PatientNoteCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });
        var result = await _service.CreateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PatientNoteUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });
        if (id != dto.Id) return BadRequest(new { message = "معرّف الملاحظة في المسار لا يطابق المعرّف في البيانات" });

        var result = await _service.UpdateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });
        var result = await _service.DeleteAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}