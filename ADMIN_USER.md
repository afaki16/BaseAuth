# Admin Kullanıcısı

Bu proje için otomatik olarak oluşturulan admin kullanıcısı bilgileri aşağıdadır:

## Admin Kullanıcı Bilgileri

- **Email:** admin@baseauth.com
- **Şifre:** Admin123!
- **Ad:** System
- **Soyad:** Administrator
- **Telefon:** +905551234567
- **Durum:** Aktif
- **Email Onaylı:** Evet
- **Telefon Onaylı:** Evet

## Admin Rolü

Admin kullanıcısı "Admin" rolüne sahiptir ve aşağıdaki tüm izinlere sahiptir:

### Kullanıcı İzinleri
- Kullanıcı oluşturma
- Kullanıcı görüntüleme
- Kullanıcı güncelleme
- Kullanıcı silme
- Kullanıcı yönetimi

### Rol İzinleri
- Rol oluşturma
- Rol görüntüleme
- Rol güncelleme
- Rol silme
- Rol yönetimi

### İzin Yönetimi
- İzin oluşturma
- İzin görüntüleme
- İzin güncelleme
- İzin silme
- İzin yönetimi

### Sistem İzinleri
- Sistem yönetimi
- Log görüntüleme
- Ayarları yönetme

## Güvenlik Notları

1. **İlk Giriş:** Uygulama ilk çalıştırıldığında admin kullanıcısı otomatik olarak oluşturulur.
2. **Şifre Değiştirme:** Güvenlik için admin kullanıcısının şifresini ilk girişten sonra değiştirmeniz önerilir.
3. **Sistem Rolü:** Admin rolü sistem rolü olarak işaretlenmiştir ve silinemez.

## API Kullanımı

Admin kullanıcısı ile giriş yapmak için:

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@baseauth.com",
  "password": "Admin123!"
}
```

## Seed Data

Admin kullanıcısı ve ilgili veriler `SeedData.cs` dosyasında tanımlanmıştır ve uygulama başlatıldığında otomatik olarak çalıştırılır.

## Değişiklikler

**GUID'den INT'e Geçiş:**
- Tüm entity'lerde ID alanları GUID'den INT'e çevrildi
- Foreign key'ler INT olarak güncellendi
- Repository'ler ve service'ler INT ID'leri kullanacak şekilde güncellendi
- DTO'lar ve Command/Query'ler INT ID'leri kullanacak şekilde güncellendi
- JWT token'larda user ID'ler INT olarak saklanıyor 