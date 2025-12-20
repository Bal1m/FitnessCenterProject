using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models.Enums;

namespace FitnessCenterProject.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== EĞİTMEN API'LERİ ====================

        /// <summary>
        /// Tüm aktif eğitmenleri listeler
        /// GET: api/ReportApi/trainers
        /// </summary>
        [HttpGet("trainers")]
        public async Task<IActionResult> GetAllTrainers()
        {
            var trainers = await _context.Trainers
                .Where(t => t.IsActive)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Email,
                    t.PhoneNumber,
                    t.Specialization,
                    t.Bio,
                    t.ImageUrl,
                    Services = t.TrainerServices.Select(ts => new
                    {
                        ts.Service.Id,
                        ts.Service.Name,
                        ts.Service.DurationMinutes,
                        ts.Service.Price
                    }),
                    Availabilities = t.Availabilities.Where(a => a.IsActive).Select(a => new
                    {
                        a.DayOfWeek,
                        StartTime = a.StartTime.ToString(@"hh\:mm"),
                        EndTime = a.EndTime.ToString(@"hh\:mm")
                    })
                })
                .OrderBy(t => t.FullName)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = trainers.Count,
                data = trainers
            });
        }

        /// <summary>
        /// Belirli bir eğitmenin detaylarını getirir
        /// GET: api/ReportApi/trainers/5
        /// </summary>
        [HttpGet("trainers/{id}")]
        public async Task<IActionResult> GetTrainerById(int id)
        {
            var trainer = await _context.Trainers
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Email,
                    t.PhoneNumber,
                    t.Specialization,
                    t.Bio,
                    t.ImageUrl,
                    Services = t.TrainerServices.Select(ts => new
                    {
                        ts.Service.Id,
                        ts.Service.Name,
                        ts.Service.DurationMinutes,
                        ts.Service.Price
                    }),
                    Availabilities = t.Availabilities.Where(a => a.IsActive).Select(a => new
                    {
                        a.DayOfWeek,
                        StartTime = a.StartTime.ToString(@"hh\:mm"),
                        EndTime = a.EndTime.ToString(@"hh\:mm")
                    }),
                    TotalAppointments = t.Appointments.Count,
                    CompletedAppointments = t.Appointments.Count(a => a.Status == AppointmentStatus.Completed)
                })
                .FirstOrDefaultAsync();

            if (trainer == null)
            {
                return NotFound(new { success = false, message = "Eğitmen bulunamadı." });
            }

            return Ok(new { success = true, data = trainer });
        }

        /// <summary>
        /// Belirli bir tarihte uygun eğitmenleri getirir
        /// GET: api/ReportApi/trainers/available?date=2025-01-15
        /// </summary>
        [HttpGet("trainers/available")]
        public async Task<IActionResult> GetAvailableTrainers([FromQuery] DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;

            var availableTrainers = await _context.Trainers
                .Where(t => t.IsActive && t.Availabilities.Any(a => a.DayOfWeek == dayOfWeek && a.IsActive))
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialization,
                    t.ImageUrl,
                    Services = t.TrainerServices.Select(ts => ts.Service.Name),
                    Availability = t.Availabilities
                        .Where(a => a.DayOfWeek == dayOfWeek && a.IsActive)
                        .Select(a => new
                        {
                            StartTime = a.StartTime.ToString(@"hh\:mm"),
                            EndTime = a.EndTime.ToString(@"hh\:mm")
                        })
                        .FirstOrDefault(),
                    // O günkü randevu sayısı
                    AppointmentsOnDate = t.Appointments.Count(a =>
                        a.AppointmentDate == date.Date &&
                        a.Status != AppointmentStatus.Cancelled &&
                        a.Status != AppointmentStatus.Rejected)
                })
                .OrderBy(t => t.FullName)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                date = date.ToString("yyyy-MM-dd"),
                dayOfWeek = dayOfWeek.ToString(),
                count = availableTrainers.Count,
                data = availableTrainers
            });
        }

        /// <summary>
        /// Uzmanlık alanına göre eğitmenleri filtreler
        /// GET: api/ReportApi/trainers/by-specialization?spec=yoga
        /// </summary>
        [HttpGet("trainers/by-specialization")]
        public async Task<IActionResult> GetTrainersBySpecialization([FromQuery] string spec)
        {
            if (string.IsNullOrWhiteSpace(spec))
            {
                return BadRequest(new { success = false, message = "Uzmanlık alanı parametresi gereklidir." });
            }

            var trainers = await _context.Trainers
                .Where(t => t.IsActive && t.Specialization.ToLower().Contains(spec.ToLower()))
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialization,
                    t.Email,
                    t.PhoneNumber,
                    Services = t.TrainerServices.Select(ts => ts.Service.Name)
                })
                .OrderBy(t => t.FullName)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                filter = spec,
                count = trainers.Count,
                data = trainers
            });
        }

        // ==================== HİZMET API'LERİ ====================

        /// <summary>
        /// Tüm aktif hizmetleri listeler
        /// GET: api/ReportApi/services
        /// </summary>
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.DurationMinutes,
                    s.Price,
                    TrainerCount = s.TrainerServices.Count(ts => ts.Trainer.IsActive),
                    TotalAppointments = s.Appointments.Count
                })
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = services.Count,
                data = services
            });
        }

        /// <summary>
        /// Fiyat aralığına göre hizmetleri filtreler
        /// GET: api/ReportApi/services/by-price?minPrice=100&maxPrice=300
        /// </summary>
        [HttpGet("services/by-price")]
        public async Task<IActionResult> GetServicesByPriceRange(
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            var query = _context.Services.Where(s => s.IsActive);

            if (minPrice.HasValue)
            {
                query = query.Where(s => s.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.Price <= maxPrice.Value);
            }

            var services = await query
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.DurationMinutes,
                    s.Price
                })
                .OrderBy(s => s.Price)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                filter = new { minPrice, maxPrice },
                count = services.Count,
                data = services
            });
        }

        // ==================== RANDEVU API'LERİ ====================

        /// <summary>
        /// Belirli bir üyenin randevularını getirir
        /// GET: api/ReportApi/appointments/user/abc123
        /// </summary>
        [HttpGet("appointments/user/{userId}")]
        public async Task<IActionResult> GetUserAppointments(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    ServiceName = a.Service.Name,
                    TrainerName = a.Trainer.FullName,
                    a.AppointmentDate,
                    StartTime = a.StartTime.ToString(@"hh\:mm"),
                    EndTime = a.EndTime.ToString(@"hh\:mm"),
                    a.TotalPrice,
                    Status = a.Status.ToString(),
                    a.Notes,
                    a.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                userId,
                userName = user.Email,
                count = appointments.Count,
                data = appointments
            });
        }

        /// <summary>
        /// Belirli bir tarihteki tüm randevuları getirir
        /// GET: api/ReportApi/appointments/by-date?date=2025-01-15
        /// </summary>
        [HttpGet("appointments/by-date")]
        public async Task<IActionResult> GetAppointmentsByDate([FromQuery] DateTime date)
        {
            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDate == date.Date)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    UserName = a.User.FullName,
                    UserEmail = a.User.Email,
                    ServiceName = a.Service.Name,
                    TrainerName = a.Trainer.FullName,
                    StartTime = a.StartTime.ToString(@"hh\:mm"),
                    EndTime = a.EndTime.ToString(@"hh\:mm"),
                    a.TotalPrice,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                date = date.ToString("yyyy-MM-dd"),
                count = appointments.Count,
                data = appointments
            });
        }

        /// <summary>
        /// Randevu durumuna göre filtreleme
        /// GET: api/ReportApi/appointments/by-status?status=Pending
        /// </summary>
        [HttpGet("appointments/by-status")]
        public async Task<IActionResult> GetAppointmentsByStatus([FromQuery] string status)
        {
            if (!Enum.TryParse<AppointmentStatus>(status, true, out var appointmentStatus))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Geçersiz durum. Geçerli değerler: Pending, Approved, Rejected, Completed, Cancelled"
                });
            }

            var appointments = await _context.Appointments
                .Where(a => a.Status == appointmentStatus)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.Id,
                    UserName = a.User.FullName,
                    ServiceName = a.Service.Name,
                    TrainerName = a.Trainer.FullName,
                    a.AppointmentDate,
                    StartTime = a.StartTime.ToString(@"hh\:mm"),
                    a.TotalPrice
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                status = status,
                count = appointments.Count,
                data = appointments
            });
        }

        // ==================== İSTATİSTİK API'LERİ ====================

        /// <summary>
        /// Genel istatistikleri getirir
        /// GET: api/ReportApi/statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = new
            {
                TotalMembers = await _context.Users.CountAsync(),
                TotalTrainers = await _context.Trainers.CountAsync(t => t.IsActive),
                TotalServices = await _context.Services.CountAsync(s => s.IsActive),
                TotalAppointments = await _context.Appointments.CountAsync(),

                AppointmentsByStatus = new
                {
                    Pending = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Pending),
                    Approved = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Approved),
                    Completed = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Completed),
                    Cancelled = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Cancelled),
                    Rejected = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Rejected)
                },

                Revenue = new
                {
                    Total = await _context.Appointments
                        .Where(a => a.Status == AppointmentStatus.Completed)
                        .SumAsync(a => a.TotalPrice),
                    ThisMonth = await _context.Appointments
                        .Where(a => a.Status == AppointmentStatus.Completed &&
                                   a.AppointmentDate.Month == DateTime.Now.Month &&
                                   a.AppointmentDate.Year == DateTime.Now.Year)
                        .SumAsync(a => a.TotalPrice)
                },

                TopServices = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.Appointments.Count)
                    .Take(5)
                    .Select(s => new
                    {
                        s.Name,
                        AppointmentCount = s.Appointments.Count
                    })
                    .ToListAsync(),

                TopTrainers = await _context.Trainers
                    .Where(t => t.IsActive)
                    .OrderByDescending(t => t.Appointments.Count(a => a.Status == AppointmentStatus.Completed))
                    .Take(5)
                    .Select(t => new
                    {
                        t.FullName,
                        CompletedAppointments = t.Appointments.Count(a => a.Status == AppointmentStatus.Completed)
                    })
                    .ToListAsync()
            };

            return Ok(new { success = true, data = stats });
        }

        /// <summary>
        /// Tarih aralığına göre gelir raporu
        /// GET: api/ReportApi/revenue?startDate=2025-01-01&endDate=2025-01-31
        /// </summary>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var revenue = await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Completed &&
                           a.AppointmentDate >= startDate.Date &&
                           a.AppointmentDate <= endDate.Date)
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(a => a.TotalPrice),
                    AppointmentCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                period = new
                {
                    startDate = startDate.ToString("yyyy-MM-dd"),
                    endDate = endDate.ToString("yyyy-MM-dd")
                },
                totalRevenue = revenue.Sum(r => r.TotalRevenue),
                totalAppointments = revenue.Sum(r => r.AppointmentCount),
                data = revenue.Select(r => new
                {
                    date = r.Date.ToString("yyyy-MM-dd"),
                    r.TotalRevenue,
                    r.AppointmentCount
                })
            });
        }
    }
}