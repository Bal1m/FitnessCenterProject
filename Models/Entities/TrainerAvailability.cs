using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenterProject.Models.Entities
{
    public class TrainerAvailability
    {
        public int Id { get; set; }

        [Required]
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

        // Navigation Property
        [ForeignKey("TrainerId")]
        public virtual Trainer Trainer { get; set; } = null!;
    }
}