using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessCenterProject.Models.ViewModels
{
    public class TrainerCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Uzmanlık alanı en fazla 200 karakter olabilir.")]
        [Display(Name = "Uzmanlık Alanları")]
        public string Specialization { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Biyografi en fazla 1000 karakter olabilir.")]
        [Display(Name = "Hakkında")]
        public string? Bio { get; set; }

        [Display(Name = "Fotoğraf URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        // Seçili hizmetler
        [Display(Name = "Verebildiği Hizmetler")]
        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        // Dropdown için hizmet listesi
        public List<SelectListItem> AvailableServices { get; set; } = new List<SelectListItem>();
    }
}