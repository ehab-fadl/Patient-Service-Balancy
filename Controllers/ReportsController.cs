using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Reports;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    private const string DoctorIdHeader = "X-Doctor-Id";

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    /// <summary>
    /// جلب بيانات المريض الأساسية لملء نموذج التقرير تلقائياً
    /// </summary>
    [HttpGet("snapshot/{patientId:guid}")]
    public async Task<IActionResult> GetPatientSnapshot(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetPatientSnapshotAsync(patientId, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إنشاء تقرير جديد (سريري أو قانوني)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] ReportCreateDto dto)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.CreateReportAsync(dto, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// جلب كل تقارير مريض
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetReportsByPatientAsync(patientId, doctorId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// جلب تقرير واحد (للمعاينة أو الطباعة)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.GetReportByIdAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// حذف تقرير
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.DeleteReportAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// إضافة/إزالة من المفضلة
    /// </summary>
    [HttpPut("{id:guid}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var result = await _service.ToggleFavoriteAsync(id, doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    private bool TryGetDoctorId(out Guid doctorId)
    {
        doctorId = Guid.Empty;
        if (!Request.Headers.TryGetValue(DoctorIdHeader, out var headerValue)) return false;
        return Guid.TryParse(headerValue.ToString(), out doctorId);
    }
}