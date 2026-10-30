using Microsoft.AspNetCore.Mvc;

namespace PatientService.Controllers.Internal
{
    public class EtlLifestyleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
