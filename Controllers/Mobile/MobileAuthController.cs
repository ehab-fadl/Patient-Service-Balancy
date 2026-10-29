using Microsoft.AspNetCore.Mvc;
using PatientService.Services.DTOs.Mobile;
using PatientService.Services.Interfaces;

namespace PatientService.Controllers.Mobile;

[ApiController]
[Route("api/mobile/auth")]
public class MobileAuthController : ControllerBase
{
    private readonly IMobileService _service;

    public MobileAuthController(IMobileService service)
    {
        _service = service;
    }

    /// <summary>
    /// تسجيل الدخول بكود المريض
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] MobileLoginDto dto)
    {
        // Middleware وضع PatientId في السياق بعد التحقق من الكود
        if (HttpContext.Items["PatientId"] is not Guid patientId)
            return Unauthorized(new { message = "كود المريض غير صالح" });

        var profile = await _service.GetProfileAsync(patientId);
        if (!profile.Success)
            return BadRequest(new { message = "تعذر جلب بيانات المريض", errors = profile.Errors });

        return Ok(new MobileLoginResponseDto
        {
            Success = true,
            Message = $"مرحباً {profile.Data!.FullName}",
            Patient = profile.Data
        });
    }
}