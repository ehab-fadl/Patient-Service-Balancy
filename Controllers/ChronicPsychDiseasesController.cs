using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.ChronicPsychDisease;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChronicPsychDiseasesController : ControllerBase
{
    private readonly IChronicPsychDiseaseService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public ChronicPsychDiseasesController(IChronicPsychDiseaseService service)
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
    public async Task<IActionResult> Create([FromBody] ChronicPsychDiseaseCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });
        var result = await _service.CreateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ChronicPsychDiseaseUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح" });
        if (id != dto.Id) return BadRequest(new { message = "معرّف المرض في المسار لا يطابق المعرّف في البيانات" });

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