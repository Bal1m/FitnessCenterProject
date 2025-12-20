# 🏋️ FitLife - Spor Salonu Yönetim ve Randevu Sistemi

<div align="center">

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)
![Google Gemini](https://img.shields.io/badge/Google%20Gemini-AI-4285F4?style=for-the-badge&logo=google&logoColor=white)

**Modern ve kullanıcı dostu bir spor salonu yönetim sistemi**

[Özellikler](#-özellikler) •
[Teknolojiler](#-teknolojiler) •
[Kurulum](#-kurulum) •
[API](#-rest-api) •
[Ekran Görüntüleri](#-ekran-görüntüleri)

</div>

---

## 📋 Proje Hakkında

FitLife, ASP.NET Core MVC kullanılarak geliştirilmiş kapsamlı bir **Spor Salonu Yönetim ve Randevu Sistemi**'dir. Sistem, spor salonlarının sunduğu hizmetleri, eğitmenlerin uzmanlık alanlarını, üyelerin randevularını ve **yapay zeka tabanlı** egzersiz önerilerini yönetebilecek şekilde tasarlanmıştır.

### 🎯 Projenin Amacı

- Spor salonu hizmetlerinin etkin yönetimi
- Eğitmen ve müsaitlik takibi
- Online randevu sistemi ile çakışma kontrolü
- Yapay zeka destekli kişiselleştirilmiş fitness önerileri
- REST API ile veri erişimi ve raporlama

---

## ✨ Özellikler

### 👤 Kullanıcı Yönetimi
- ✅ Rol tabanlı yetkilendirme (Admin ve Üye rolleri)
- ✅ Güvenli kayıt ve giriş sistemi (ASP.NET Core Identity)
- ✅ Kullanıcı profil yönetimi
- ✅ Kullanıcı aktif/pasif durumu kontrolü

### 🏃 Hizmet Yönetimi
- ✅ Fitness, Yoga, Pilates, Kick Boks, Yüzme gibi hizmetler
- ✅ Hizmet süresi ve ücret tanımlama
- ✅ CRUD işlemleri (Ekleme, Listeleme, Güncelleme, Silme)

### 👨‍🏫 Eğitmen Yönetimi
- ✅ Eğitmen profilleri ve uzmanlık alanları
- ✅ Çoklu hizmet atama (bir eğitmen birden fazla hizmet verebilir)
- ✅ Müsaitlik saatleri tanımlama (gün ve saat bazlı)

### 📅 Randevu Sistemi
- ✅ Hizmet → Eğitmen → Tarih → Saat adımlarıyla randevu oluşturma
- ✅ **Otomatik çakışma kontrolü** (aynı eğitmen, aynı saat)
- ✅ Randevu durumları: Beklemede, Onaylandı, Reddedildi, Tamamlandı, İptal
- ✅ Admin tarafından onay/red mekanizması
- ✅ AJAX ile dinamik saat seçimi

### 🤖 Yapay Zeka Entegrasyonu
- ✅ Boy, kilo, yaş, cinsiyet, vücut tipi bilgisi girişi
- ✅ BMI hesaplama ve kategori belirleme
- ✅ **Google Gemini API** ile kişiselleştirilmiş egzersiz ve diyet önerileri
- ✅ **Pollinations.ai** ile hedef vücut görseli oluşturma
- ✅ Fallback sistem: API hata verirse kural tabanlı öneri

### 📊 Admin Panel
- ✅ Dashboard ile istatistikler
- ✅ Hizmet, Eğitmen, Randevu, Kullanıcı yönetimi
- ✅ Gelir raporları
- ✅ Son randevular ve aktiviteler

### 🔌 REST API
- ✅ Eğitmen listeleme ve filtreleme
- ✅ Randevu sorgulama
- ✅ İstatistik ve gelir raporları
- ✅ LINQ ile gelişmiş sorgular

---

## 🛠 Teknolojiler

| Teknoloji | Açıklama |
|-----------|----------|
| **ASP.NET Core MVC 8.0** | Web uygulama framework |
| **C#** | Ana programlama dili |
| **PostgreSQL** | İlişkisel veritabanı |
| **Entity Framework Core** | ORM (Object-Relational Mapping) |
| **ASP.NET Core Identity** | Kimlik doğrulama ve yetkilendirme |
| **Bootstrap 5** | Responsive CSS framework |
| **jQuery** | JavaScript kütüphanesi (AJAX) |
| **Google Gemini API** | Yapay zeka metin önerileri |
| **Pollinations.ai** | AI görsel oluşturma (ücretsiz) |

---

## 📁 Proje Yapısı

```
FitnessCenterProject/
├── 📂 Controllers/
│   ├── AccountController.cs      # Kimlik doğrulama
│   ├── AdminController.cs        # Admin panel işlemleri
│   ├── AIController.cs           # Yapay zeka önerileri
│   ├── AppointmentController.cs  # Randevu işlemleri
│   ├── HomeController.cs         # Ana sayfa
│   └── Api/
│       └── ReportApiController.cs # REST API
├── 📂 Models/
│   ├── Entities/                 # Veritabanı modelleri
│   ├── ViewModels/               # View modelleri
│   └── Enums/                    # Sabit değerler
├── 📂 Views/
│   ├── Account/                  # Giriş, Kayıt, Profil
│   ├── Admin/                    # Admin panel sayfaları
│   ├── AI/                       # AI öneri sayfası
│   ├── Appointment/              # Randevu sayfaları
│   ├── Home/                     # Ana sayfa, Hizmetler, Eğitmenler
│   └── Shared/                   # Layout, Partial views
├── 📂 Data/
│   ├── ApplicationDbContext.cs   # Veritabanı context
│   └── DbInitializer.cs          # Seed data
├── 📂 Migrations/                # EF Core migrations
├── Program.cs                    # Uygulama başlangıç
└── appsettings.json              # Yapılandırma
```

---

## 🚀 Kurulum

### Gereksinimler

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) veya [VS Code](https://code.visualstudio.com/)

### Adım 1: Projeyi Klonlayın

```bash
git clone https://github.com/Bal1m/FitnessCenterProject.git
cd FitnessCenterProject
```

### Adım 2: Veritabanı Ayarları

`appsettings.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FitnessCenterProject;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY"
  }
}
```

### Adım 3: Veritabanını Oluşturun

```bash
dotnet ef database update
```

### Adım 4: Uygulamayı Çalıştırın

```bash
dotnet run
```

Tarayıcıda açın: `https://localhost:7143` veya `http://localhost:5027`

---

## 👤 Varsayılan Kullanıcılar

| Rol | E-posta | Şifre |
|-----|---------|-------|
| **Admin** | B231210083@sakarya.edu.tr | sau |
| **Üye** | Kayıt olarak oluşturabilirsiniz | - |

---

## 🔌 REST API

### Base URL
```
/api/ReportApi
```

### Eğitmen Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/trainers` | Tüm eğitmenleri listele |
| `GET` | `/trainers/{id}` | Eğitmen detayı |
| `GET` | `/trainers/available?date=2025-01-15` | Tarihe göre müsait eğitmenler |
| `GET` | `/trainers/by-specialization?spec=yoga` | Uzmanlığa göre filtrele |

### Randevu Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/appointments/user/{userId}` | Kullanıcı randevuları |
| `GET` | `/appointments/by-date?date=2025-01-15` | Tarihe göre randevular |
| `GET` | `/appointments/by-status?status=Pending` | Duruma göre randevular |

### İstatistik Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| `GET` | `/statistics` | Genel istatistikler |
| `GET` | `/revenue?startDate=2025-01-01&endDate=2025-01-31` | Gelir raporu |

### Örnek API Yanıtı

```json
{
  "totalMembers": 150,
  "totalTrainers": 4,
  "totalServices": 6,
  "totalAppointments": 320,
  "totalRevenue": 85000,
  "topServices": [
    { "name": "Fitness", "count": 120 },
    { "name": "Yoga", "count": 80 }
  ]
}
```

---

## 🗄 Veritabanı Şeması

### Tablolar

| Tablo | Açıklama |
|-------|----------|
| `AspNetUsers` | Kullanıcılar (Identity genişletilmiş) |
| `Services` | Hizmetler |
| `Trainers` | Eğitmenler |
| `TrainerServices` | Eğitmen-Hizmet ilişkisi (M:N) |
| `TrainerAvailabilities` | Eğitmen müsaitlik saatleri |
| `Appointments` | Randevular |
| `GymSettings` | Salon ayarları |

### Entity İlişkileri

```
User (1) ──────< (N) Appointment
Service (1) ────< (N) Appointment
Trainer (1) ────< (N) Appointment
Trainer (M) ────< (N) Service (via TrainerServices)
Trainer (1) ────< (N) TrainerAvailability
```

---

## 📸 Ekran Görüntüleri

### Ana Sayfa
![Ana Sayfa](screenshots/home.png)

### Admin Dashboard
![Admin Dashboard](screenshots/admin-dashboard.png)

### Randevu Oluşturma
![Randevu](screenshots/appointment.png)

### AI Öneri
![AI Öneri](screenshots/ai-recommendation.png)

> **Not:** Ekran görüntülerini `screenshots/` klasörüne ekleyebilirsiniz.

---

## 🤖 Yapay Zeka Özellikleri

### Google Gemini API
- Kullanıcı bilgilerine göre kişiselleştirilmiş egzersiz programı
- Günlük kalori hesaplama
- Makro besin dağılımı önerisi
- Haftalık antrenman planı

### Pollinations.ai
- Hedef vücuda göre motivasyon görseli oluşturma
- Ücretsiz ve API key gerektirmez

### Fallback Sistemi
- API hata verirse kural tabanlı öneri sistemi devreye girer
- Kullanıcı her durumda öneri alır

---

## 🔒 Güvenlik Özellikleri

- ✅ ASP.NET Core Identity ile güvenli kimlik doğrulama
- ✅ Rol tabanlı yetkilendirme (`[Authorize]` attribute)
- ✅ CSRF koruması (`ValidateAntiForgeryToken`)
- ✅ SQL Injection koruması (Entity Framework parametreli sorgular)
- ✅ XSS koruması (Razor otomatik encoding)
- ✅ Input validation (Client + Server side)

---

## 📝 Geliştirici Notları

### Migration Oluşturma
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Paket Yükleme
```bash
dotnet restore
```

### Build
```bash
dotnet build
```

### Test Çalıştırma
```bash
dotnet run
```

---

## 🤝 Katkıda Bulunma

1. Bu repo'yu fork edin
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -m 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/YeniOzellik`)
5. Pull Request açın

---

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

---

## 👨‍💻 Geliştirici

**Murat Haktan BALIM**

- 📧 E-posta: B231210083@sakarya.edu.tr
- 🎓 Sakarya Üniversitesi - Bilgisayar Mühendisliği
- 🔗 GitHub: [@Bal1m](https://github.com/Bal1m)

---

## 🙏 Teşekkürler

- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Bootstrap](https://getbootstrap.com/)
- [Google Gemini](https://ai.google.dev/)
- [Pollinations.ai](https://pollinations.ai/)
- [PostgreSQL](https://www.postgresql.org/)

---

<div align="center">

**⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!**

Made with ❤️ in Sakarya, Turkey

</div>
