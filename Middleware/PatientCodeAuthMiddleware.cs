using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;

namespace PatientService.Middleware;

public class PatientCodeAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PatientCodeAuthMiddleware> _logger;

    public PatientCodeAuthMiddleware(RequestDelegate next, ILogger<PatientCodeAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, PatientDbContext dbContext)
    {
        _logger.LogWarning("═══════════════════════════════════════════");
        _logger.LogWarning("🔍 MIDDLEWARE: {Path} [{Method}]", context.Request.Path, context.Request.Method);

        // اطبع كل الـ Headers
        foreach (var h in context.Request.Headers)
        {
            _logger.LogWarning("   Header: {Key} = {Value}", h.Key, h.Value);
        }

        if (!context.Request.Path.StartsWithSegments("/api/mobile"))
        {
            _logger.LogWarning("⏭️ Skipping non-mobile path");
            await _next(context);
            return;
        }

        string? code = null;

        // محاولة 1: Header
        if (context.Request.Headers.TryGetValue("X-Patient-Code", out var headerValue))
        {
            code = headerValue.ToString().Trim();
            _logger.LogWarning("✅ Code from Header: '{Code}'", code);
        }
        // محاولة 2: Body (فقط login)
        else if (context.Request.Path.StartsWithSegments("/api/mobile/auth/login"))
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            _logger.LogWarning("📄 Body: {Body}", body);

            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    var json = System.Text.Json.JsonDocument.Parse(body);
                    if (json.RootElement.TryGetProperty("patientCode", out var prop))
                        code = prop.GetString()?.Trim();
                    _logger.LogWarning("✅ Code from Body: '{Code}'", code);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("❌ JSON parse error: {Error}", ex.Message);
                }
            }
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogWarning("❌ CODE IS MISSING → 401");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "كود المريض (X-Patient-Code) مطلوب" });
            return;
        }

        _logger.LogWarning("🔎 Searching DB for code: '{Code}'", code);
        var patientCode = await dbContext.PatientCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive && !c.IsDeleted);

        if (patientCode is null)
        {
            _logger.LogWarning("❌ Code '{Code}' NOT FOUND in DB → 401", code);

            var allCodes = await dbContext.PatientCodes.AsNoTracking().Take(20).ToListAsync();
            _logger.LogWarning("📋 Available codes in DB:");
            foreach (var c in allCodes)
            {
                _logger.LogWarning("   Code='{Code}', IsActive={Active}, IsDeleted={Deleted}, PatientId={PID}",
                    c.Code, c.IsActive, c.IsDeleted, c.PatientId);
            }

            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "كود المريض غير صالح أو غير نشط" });
            return;
        }

        _logger.LogWarning("✅ AUTH OK. PatientId: {PatientId}", patientCode.PatientId);
        context.Items["PatientId"] = patientCode.PatientId;
        context.Items["PatientCode"] = patientCode.Code;

        await _next(context);
    }
}