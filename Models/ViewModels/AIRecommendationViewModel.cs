using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models.ViewModels
{
    public class AIRecommendationViewModel
    {
        [Required(ErrorMessage = "Boy bilgisi zorunludur.")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır.")]
        [Display(Name = "Boy (cm)")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Kilo bilgisi zorunludur.")]
        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Yaş bilgisi zorunludur.")]
        [Range(10, 100, ErrorMessage = "Yaş 10-100 arasında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur.")]
        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vücut tipi seçimi zorunludur.")]
        [Display(Name = "Vücut Tipi")]
        public string BodyType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hedef seçimi zorunludur.")]
        [Display(Name = "Hedefiniz")]
        public string Goal { get; set; } = string.Empty;

        [Display(Name = "Aktivite Seviyesi")]
        public string ActivityLevel { get; set; } = string.Empty;

        [Display(Name = "Sağlık Problemleri (Varsa)")]
        [StringLength(500)]
        public string? HealthIssues { get; set; }

        // Sonuç için
        public string? Recommendation { get; set; }
        public decimal? BMI { get; set; }
        public string? BMICategory { get; set; }

        // YENİ: AI Görsel URL'si
        public string? TransformationImageUrl { get; set; }
        public string? ImagePrompt { get; set; }
    }
}