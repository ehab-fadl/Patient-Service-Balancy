using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.ChronicDisease;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChronicDiseasesController : ControllerBase
{
    private readonly IChronicDiseaseService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public ChronicDiseasesController(IChronicDiseaseService service)
    {
        _service = service;
    }

    /// <summary>
    /// جلب كل الأمراض المزمنة لمريض معيّن
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.GetByPatientIdAsync(patientId, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// جلب مرض مزمن واحد بالمعرّف
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.GetByIdAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// إضافة مرض مزمن جديد
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChronicDiseaseCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.CreateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تحديث مرض مزمن
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ChronicDiseaseUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        // ضمان التطابق بين Route و Body
        if (id != dto.Id)
            return BadRequest(new { message = "معرّف المرض في المسار لا يطابق المعرّف في البيانات" });

        var result = await _service.UpdateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// حذف مرض مزمن (Soft Delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.DeleteAsync(id, doctorId);
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