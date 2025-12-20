using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Ana sayfa için verileri hazýrla
            ViewBag.Services = await _context.Services
                .Where(s => s.IsActive)
                .Take(6)
                .ToListAsync();

            ViewBag.Trainers = await _context.Trainers
                .Where(t => t.IsActive)
                .Take(4)
                .ToListAsync();

            ViewBag.GymSettings = await _context.GymSettings.FirstOrDefaultAsync();

            return View();
        }

        public async Task<IActionResult> Services()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(services);
        }

        public async Task<IActionResult> Trainers()
        {
            var trainers = await _context.Trainers
                .Where(t => t.IsActive)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)
                .Include(t => t.Availabilities)
                .OrderBy(t => t.FullName)
                .ToListAsync();

            return View(trainers);
        }

        public async Task<IActionResult> About()
        {
            ViewBag.GymSettings = await _context.GymSettings.FirstOrDefaultAsync();
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}