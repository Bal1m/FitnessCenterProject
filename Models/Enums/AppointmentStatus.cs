namespace FitnessCenterProject.Models.Enums
{
    public enum AppointmentStatus
    {
        Pending = 0,    // Beklemede
        Approved = 1,   // Onaylandı
        Rejected = 2,   // Reddedildi
        Completed = 3,  // Tamamlandı
        Cancelled = 4   // İptal Edildi
    }
}