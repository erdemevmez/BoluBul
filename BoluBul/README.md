# BoluBul

BoluBul, Bolu’daki restoran, kafe, oto servis, kuaför, sağlık, eğitim, mağaza, konaklama ve benzeri yerel işletmeleri tek platformda toplayan ASP.NET Core MVC tabanlı yerel işletme rehberi MVP’sidir.

## Kullanılan teknolojiler

- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL
- Npgsql.EntityFrameworkCore.PostgreSQL
- ASP.NET Core Identity
- Role-based authorization
- Bootstrap 5
- Özel CSS (`wwwroot/css/bolubul.css`)
- Local dosya yükleme (`wwwroot/uploads`)

## Kurulum

1. PostgreSQL kurulu ve çalışır durumda olmalıdır.
2. `appsettings.json` içindeki `DefaultConnection` kendi yerel PostgreSQL bilgilerinizle güncellenmelidir.
3. Migration ve database update komutları çalıştırılmalıdır.
4. Uygulama `dotnet run` ile başlatılmalıdır.

Örnek connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=BoluBulDb;Username=postgres;Password=Postgres123"
}
```

`Postgres123` sadece örnek şifredir. Kendi PostgreSQL şifrenizi kullanın; gerçek şifreleri public commit etmeyin.

## Migration komutları

Kod derlendikten sonra PostgreSQL hazırken çalıştırın:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Varsayılan admin

- Email: `admin@bolubul.com`
- Password: `Admin123*`

## Roller

- Admin
- BusinessOwner
- User

## Proje klasör yapısı

- `Areas/Admin`: Yönetim paneli, işletme, kategori, yorum ve kullanıcı yönetimi
- `Areas/Owner`: İşletme sahibi paneli, işletme CRUD, galeri ve çalışma saatleri
- `Controllers`: Public MVC controller’ları
- `Data`: `ApplicationDbContext` ve `SeedData`
- `Models`: Entity modelleri
- `ViewModels`: Public, Admin ve Owner view modelleri
- `Repositories`: Generic, business ve category repository katmanı
- `Services/Interfaces`: Servis sözleşmeleri
- `Services/Implementations`: İş kuralları ve uygulama servisleri
- `wwwroot/css/bolubul.css`: BoluBul yeşil tema tasarımı
- `wwwroot/uploads`: Local yükleme klasörleri

## MVP özellikleri

- Modern BoluBul ana sayfası
- İşletme arama, kategori ve ilçe filtresi
- İşletme detay sayfası
- Telefon, WhatsApp ve yol tarifi tıklama istatistikleri
- Favori ekleme/çıkarma
- Yorum oluşturma ve admin onayı
- Admin paneli
- İşletme sahibi paneli
- Logo, kapak ve galeri yükleme altyapısı
- Çalışma saatleri düzenleme
- Bolu, ilçeler, kategoriler ve test işletmeleri seed verisi
- Light/dark tema desteği

## Sonraki aşama

- İşletme sahiplenme sistemi
- Gerçek işletme başvuru akışı
- Harita entegrasyonu
- Gelişmiş arama ve filtreleme
- Sayfalama
- SEO `sitemap.xml`
- Profil doğrulama rozeti
- Kampanya/duyuru sistemi
- Sponsorlu işletme
- QR kodlu işletme profili
- Bildirim sistemi
- İletişim formu
- Şikayet / bilgi düzeltme bildirimi
