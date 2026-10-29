using Microsoft.AspNetCore.Mvc;

namespace PatientService.Controllers.Mobile;

public abstract class MobileBaseController : ControllerBase
{
    protected Guid GetPatientId()
    {
        if (HttpContext.Items["PatientId"] is Guid patientId)
            return patientId;

        throw new UnauthorizedAccessException("PatientId غير موجود في السياق");
    }

    protected string GetPatientCode()
    {
        return HttpContext.Items["PatientCode"] as string ?? string.Empty;
    }
}