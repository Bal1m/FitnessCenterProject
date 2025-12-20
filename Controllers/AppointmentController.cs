using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models.Entities;
using FitnessCenterProject.Models.Enums;
using FitnessCenterProject.Models.ViewModels;

namespace FitnessCenterProject.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Appointment/Index - Kullanıcının randevuları
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .Select(a => new AppointmentListViewModel
                {
                    Id = a.Id,
                    ServiceName = a.Service.Name,
                    TrainerName = a.Trainer.FullName,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    DurationMinutes = a.Service.DurationMinutes,
                    TotalPrice = a.TotalPrice,
                    Status = a.Status,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return View(appointments);
        }

        // GET: /Appointment/Create
        public async Task<IActionResult> Create()
        {
            var model = new AppointmentCreateViewModel
            {
                AppointmentDate = DateTime.Today.AddDays(1),
                Services = await _context.Services
                    .Where(s => s.IsActive)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = $"{s.Name} - {s.DurationMinutes} dk - {s.Price:N0} TL"
                    })
                    .ToListAsync(),
                Trainers = new List<SelectListItem>()
            };

            return View(model);
        }

        // POST: /Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var service = await _context.Services.FindAsync(model.ServiceId);
                var trainer = await _context.Trainers.FindAsync(model.TrainerId);

                if (service == null || trainer == null)
                {
                    ModelState.AddModelError("", "Geçersiz hizmet veya eğitmen seçimi.");
                    await PopulateDropdowns(model);
                    return View(model);
                }

                // Bitiş saatini hesapla
                var endTime = model.StartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));

                // Çakışma kontrolü
                var hasConflict = await CheckAppointmentConflict(
                    model.TrainerId,
                    model.AppointmentDate,
                    model.StartTime,
                    endTime,
                    null);

                if (hasConflict)
                {
                    ModelState.AddModelError("", "Bu eğitmenin seçilen tarih ve saatte başka bir randevusu bulunmaktadır. Lütfen farklı bir saat seçiniz.");
                    await PopulateDropdowns(model);
                    return View(model);
                }

                // Eğitmen müsaitlik kontrolü
                var isAvailable = await CheckTrainerAvailability(
                    model.TrainerId,
                    model.AppointmentDate,
                    model.StartTime,
                    endTime);

                if (!isAvailable)
                {
                    ModelState.AddModelError("", "Eğitmen seçilen tarih ve saatte müsait değildir.");
                    await PopulateDropdowns(model);
                    return View(model);
                }

                var appointment = new Appointment
                {
                    UserId = userId!,
                    ServiceId = model.ServiceId,
                    TrainerId = model.TrainerId,
                    AppointmentDate = model.AppointmentDate,
                    StartTime = model.StartTime,
                    EndTime = endTime,
                    TotalPrice = service.Price,
                    Status = AppointmentStatus.Pending,
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Randevunuz başarıyla oluşturuldu. Onay bekleniyor.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model);
            return View(model);
        }

        // GET: /Appointment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
            {
                return NotFound();
            }

            var model = new AppointmentListViewModel
            {
                Id = appointment.Id,
                ServiceName = appointment.Service.Name,
                TrainerName = appointment.Trainer.FullName,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                DurationMinutes = appointment.Service.DurationMinutes,
                TotalPrice = appointment.TotalPrice,
                Status = appointment.Status,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt
            };

            return View(model);
        }

        // POST: /Appointment/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
            {
                return NotFound();
            }

            // Sadece beklemede veya onaylanmış randevular iptal edilebilir
            if (appointment.Status != AppointmentStatus.Pending &&
                appointment.Status != AppointmentStatus.Approved)
            {
                TempData["Error"] = "Bu randevu iptal edilemez.";
                return RedirectToAction(nameof(Index));
            }

            // Geçmiş tarihli randevular iptal edilemez
            if (appointment.AppointmentDate < DateTime.Today)
            {
                TempData["Error"] = "Geçmiş tarihli randevular iptal edilemez.";
                return RedirectToAction(nameof(Index));
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevunuz başarıyla iptal edildi.";
            return RedirectToAction(nameof(Index));
        }

        // ==================== AJAX ENDPOINTS ====================

        // GET: /Appointment/GetTrainersByService?serviceId=1
        [HttpGet]
        public async Task<IActionResult> GetTrainersByService(int serviceId)
        {
            var trainers = await _context.TrainerServices
                .Where(ts => ts.ServiceId == serviceId && ts.Trainer.IsActive)
                .Select(ts => new
                {
                    value = ts.Trainer.Id,
                    text = ts.Trainer.FullName
                })
                .ToListAsync();

            return Json(trainers);
        }

        // GET: /Appointment/GetAvailableTimes?trainerId=1&date=2025-01-15&serviceId=1
        [HttpGet]
        public async Task<IActionResult> GetAvailableTimes(int trainerId, DateTime date, int serviceId)
        {
            var dayOfWeek = date.DayOfWeek;

            // Eğitmenin o güne ait müsaitlik saatlerini al
            var availability = await _context.TrainerAvailabilities
                .Where(ta => ta.TrainerId == trainerId &&
                             ta.DayOfWeek == dayOfWeek &&
                             ta.IsActive)
                .FirstOrDefaultAsync();

            if (availability == null)
            {
                return Json(new List<object>());
            }

            // Hizmetin süresini al
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                return Json(new List<object>());
            }

            // O gün için mevcut randevuları al
            var existingAppointments = await _context.Appointments
                .Where(a => a.TrainerId == trainerId &&
                           a.AppointmentDate == date.Date &&
                           a.Status != AppointmentStatus.Cancelled &&
                           a.Status != AppointmentStatus.Rejected)
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync();

            // Müsait saatleri hesapla (30 dakikalık slotlar)
            var availableTimes = new List<object>();
            var currentTime = availability.StartTime;
            var slotDuration = TimeSpan.FromMinutes(30);

            while (currentTime.Add(TimeSpan.FromMinutes(service.DurationMinutes)) <= availability.EndTime)
            {
                var slotEnd = currentTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));

                // Bu slot'un mevcut randevularla çakışıp çakışmadığını kontrol et
                var hasConflict = existingAppointments.Any(ea =>
                    (currentTime >= ea.StartTime && currentTime < ea.EndTime) ||
                    (slotEnd > ea.StartTime && slotEnd <= ea.EndTime) ||
                    (currentTime <= ea.StartTime && slotEnd >= ea.EndTime));

                // Geçmiş saatleri kontrol et
                var isPast = date.Date == DateTime.Today && currentTime < DateTime.Now.TimeOfDay;

                if (!hasConflict && !isPast)
                {
                    availableTimes.Add(new
                    {
                        value = currentTime.ToString(@"hh\:mm"),
                        text = $"{currentTime:hh\\:mm} - {slotEnd:hh\\:mm}"
                    });
                }

                currentTime = currentTime.Add(slotDuration);
            }

            return Json(availableTimes);
        }

        // ==================== HELPER METHODS ====================

        private async Task<bool> CheckAppointmentConflict(int trainerId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId)
        {
            var query = _context.Appointments
                .Where(a => a.TrainerId == trainerId &&
                           a.AppointmentDate == date.Date &&
                           a.Status != AppointmentStatus.Cancelled &&
                           a.Status != AppointmentStatus.Rejected);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            var existingAppointments = await query
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync();

            // Çakışma kontrolü
            return existingAppointments.Any(ea =>
                (startTime >= ea.StartTime && startTime < ea.EndTime) ||
                (endTime > ea.StartTime && endTime <= ea.EndTime) ||
                (startTime <= ea.StartTime && endTime >= ea.EndTime));
        }

        private async Task<bool> CheckTrainerAvailability(int trainerId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var dayOfWeek = date.DayOfWeek;

            var availability = await _context.TrainerAvailabilities
                .Where(ta => ta.TrainerId == trainerId &&
                            ta.DayOfWeek == dayOfWeek &&
                            ta.IsActive)
                .FirstOrDefaultAsync();

            if (availability == null)
            {
                return false;
            }

            return startTime >= availability.StartTime && endTime <= availability.EndTime;
        }

        private async Task PopulateDropdowns(AppointmentCreateViewModel model)
        {
            model.Services = await _context.Services
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} - {s.DurationMinutes} dk - {s.Price:N0} TL"
                })
                .ToListAsync();

            if (model.ServiceId > 0)
            {
                model.Trainers = await _context.TrainerServices
                    .Where(ts => ts.ServiceId == model.ServiceId && ts.Trainer.IsActive)
                    .Select(ts => new SelectListItem
                    {
                        Value = ts.Trainer.Id.ToString(),
                        Text = ts.Trainer.FullName
                    })
                    .ToListAsync();
            }
            else
            {
                model.Trainers = new List<SelectListItem>();
            }
        }
    }
}