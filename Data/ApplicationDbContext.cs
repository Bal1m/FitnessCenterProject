using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Models.Entities;

namespace FitnessCenterProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<TrainerService> TrainerServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<GymSettings> GymSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Service Configuration
            builder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.Name);
            });

            // Trainer Configuration
            builder.Entity<Trainer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // TrainerAvailability Configuration
            builder.Entity<TrainerAvailability>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Trainer)
                      .WithMany(t => t.Availabilities)
                      .HasForeignKey(e => e.TrainerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TrainerService Configuration (Many-to-Many)
            builder.Entity<TrainerService>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.TrainerId, e.ServiceId }).IsUnique();

                entity.HasOne(e => e.Trainer)
                      .WithMany(t => t.TrainerServices)
                      .HasForeignKey(e => e.TrainerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Service)
                      .WithMany(s => s.TrainerServices)
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment Configuration
            builder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => new { e.TrainerId, e.AppointmentDate, e.StartTime });

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Appointments)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Trainer)
                      .WithMany(t => t.Appointments)
                      .HasForeignKey(e => e.TrainerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Service)
                      .WithMany(s => s.Appointments)
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // GymSettings Configuration
            builder.Entity<GymSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}