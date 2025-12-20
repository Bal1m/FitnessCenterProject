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
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==================== DASHBOARD ====================
        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalMembers = await _userManager.Users.CountAsync(),
                TotalTrainers = await _context.Trainers.CountAsync(),
                TotalServices = await _context.Services.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),

                PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Pending),
                ApprovedAppointments = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Approved),
                CompletedAppointments = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Completed),
                CancelledAppointments = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Cancelled),

                TotalRevenue = await _context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Completed)
                    .SumAsync(a => a.TotalPrice),

                MonthlyRevenue = await _context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Completed &&
                                a.AppointmentDate.Month == DateTime.Now.Month &&
                                a.AppointmentDate.Year == DateTime.Now.Year)
                    .SumAsync(a => a.TotalPrice),

                RecentAppointments = await _context.Appointments
                    .Include(a => a.User)
                    .Include(a => a.Trainer)
                    .Include(a => a.Service)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .Select(a => new AppointmentListViewModel
                    {
                        Id = a.Id,
                        ServiceName = a.Service.Name,
                        TrainerName = a.Trainer.FullName,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        TotalPrice = a.TotalPrice,
                        Status = a.Status
                    })
                    .ToListAsync()
            };

            return View(dashboard);
        }

        // ==================== SERVICES (HİZMETLER) ====================

        // GET: Admin/Services
        public async Task<IActionResult> Services()
        {
            var services = await _context.Services
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(services);
        }

        // GET: Admin/ServiceCreate
        public IActionResult ServiceCreate()
        {
            return View(new ServiceCreateViewModel());
        }

        // POST: Admin/ServiceCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceCreate(ServiceCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Name = model.Name,
                    Description = model.Description,
                    DurationMinutes = model.DurationMinutes,
                    Price = model.Price,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Hizmet başarıyla eklendi.";
                return RedirectToAction(nameof(Services));
            }

            return View(model);
        }

        // GET: Admin/ServiceEdit/5
        public async Task<IActionResult> ServiceEdit(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var model = new ServiceCreateViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                DurationMinutes = service.DurationMinutes,
                Price = service.Price,
                IsActive = service.IsActive
            };

            return View(model);
        }

        // POST: Admin/ServiceEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceEdit(int id, ServiceCreateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound();
                }

                service.Name = model.Name;
                service.Description = model.Description;
                service.DurationMinutes = model.DurationMinutes;
                service.Price = model.Price;
                service.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Hizmet başarıyla güncellendi.";
                return RedirectToAction(nameof(Services));
            }

            return View(model);
        }

        // POST: Admin/ServiceDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceDelete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hizmet başarıyla silindi.";
            }

            return RedirectToAction(nameof(Services));
        }

        // ==================== TRAINERS (EĞİTMENLER) ====================

        // GET: Admin/Trainers
        public async Task<IActionResult> Trainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return View(trainers);
        }

        // GET: Admin/TrainerCreate
        public async Task<IActionResult> TrainerCreate()
        {
            var model = new TrainerCreateViewModel
            {
                AvailableServices = await _context.Services
                    .Where(s => s.IsActive)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Admin/TrainerCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainerCreate(TrainerCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var trainer = new Trainer
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Specialization = model.Specialization,
                    Bio = model.Bio,
                    ImageUrl = model.ImageUrl,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();

                // Seçili hizmetleri ekle
                if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
                {
                    foreach (var serviceId in model.SelectedServiceIds)
                    {
                        _context.TrainerServices.Add(new TrainerService
                        {
                            TrainerId = trainer.Id,
                            ServiceId = serviceId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Eğitmen başarıyla eklendi.";
                return RedirectToAction(nameof(Trainers));
            }

            model.AvailableServices = await _context.Services
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            return View(model);
        }

        // GET: Admin/TrainerEdit/5
        public async Task<IActionResult> TrainerEdit(int id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.TrainerServices)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null)
            {
                return NotFound();
            }

            var model = new TrainerCreateViewModel
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Email = trainer.Email,
                PhoneNumber = trainer.PhoneNumber,
                Specialization = trainer.Specialization,
                Bio = trainer.Bio,
                ImageUrl = trainer.ImageUrl,
                IsActive = trainer.IsActive,
                SelectedServiceIds = trainer.TrainerServices.Select(ts => ts.ServiceId).ToList(),
                AvailableServices = await _context.Services
                    .Where(s => s.IsActive)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Admin/TrainerEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainerEdit(int id, TrainerCreateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var trainer = await _context.Trainers
                    .Include(t => t.TrainerServices)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (trainer == null)
                {
                    return NotFound();
                }

                trainer.FullName = model.FullName;
                trainer.Email = model.Email;
                trainer.PhoneNumber = model.PhoneNumber;
                trainer.Specialization = model.Specialization;
                trainer.Bio = model.Bio;
                trainer.ImageUrl = model.ImageUrl;
                trainer.IsActive = model.IsActive;

                // Mevcut hizmetleri sil
                _context.TrainerServices.RemoveRange(trainer.TrainerServices);

                // Yeni hizmetleri ekle
                if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
                {
                    foreach (var serviceId in model.SelectedServiceIds)
                    {
                        _context.TrainerServices.Add(new TrainerService
                        {
                            TrainerId = trainer.Id,
                            ServiceId = serviceId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Eğitmen başarıyla güncellendi.";
                return RedirectToAction(nameof(Trainers));
            }

            model.AvailableServices = await _context.Services
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            return View(model);
        }

        // POST: Admin/TrainerDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainerDelete(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Eğitmen başarıyla silindi.";
            }

            return RedirectToAction(nameof(Trainers));
        }

        // ==================== TRAINER AVAILABILITY (MÜSAİTLİK) ====================

        // GET: Admin/TrainerAvailabilities
        public async Task<IActionResult> TrainerAvailabilities()
        {
            var availabilities = await _context.TrainerAvailabilities
                .Include(ta => ta.Trainer)
                .OrderBy(ta => ta.Trainer.FullName)
                .ThenBy(ta => ta.DayOfWeek)
                .ToListAsync();

            return View(availabilities);
        }

        // GET: Admin/AvailabilityCreate
        public async Task<IActionResult> AvailabilityCreate()
        {
            var model = new TrainerAvailabilityCreateViewModel
            {
                Trainers = await _context.Trainers
                    .Where(t => t.IsActive)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.FullName
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Admin/AvailabilityCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AvailabilityCreate(TrainerAvailabilityCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var availability = new TrainerAvailability
                {
                    TrainerId = model.TrainerId,
                    DayOfWeek = model.DayOfWeek,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    IsActive = model.IsActive
                };

                _context.TrainerAvailabilities.Add(availability);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Müsaitlik başarıyla eklendi.";
                return RedirectToAction(nameof(TrainerAvailabilities));
            }

            model.Trainers = await _context.Trainers
                .Where(t => t.IsActive)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.FullName
                })
                .ToListAsync();

            return View(model);
        }

        // POST: Admin/AvailabilityDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AvailabilityDelete(int id)
        {
            var availability = await _context.TrainerAvailabilities.FindAsync(id);
            if (availability != null)
            {
                _context.TrainerAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Müsaitlik başarıyla silindi.";
            }

            return RedirectToAction(nameof(TrainerAvailabilities));
        }

        // ==================== APPOINTMENTS (RANDEVULAR) ====================

        // GET: Admin/Appointments
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(appointments);
        }

        // POST: Admin/AppointmentApprove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppointmentApprove(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Approved;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu onaylandı.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        // POST: Admin/AppointmentReject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppointmentReject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Rejected;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu reddedildi.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        // POST: Admin/AppointmentComplete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppointmentComplete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Completed;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu tamamlandı olarak işaretlendi.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        // POST: Admin/AppointmentDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppointmentDelete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu başarıyla silindi.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        // ==================== USERS (KULLANICILAR) ====================

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    Roles = roles.ToList(),
                    TotalAppointments = await _context.Appointments.CountAsync(a => a.UserId == user.Id)
                });
            }

            return View(userViewModels);
        }

        // POST: Admin/UserToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = user.IsActive ? "Kullanıcı aktif edildi." : "Kullanıcı devre dışı bırakıldı.";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}