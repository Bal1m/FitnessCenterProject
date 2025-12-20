using FitnessCenterProject.Models.Enums;

namespace FitnessCenterProject.Models.ViewModels
{
    public class AppointmentListViewModel
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalPrice { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Durum için Türkçe metin
        public string StatusText => Status switch
        {
            AppointmentStatus.Pending => "Beklemede",
            AppointmentStatus.Approved => "Onaylandı",
            AppointmentStatus.Rejected => "Reddedildi",
            AppointmentStatus.Completed => "Tamamlandı",
            AppointmentStatus.Cancelled => "İptal Edildi",
            _ => "Bilinmiyor"
        };

        // Durum için CSS class
        public string StatusBadgeClass => Status switch
        {
            AppointmentStatus.Pending => "bg-warning",
            AppointmentStatus.Approved => "bg-success",
            AppointmentStatus.Rejected => "bg-danger",
            AppointmentStatus.Completed => "bg-info",
            AppointmentStatus.Cancelled => "bg-secondary",
            _ => "bg-secondary"
        };
    }
}