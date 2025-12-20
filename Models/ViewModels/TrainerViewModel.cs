namespace FitnessCenterProject.Models.ViewModels
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Specialization { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }

        // İlişkili veriler
        public List<string> Services { get; set; } = new List<string>();
        public List<TrainerAvailabilityViewModel> Availabilities { get; set; } = new List<TrainerAvailabilityViewModel>();
    }

    public class TrainerAvailabilityViewModel
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string DayName => DayOfWeek switch
        {
            DayOfWeek.Monday => "Pazartesi",
            DayOfWeek.Tuesday => "Salı",
            DayOfWeek.Wednesday => "Çarşamba",
            DayOfWeek.Thursday => "Perşembe",
            DayOfWeek.Friday => "Cuma",
            DayOfWeek.Saturday => "Cumartesi",
            DayOfWeek.Sunday => "Pazar",
            _ => ""
        };
    }
}