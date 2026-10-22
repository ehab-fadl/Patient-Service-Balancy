using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.PatientPersonalInfo;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientPersonalInfoController : ControllerBase
{
    private readonly IPatientPersonalInfoService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public PatientPersonalInfoController(IPatientPersonalInfoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.GetAllAsync(doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.GetByIdAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.SearchAsync(doctorId, term);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PatientPersonalInfoCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.CreateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PatientPersonalInfoUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId))
            return Unauthorized(new { message = "معرّف الطبيب غير موجود أو غير صالح في الترويسة X-Doctor-Id" });

        var result = await _service.UpdateAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

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

        // TODO: استبدل هذا بـ JWT Claims عند بناء AuthService
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue))
            return false;

        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}