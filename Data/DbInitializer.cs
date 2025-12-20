using Microsoft.AspNetCore.Identity;
using FitnessCenterProject.Models.Entities;

namespace FitnessCenterProject.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Admin User
            await SeedAdminUserAsync(userManager);

            // Seed Gym Settings
            await SeedGymSettingsAsync(context);

            // Seed Services
            await SeedServicesAsync(context);

            // Seed Trainers
            await SeedTrainersAsync(context);

            // Seed Trainer Availabilities
            await SeedTrainerAvailabilitiesAsync(context);

            // Seed Trainer Services
            await SeedTrainerServicesAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Member" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            // BURAYA KENDİ ÖĞRENCİ NUMARANI YAZ!
            string adminEmail = "B231210083@ogr.sakarya.edu.tr";
            string adminPassword = "sau";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin Kullanıcı",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedGymSettingsAsync(ApplicationDbContext context)
        {
            if (!context.GymSettings.Any())
            {
                var gymSettings = new GymSettings
                {
                    GymName = "FitLife Spor Salonu",
                    OpenTime = new TimeSpan(6, 0, 0),   // 06:00
                    CloseTime = new TimeSpan(23, 0, 0), // 23:00
                    Address = "Sakarya Üniversitesi Kampüsü, Serdivan/Sakarya",
                    PhoneNumber = "0264 295 50 00",
                    Email = "info@fitlife.com",
                    Description = "Modern ekipmanlar ve uzman eğitmenlerle sağlıklı yaşamın adresi."
                };

                context.GymSettings.Add(gymSettings);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedServicesAsync(ApplicationDbContext context)
        {
            if (!context.Services.Any())
            {
                var services = new List<Service>
                {
                    new Service
                    {
                        Name = "Fitness",
                        Description = "Kişiye özel fitness antrenmanı ile kas geliştirme ve form tutma.",
                        DurationMinutes = 60,
                        Price = 250,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Yoga",
                        Description = "Zihin ve beden uyumunu sağlayan rahatlama ve esneklik egzersizleri.",
                        DurationMinutes = 45,
                        Price = 200,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Pilates",
                        Description = "Core kaslarını güçlendiren, duruş ve esnekliği artıran egzersizler.",
                        DurationMinutes = 50,
                        Price = 220,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Kick Boks",
                        Description = "Kardiyovasküler dayanıklılık ve savunma teknikleri eğitimi.",
                        DurationMinutes = 60,
                        Price = 280,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Yüzme",
                        Description = "Profesyonel yüzme eğitimi ve su içi egzersizler.",
                        DurationMinutes = 45,
                        Price = 300,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Kilo Verme Programı",
                        Description = "Kişiye özel beslenme danışmanlığı eşliğinde yağ yakımı odaklı antrenman.",
                        DurationMinutes = 60,
                        Price = 350,
                        IsActive = true
                    }
                };

                context.Services.AddRange(services);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTrainersAsync(ApplicationDbContext context)
        {
            if (!context.Trainers.Any())
            {
                var trainers = new List<Trainer>
                {
                    new Trainer
                    {
                        FullName = "Ahmet Yılmaz",
                        Email = "ahmet.yilmaz@fitlife.com",
                        PhoneNumber = "0532 111 22 33",
                        Specialization = "Kas Geliştirme, Fitness",
                        Bio = "10 yıllık deneyimli fitness eğitmeni. IFBB sertifikalı.",
                        IsActive = true
                    },
                    new Trainer
                    {
                        FullName = "Elif Kaya",
                        Email = "elif.kaya@fitlife.com",
                        PhoneNumber = "0533 222 33 44",
                        Specialization = "Yoga, Pilates",
                        Bio = "Hindistan'da eğitim almış uzman yoga eğitmeni.",
                        IsActive = true
                    },
                    new Trainer
                    {
                        FullName = "Mehmet Demir",
                        Email = "mehmet.demir@fitlife.com",
                        PhoneNumber = "0534 333 44 55",
                        Specialization = "Kick Boks, Kilo Verme",
                        Bio = "Eski milli sporcu, 8 yıldır eğitmenlik yapıyor.",
                        IsActive = true
                    },
                    new Trainer
                    {
                        FullName = "Zeynep Arslan",
                        Email = "zeynep.arslan@fitlife.com",
                        PhoneNumber = "0535 444 55 66",
                        Specialization = "Yüzme, Pilates",
                        Bio = "Profesyonel yüzücü ve su sporları uzmanı.",
                        IsActive = true
                    }
                };

                context.Trainers.AddRange(trainers);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTrainerAvailabilitiesAsync(ApplicationDbContext context)
        {
            if (!context.TrainerAvailabilities.Any())
            {
                var trainers = context.Trainers.ToList();
                var availabilities = new List<TrainerAvailability>();

                // Ahmet Yılmaz - Pazartesi, Çarşamba, Cuma
                var ahmet = trainers.FirstOrDefault(t => t.FullName == "Ahmet Yılmaz");
                if (ahmet != null)
                {
                    availabilities.Add(new TrainerAvailability { TrainerId = ahmet.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = ahmet.Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = ahmet.Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) });
                }

                // Elif Kaya - Salı, Perşembe, Cumartesi
                var elif = trainers.FirstOrDefault(t => t.FullName == "Elif Kaya");
                if (elif != null)
                {
                    availabilities.Add(new TrainerAvailability { TrainerId = elif.Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(16, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = elif.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(16, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = elif.Id, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(14, 0, 0) });
                }

                // Mehmet Demir - Pazartesi, Salı, Perşembe, Cuma
                var mehmet = trainers.FirstOrDefault(t => t.FullName == "Mehmet Demir");
                if (mehmet != null)
                {
                    availabilities.Add(new TrainerAvailability { TrainerId = mehmet.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(21, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = mehmet.Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(21, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = mehmet.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(21, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = mehmet.Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(21, 0, 0) });
                }

                // Zeynep Arslan - Çarşamba, Cumartesi, Pazar
                var zeynep = trainers.FirstOrDefault(t => t.FullName == "Zeynep Arslan");
                if (zeynep != null)
                {
                    availabilities.Add(new TrainerAvailability { TrainerId = zeynep.Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(7, 0, 0), EndTime = new TimeSpan(15, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = zeynep.Id, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) });
                    availabilities.Add(new TrainerAvailability { TrainerId = zeynep.Id, DayOfWeek = DayOfWeek.Sunday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(13, 0, 0) });
                }

                context.TrainerAvailabilities.AddRange(availabilities);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTrainerServicesAsync(ApplicationDbContext context)
        {
            if (!context.TrainerServices.Any())
            {
                var trainers = context.Trainers.ToList();
                var services = context.Services.ToList();
                var trainerServices = new List<TrainerService>();

                // Ahmet Yılmaz - Fitness, Kilo Verme
                var ahmet = trainers.FirstOrDefault(t => t.FullName == "Ahmet Yılmaz");
                var fitness = services.FirstOrDefault(s => s.Name == "Fitness");
                var kiloVerme = services.FirstOrDefault(s => s.Name == "Kilo Verme Programı");
                if (ahmet != null && fitness != null)
                    trainerServices.Add(new TrainerService { TrainerId = ahmet.Id, ServiceId = fitness.Id });
                if (ahmet != null && kiloVerme != null)
                    trainerServices.Add(new TrainerService { TrainerId = ahmet.Id, ServiceId = kiloVerme.Id });

                // Elif Kaya - Yoga, Pilates
                var elif = trainers.FirstOrDefault(t => t.FullName == "Elif Kaya");
                var yoga = services.FirstOrDefault(s => s.Name == "Yoga");
                var pilates = services.FirstOrDefault(s => s.Name == "Pilates");
                if (elif != null && yoga != null)
                    trainerServices.Add(new TrainerService { TrainerId = elif.Id, ServiceId = yoga.Id });
                if (elif != null && pilates != null)
                    trainerServices.Add(new TrainerService { TrainerId = elif.Id, ServiceId = pilates.Id });

                // Mehmet Demir - Kick Boks, Kilo Verme, Fitness
                var mehmet = trainers.FirstOrDefault(t => t.FullName == "Mehmet Demir");
                var kickBoks = services.FirstOrDefault(s => s.Name == "Kick Boks");
                if (mehmet != null && kickBoks != null)
                    trainerServices.Add(new TrainerService { TrainerId = mehmet.Id, ServiceId = kickBoks.Id });
                if (mehmet != null && kiloVerme != null)
                    trainerServices.Add(new TrainerService { TrainerId = mehmet.Id, ServiceId = kiloVerme.Id });
                if (mehmet != null && fitness != null)
                    trainerServices.Add(new TrainerService { TrainerId = mehmet.Id, ServiceId = fitness.Id });

                // Zeynep Arslan - Yüzme, Pilates
                var zeynep = trainers.FirstOrDefault(t => t.FullName == "Zeynep Arslan");
                var yuzme = services.FirstOrDefault(s => s.Name == "Yüzme");
                if (zeynep != null && yuzme != null)
                    trainerServices.Add(new TrainerService { TrainerId = zeynep.Id, ServiceId = yuzme.Id });
                if (zeynep != null && pilates != null)
                    trainerServices.Add(new TrainerService { TrainerId = zeynep.Id, ServiceId = pilates.Id });

                context.TrainerServices.AddRange(trainerServices);
                await context.SaveChangesAsync();
            }
        }
    }
}