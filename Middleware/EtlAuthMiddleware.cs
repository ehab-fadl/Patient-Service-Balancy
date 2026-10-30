using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PatientService.Middleware;

public class EtlAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EtlAuthMiddleware> _logger;
    private readonly string _expectedApiKey;

    public EtlAuthMiddleware(
        RequestDelegate next,
        IConfiguration config,
        ILogger<EtlAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _expectedApiKey = config["Etl:ApiKey"]
            ?? throw new InvalidOperationException("ETL API Key not configured in appsettings.json");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // فقط لمسارات /api/internal/etl
        if (!context.Request.Path.StartsWithSegments("/api/internal/etl"))
        {
            await _next(context);
            return;
        }

        // 1. فحص API Key
        if (!context.Request.Headers.TryGetValue("X-ETL-API-Key", out var key)
            || !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(key.ToString()),
                System.Text.Encoding.UTF8.GetBytes(_expectedApiKey)))
        {
            _logger.LogWarning("ETL request rejected: Invalid or missing API Key from {RemoteIp}",
                context.Connection.RemoteIpAddress);

            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "ETL API Key is invalid or missing",
                code = "ETL_UNAUTHORIZED"
            });
            return;
        }

        // 2. (اختياري) فحص IP Whitelist
        var allowedIps = context.RequestServices
            .GetService<IConfiguration>()?
            .GetSection("Etl:AllowedIps")
            .Get<string[]>();

        if (allowedIps != null && allowedIps.Any())
        {
            var remoteIp = context.Connection.RemoteIpAddress?.ToString();
            if (!allowedIps.Contains(remoteIp))
            {
                _logger.LogWarning("ETL request rejected: IP {Ip} not whitelisted", remoteIp);

                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "IP not allowed for ETL",
                    code = "ETL_IP_FORBIDDEN"
                });
                return;
            }
        }

        // 3. تسجيل كل طلب
        _logger.LogInformation("ETL Request: {Method} {Path} from {Ip}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        await _next(context);
    }
}