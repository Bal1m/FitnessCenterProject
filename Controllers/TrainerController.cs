using Microsoft.AspNetCore.Mvc;

namespace FitnessCenterProject.Controllers
{
    public class TrainerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
