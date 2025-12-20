using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessCenterProject.Models.ViewModels;
using System.Text;
using System.Text.Json;

namespace FitnessCenterProject.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AIController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: /AI/Recommendation
        [HttpGet]
        public IActionResult Recommendation()
        {
            return View(new AIRecommendationViewModel());
        }

        // POST: /AI/Recommendation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recommendation(AIRecommendationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Input validation - güvenlik için
            if (!ValidateInput(model))
            {
                ModelState.AddModelError("", "Geçersiz giriş değerleri tespit edildi.");
                return View(model);
            }

            // BMI Hesapla
            var heightInMeters = model.Height / 100m;
            model.BMI = Math.Round(model.Weight / (heightInMeters * heightInMeters), 1);
            model.BMICategory = GetBMICategory(model.BMI.Value);

            // Yapay Zeka önerisi al
            try
            {
                var geminiApiKey = _configuration["Gemini:ApiKey"];

                if (!string.IsNullOrEmpty(geminiApiKey))
                {
                    // Gemini API kullan (ÜCRETSİZ)
                    model.Recommendation = await GetGeminiRecommendation(model, geminiApiKey);
                }
                else
                {
                    // Kural tabanlı öneri sistemi
                    model.Recommendation = GetRuleBasedRecommendation(model);
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kural tabanlı sisteme geç
                model.Recommendation = GetRuleBasedRecommendation(model);
            }

            // AI Görsel oluştur
            model.TransformationImageUrl = GenerateTransformationImageUrl(model);

            return View(model);
        }

        // ==================== GEMINI API ====================

        private async Task<string> GetGeminiRecommendation(AIRecommendationViewModel model, string apiKey)
        {
            var prompt = BuildPrompt(model);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 1500
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Gemini API hatası");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var recommendation = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return recommendation ?? GetRuleBasedRecommendation(model);
        }

        // ==================== HELPER METHODS ====================

        private bool ValidateInput(AIRecommendationViewModel model)
        {
            // SQL Injection ve XSS koruması
            var dangerousPatterns = new[] { "<script", "javascript:", "onclick", "onerror", "DROP TABLE", "DELETE FROM", "--", ";" };

            var fieldsToCheck = new[] { model.Gender, model.BodyType, model.Goal, model.ActivityLevel, model.HealthIssues ?? "" };

            foreach (var field in fieldsToCheck)
            {
                if (dangerousPatterns.Any(p => field.ToLower().Contains(p.ToLower())))
                {
                    return false;
                }
            }

            // Değer aralıkları kontrolü
            if (model.Height < 100 || model.Height > 250) return false;
            if (model.Weight < 30 || model.Weight > 300) return false;
            if (model.Age < 10 || model.Age > 100) return false;

            return true;
        }

        private string GetBMICategory(decimal bmi)
        {
            return bmi switch
            {
                < 18.5m => "Zayıf",
                < 25m => "Normal",
                < 30m => "Fazla Kilolu",
                < 35m => "Obez (Sınıf 1)",
                < 40m => "Obez (Sınıf 2)",
                _ => "Aşırı Obez (Sınıf 3)"
            };
        }

        private string BuildPrompt(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sen bir profesyonel fitness ve beslenme uzmanısın. Aşağıdaki bilgilere göre Türkçe olarak kişiselleştirilmiş, detaylı ve motive edici bir egzersiz ve diyet önerisi hazırla.");
            sb.AppendLine();
            sb.AppendLine("Kullanıcı Bilgileri:");
            sb.AppendLine($"- Cinsiyet: {model.Gender}");
            sb.AppendLine($"- Yaş: {model.Age}");
            sb.AppendLine($"- Boy: {model.Height} cm");
            sb.AppendLine($"- Kilo: {model.Weight} kg");
            sb.AppendLine($"- BMI: {model.BMI} ({model.BMICategory})");
            sb.AppendLine($"- Vücut Tipi: {model.BodyType}");
            sb.AppendLine($"- Hedef: {model.Goal}");
            sb.AppendLine($"- Aktivite Seviyesi: {model.ActivityLevel}");

            if (!string.IsNullOrEmpty(model.HealthIssues))
            {
                sb.AppendLine($"- Sağlık Durumu/Kısıtlamalar: {model.HealthIssues}");
            }

            sb.AppendLine();
            sb.AppendLine("Lütfen şu başlıkları içeren kapsamlı bir öneri hazırla:");
            sb.AppendLine("1. 📊 Genel Değerlendirme (BMI analizi ve mevcut durum)");
            sb.AppendLine("2. 🏋️ Haftalık Egzersiz Programı (gün gün detaylı)");
            sb.AppendLine("3. 🥗 Günlük Beslenme Önerileri (öğün öğün)");
            sb.AppendLine("4. 💧 Su ve Uyku Önerileri");
            sb.AppendLine("5. 💡 Hedefe Ulaşmak İçin İpuçları");
            sb.AppendLine();
            sb.AppendLine("Emoji kullanarak görsel olarak zenginleştir. Motive edici ve pozitif bir dil kullan.");

            return sb.ToString();
        }

        private string GetRuleBasedRecommendation(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();

            sb.AppendLine("## 📊 Kişisel Analiz");
            sb.AppendLine();
            sb.AppendLine($"**BMI Değeriniz:** {model.BMI} ({model.BMICategory})");
            sb.AppendLine();

            // Hedef bazlı egzersiz önerileri
            sb.AppendLine("## 🏋️ Egzersiz Programı Önerisi");
            sb.AppendLine();

            switch (model.Goal?.ToLower())
            {
                case "kilo vermek":
                    sb.AppendLine(GetWeightLossExercise(model));
                    break;
                case "kilo almak":
                    sb.AppendLine(GetWeightGainExercise(model));
                    break;
                case "kas yapmak":
                    sb.AppendLine(GetMuscleGainExercise(model));
                    break;
                case "fit kalmak":
                    sb.AppendLine(GetFitnessMaintenanceExercise(model));
                    break;
                case "esneklik kazanmak":
                    sb.AppendLine(GetFlexibilityExercise(model));
                    break;
                default:
                    sb.AppendLine(GetGeneralExercise(model));
                    break;
            }

            // Diyet önerileri
            sb.AppendLine();
            sb.AppendLine("## 🥗 Beslenme Önerileri");
            sb.AppendLine();
            sb.AppendLine(GetDietRecommendation(model));

            // Genel ipuçları
            sb.AppendLine();
            sb.AppendLine("## 💡 Önemli İpuçları");
            sb.AppendLine();
            sb.AppendLine(GetGeneralTips(model));

            // Uyarı
            if (!string.IsNullOrEmpty(model.HealthIssues))
            {
                sb.AppendLine();
                sb.AppendLine("## ⚠️ Önemli Uyarı");
                sb.AppendLine();
                sb.AppendLine("Belirttiğiniz sağlık durumunuz nedeniyle, herhangi bir egzersiz programına başlamadan önce mutlaka doktorunuza danışmanızı öneririz.");
            }

            return sb.ToString();
        }

        private string GetWeightLossExercise(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Haftalık Program (Kilo Verme Odaklı):**");
            sb.AppendLine();
            sb.AppendLine("🔹 **Pazartesi:** 45 dk kardiyo (koşu/yürüyüş) + 15 dk core egzersizleri");
            sb.AppendLine("🔹 **Salı:** 30 dk HIIT antrenmanı + 15 dk stretching");
            sb.AppendLine("🔹 **Çarşamba:** Dinlenme veya hafif yürüyüş (30 dk)");
            sb.AppendLine("🔹 **Perşembe:** 40 dk kardiyo + 20 dk alt vücut çalışması");
            sb.AppendLine("🔹 **Cuma:** 30 dk HIIT + 15 dk üst vücut çalışması");
            sb.AppendLine("🔹 **Cumartesi:** 60 dk tempolu yürüyüş veya bisiklet");
            sb.AppendLine("🔹 **Pazar:** Dinlenme");
            sb.AppendLine();
            sb.AppendLine("**Önerilen Salonumuz Hizmetleri:** Fitness, Kick Boks, Kilo Verme Programı");
            return sb.ToString();
        }

        private string GetWeightGainExercise(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Haftalık Program (Kilo Alma Odaklı):**");
            sb.AppendLine();
            sb.AppendLine("🔹 **Pazartesi:** Göğüs + Triceps - Ağır yükler, düşük tekrar (8-10)");
            sb.AppendLine("🔹 **Salı:** Sırt + Biceps - Compound hareketler (barbell row, deadlift)");
            sb.AppendLine("🔹 **Çarşamba:** Dinlenme (kas gelişimi için kritik!)");
            sb.AppendLine("🔹 **Perşembe:** Bacak + Kalça - Squat, leg press, lunges");
            sb.AppendLine("🔹 **Cuma:** Omuz + Core - Ağır overhead press");
            sb.AppendLine("🔹 **Cumartesi:** Full Body - Orta yoğunluk");
            sb.AppendLine("🔹 **Pazar:** Dinlenme");
            sb.AppendLine();
            sb.AppendLine("**⚠️ Önemli Notlar:**");
            sb.AppendLine("- Kardiyo minimumda tutun (haftada max 2x, 20 dk)");
            sb.AppendLine("- Ağır yüklerle çalışın, düşük tekrar yapın");
            sb.AppendLine("- Setler arası 2-3 dk dinlenin");
            sb.AppendLine("- Compound (çoklu kas) hareketlere odaklanın");
            sb.AppendLine();
            sb.AppendLine("**Önerilen Salonumuz Hizmetleri:** Fitness, Kilo Verme Programı (kişisel antrenör desteği)");
            return sb.ToString();
        }

        private string GetMuscleGainExercise(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Haftalık Program (Kas Yapma Odaklı):**");
            sb.AppendLine();
            sb.AppendLine("🔹 **Pazartesi:** Göğüs + Triceps (45-60 dk ağırlık antrenmanı)");
            sb.AppendLine("🔹 **Salı:** Sırt + Biceps (45-60 dk ağırlık antrenmanı)");
            sb.AppendLine("🔹 **Çarşamba:** Dinlenme veya hafif kardiyo (20 dk)");
            sb.AppendLine("🔹 **Perşembe:** Bacak + Kalça (45-60 dk ağırlık antrenmanı)");
            sb.AppendLine("🔹 **Cuma:** Omuz + Core (45-60 dk ağırlık antrenmanı)");
            sb.AppendLine("🔹 **Cumartesi:** Full Body veya zayıf kas grupları");
            sb.AppendLine("🔹 **Pazar:** Dinlenme");
            sb.AppendLine();
            sb.AppendLine("**Önerilen Salonumuz Hizmetleri:** Fitness, Kilo Verme Programı");
            return sb.ToString();
        }

        private string GetFitnessMaintenanceExercise(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Haftalık Program (Fit Kalma Odaklı):**");
            sb.AppendLine();
            sb.AppendLine("🔹 **Pazartesi:** 30 dk kardiyo + 20 dk ağırlık");
            sb.AppendLine("🔹 **Salı:** 45 dk Yoga veya Pilates");
            sb.AppendLine("🔹 **Çarşamba:** 30 dk HIIT antrenmanı");
            sb.AppendLine("🔹 **Perşembe:** 40 dk yüzme veya bisiklet");
            sb.AppendLine("🔹 **Cuma:** 30 dk full body ağırlık antrenmanı");
            sb.AppendLine("🔹 **Cumartesi:** Outdoor aktivite (yürüyüş, bisiklet)");
            sb.AppendLine("🔹 **Pazar:** Dinlenme veya hafif stretching");
            sb.AppendLine();
            sb.AppendLine("**Önerilen Salonumuz Hizmetleri:** Fitness, Yoga, Pilates, Yüzme");
            return sb.ToString();
        }

        private string GetFlexibilityExercise(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Haftalık Program (Esneklik Odaklı):**");
            sb.AppendLine();
            sb.AppendLine("🔹 **Pazartesi:** 60 dk Yoga (Hatha veya Vinyasa)");
            sb.AppendLine("🔹 **Salı:** 45 dk Pilates");
            sb.AppendLine("🔹 **Çarşamba:** 30 dk stretching + 20 dk hafif kardiyo");
            sb.AppendLine("🔹 **Perşembe:** 60 dk Yoga");
            sb.AppendLine("🔹 **Cuma:** 45 dk Pilates");
            sb.AppendLine("🔹 **Cumartesi:** 30 dk foam rolling + dinamik stretching");
            sb.AppendLine("🔹 **Pazar:** Dinlenme veya meditasyon");
            sb.AppendLine();
            sb.AppendLine("**Önerilen Salonumuz Hizmetleri:** Yoga, Pilates");
            return sb.ToString();
        }

        private string GetGeneralExercise(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Haftalık Dengeli Program:**");
            sb.AppendLine();
            sb.AppendLine("🔹 **Pazartesi:** 30 dk kardiyo + 20 dk ağırlık");
            sb.AppendLine("🔹 **Salı:** 45 dk Yoga veya Pilates");
            sb.AppendLine("🔹 **Çarşamba:** Dinlenme");
            sb.AppendLine("🔹 **Perşembe:** 30 dk HIIT + 15 dk stretching");
            sb.AppendLine("🔹 **Cuma:** 40 dk full body antrenman");
            sb.AppendLine("🔹 **Cumartesi:** Outdoor aktivite");
            sb.AppendLine("🔹 **Pazar:** Dinlenme");
            sb.AppendLine();
            sb.AppendLine("**Önerilen Salonumuz Hizmetleri:** Fitness, Yoga, Pilates");
            return sb.ToString();
        }

        private string GetDietRecommendation(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();

            // Günlük kalori hesaplama (Harris-Benedict formülü - basitleştirilmiş)
            decimal bmr;
            if (model.Gender?.ToLower() == "erkek")
            {
                bmr = 88.362m + (13.397m * model.Weight) + (4.799m * model.Height) - (5.677m * model.Age);
            }
            else
            {
                bmr = 447.593m + (9.247m * model.Weight) + (3.098m * model.Height) - (4.330m * model.Age);
            }

            var activityMultiplier = model.ActivityLevel?.ToLower() switch
            {
                "sedanter" => 1.2m,
                "hafif aktif" => 1.375m,
                "orta aktif" => 1.55m,
                "çok aktif" => 1.725m,
                _ => 1.375m
            };

            var dailyCalories = Math.Round(bmr * activityMultiplier);

            // Hedefe göre kalori ayarla
            if (model.Goal?.ToLower() == "kilo vermek") dailyCalories -= 500;
            else if (model.Goal?.ToLower() == "kas yapmak") dailyCalories += 300;
            else if (model.Goal?.ToLower() == "kilo almak") dailyCalories += 500;

            sb.AppendLine($"**Günlük Tahmini Kalori İhtiyacınız:** {dailyCalories:N0} kcal");
            sb.AppendLine();
            sb.AppendLine("**Makro Besin Dağılımı:**");
            sb.AppendLine($"- Protein: {Math.Round(dailyCalories * 0.30m / 4):N0}g (günlük kalorinin %30'u)");
            sb.AppendLine($"- Karbonhidrat: {Math.Round(dailyCalories * 0.40m / 4):N0}g (günlük kalorinin %40'ı)");
            sb.AppendLine($"- Yağ: {Math.Round(dailyCalories * 0.30m / 9):N0}g (günlük kalorinin %30'u)");
            sb.AppendLine();
            sb.AppendLine("**Günlük Öğün Önerisi:**");
            sb.AppendLine("- 🍳 **Kahvaltı:** Yumurta, tam tahıllı ekmek, peynir, domates, salatalık");
            sb.AppendLine("- 🥗 **Öğle:** Izgara tavuk/balık, bulgur pilavı, bol yeşil salata");
            sb.AppendLine("- 🍎 **Ara Öğün:** Meyve + bir avuç kuruyemiş veya yoğurt");
            sb.AppendLine("- 🍽️ **Akşam:** Sebze yemeği, protein kaynağı, az miktarda karbonhidrat");
            sb.AppendLine();
            sb.AppendLine("**Su Tüketimi:** Günde en az 2-3 litre su için.");

            return sb.ToString();
        }

        private string GetGeneralTips(AIRecommendationViewModel model)
        {
            var sb = new StringBuilder();

            sb.AppendLine("1. ✅ Egzersizlere ısınma ile başlayın, soğuma ile bitirin");
            sb.AppendLine("2. ✅ Haftada en az 150 dakika orta yoğunlukta kardiyo yapın");
            sb.AppendLine("3. ✅ Yeterli uyku alın (7-9 saat)");
            sb.AppendLine("4. ✅ Stresi yönetmek için meditasyon veya nefes egzersizleri yapın");
            sb.AppendLine("5. ✅ Düzenli olun - tutarlılık sonuç getirir");
            sb.AppendLine("6. ✅ İlerlemenizi takip edin ve gerekirse programı güncelleyin");

            if (model.BMI >= 30)
            {
                sb.AppendLine("7. ⚠️ Yüksek BMI değeriniz nedeniyle düşük etkili egzersizlerle başlamanızı öneririz (yüzme, yürüyüş)");
            }

            if (model.Age >= 50)
            {
                sb.AppendLine("7. ⚠️ Yaşınız göz önüne alındığında, eklem dostu egzersizlere öncelik verin");
            }

            return sb.ToString();
        }

        private string GenerateTransformationImageUrl(AIRecommendationViewModel model)
        {
            // Hedefe göre prompt oluştur
            var promptParts = new List<string>
    {
        "fitness transformation",
        "healthy athletic person",
        "gym motivation",
        "professional photo",
        "bright lighting"
    };

            // Cinsiyet
            if (model.Gender?.ToLower() == "erkek")
            {
                promptParts.Add("fit man");
                promptParts.Add("muscular");
            }
            else
            {
                promptParts.Add("fit woman");
                promptParts.Add("toned body");
            }

            // Hedefe göre özelleştir
            switch (model.Goal?.ToLower())
            {
                case "kilo vermek":
                    promptParts.Add("slim body");
                    promptParts.Add("weight loss success");
                    promptParts.Add("lean physique");
                    break;
                case "kilo almak":
                    promptParts.Add("healthy weight gain");
                    promptParts.Add("strong build");
                    promptParts.Add("bulking transformation");
                    break;
                case "kas yapmak":
                    promptParts.Add("bodybuilder");
                    promptParts.Add("muscle definition");
                    promptParts.Add("six pack abs");
                    break;
                case "fit kalmak":
                    promptParts.Add("athletic body");
                    promptParts.Add("cardio fitness");
                    promptParts.Add("healthy lifestyle");
                    break;
                case "esneklik kazanmak":
                    promptParts.Add("yoga pose");
                    promptParts.Add("flexible athlete");
                    promptParts.Add("stretching");
                    break;
                default:
                    promptParts.Add("fitness success");
                    break;
            }

            // Vücut tipine göre
            switch (model.BodyType?.ToLower())
            {
                case "ektomorf":
                    promptParts.Add("lean muscle");
                    break;
                case "mezomorf":
                    promptParts.Add("athletic build");
                    break;
                case "endomorf":
                    promptParts.Add("strong powerful body");
                    break;
            }

            var prompt = string.Join(", ", promptParts);
            var encodedPrompt = Uri.EscapeDataString(prompt);

            // Prompt'u da kaydet (göstermek için)
            model.ImagePrompt = prompt;

            // Pollinations.ai ücretsiz API
            return $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=512&height=512&nologo=true";
        }
    }
}