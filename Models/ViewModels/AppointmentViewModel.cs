using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessCenterProject.Models.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Eğitmen seçimi zorunludur.")]
        [Display(Name = "Eğitmen")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Randevu Tarihi")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Randevu saati zorunludur.")]
        [Display(Name = "Randevu Saati")]
        public TimeSpan StartTime { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        [Display(Name = "Notlar (Opsiyonel)")]
        public string? Notes { get; set; }

        // Dropdown listeleri için
        public List<SelectListItem> Services { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Trainers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableTimes { get; set; } = new List<SelectListItem>();
    }
}