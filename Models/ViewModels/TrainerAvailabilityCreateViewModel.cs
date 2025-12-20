using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessCenterProject.Models.ViewModels
{
    public class TrainerAvailabilityCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Eğitmen seçimi zorunludur.")]
        [Display(Name = "Eğitmen")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Gün seçimi zorunludur.")]
        [Display(Name = "Gün")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        // Dropdown için
        public List<SelectListItem> Trainers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Days { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Pazartesi" },
            new SelectListItem { Value = "2", Text = "Salı" },
            new SelectListItem { Value = "3", Text = "Çarşamba" },
            new SelectListItem { Value = "4", Text = "Perşembe" },
            new SelectListItem { Value = "5", Text = "Cuma" },
            new SelectListItem { Value = "6", Text = "Cumartesi" },
            new SelectListItem { Value = "0", Text = "Pazar" }
        };
    }
}