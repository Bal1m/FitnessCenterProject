using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenterProject.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Boy (cm)")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır.")]
        public int? Height { get; set; }

        [Display(Name = "Kilo (kg)")]
        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasında olmalıdır.")]
        public decimal? Weight { get; set; }

        [StringLength(50)]
        [Display(Name = "Vücut Tipi")]
        public string? BodyType { get; set; }

        [Display(Name = "Profil Fotoğrafı")]
        public string? ProfileImageUrl { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        // Navigation Property
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}