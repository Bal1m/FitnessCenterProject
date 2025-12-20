using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenterProject.Models.Entities
{
    public class TrainerService
    {
        public int Id { get; set; }

        public int TrainerId { get; set; }

        public int ServiceId { get; set; }

        // Navigation Properties
        [ForeignKey("TrainerId")]
        public virtual Trainer Trainer { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; } = null!;
    }
}