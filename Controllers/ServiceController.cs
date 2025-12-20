using Microsoft.AspNetCore.Mvc;

namespace FitnessCenterProject.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
