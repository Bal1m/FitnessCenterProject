using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models.ViewModels
{
    public class ServiceCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Hizmet adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Süre zorunludur.")]
        [Range(15, 180, ErrorMessage = "Süre 15-180 dakika arasında olmalıdır.")]
        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Ücret zorunludur.")]
        [Range(0, 10000, ErrorMessage = "Ücret 0-10000 TL arasında olmalıdır.")]
        [Display(Name = "Ücret (TL)")]
        public decimal Price { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;
    }
}