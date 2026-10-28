using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Documents;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public DocumentsController(IDocumentService service)
    {
        _service = service;
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetByPatientIdAsync(patientId, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetByIdAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        [FromForm] Guid patientId,
        [FromForm] IFormFile file,
        [FromForm] string name,
        [FromForm] string type,
        [FromForm] string sourceType,
        [FromForm] DateTime documentDate,
        [FromForm] string? issuer,
        [FromForm] string? category,
        [FromForm] string? refNo,
        [FromForm] DateTime? expiryDate,
        [FromForm] bool isSigned,
        [FromForm] bool isCritical,
        [FromForm] string? notes,
        [FromForm] string? tags)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "يجب إرفاق ملف" });

        var result = await _service.UploadDocumentAsync(
            patientId, file, name, type, sourceType, documentDate,
            issuer, category, refNo, expiryDate, isSigned, isCritical, notes, tags, doctorId);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DocumentUpdateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (id != dto.Id) return BadRequest("ID mismatch");
        var result = await _service.UpdateDocumentAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.ToggleFavoriteAsync(id, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DeleteDocumentAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DownloadFileAsync(id, doctorId);
        if (!result.Success || result.Data == null) return NotFound(result);

        var doc = await _service.GetByIdAsync(id, doctorId);
        var fileName = doc.Data?.FileName ?? "document";

        return File(result.Data, "application/octet-stream", fileName);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}