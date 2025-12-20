namespace FitnessCenterProject.Models.ViewModels
{
    public class DashboardViewModel
    {
        // İstatistikler
        public int TotalMembers { get; set; }
        public int TotalTrainers { get; set; }
        public int TotalServices { get; set; }
        public int TotalAppointments { get; set; }

        // Randevu durumları
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

        // Gelir
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }

        // Son randevular
        public List<AppointmentListViewModel> RecentAppointments { get; set; } = new List<AppointmentListViewModel>();

        // Son üyeler
        public List<UserListViewModel> RecentMembers { get; set; } = new List<UserListViewModel>();
    }
}