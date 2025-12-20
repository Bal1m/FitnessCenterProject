using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models.Entities
{
    public class GymSettings
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Salon adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Salon Adı")]
        public string GymName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açılış saati zorunludur.")]
        [Display(Name = "Açılış Saati")]
        public TimeSpan OpenTime { get; set; }

        [Required(ErrorMessage = "Kapanış saati zorunludur.")]
        [Display(Name = "Kapanış Saati")]
        public TimeSpan CloseTime { get; set; }

        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Hakkında")]
        public string? Description { get; set; }

        [Display(Name = "Logo")]
        public string? LogoUrl { get; set; }
    }
}