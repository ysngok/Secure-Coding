# 📅 Sprint 2 — Girdi Doğrulama ve Güven Sınırları (Input Validation)

Bu sprintte amacımız, sunucuya dışarıdan gelen verilerin (girdilerin) hangi noktalarda kontrolsüzce içeri alındığını ve güven sınırlarının (Trust Boundary) nasıl ihlal edildiğini kod okuyarak tespit etmektir.

---

## 🔍 İnceleme ve Analiz Konuları

### Görev 2.1: Veri Tipi, Uzunluk ve Sınır Kontrolleri
* **Analiz Konusu:** Proje genelindeki veri alma modellerini (klasörler altındaki model sınıfları) ve veritabanı kayıt işlemlerini okuyun. 
* **Analiz Soruları:**
  * Kullanıcıların sisteme kaydolurken gönderdikleri e-posta veya şifre formatlarında herhangi bir kısıt kontrolü var mı? Yoksa her gelen veri kabul ediliyor mu?
  * Ürün veya sipariş oluşturma süreçlerinde, sayısal alanların (fiyat, miktar vb.) negatif veya mantıksız değerler alamamasını garanti eden bir kontrol mekanizması kodda mevcut mu?
  * Çok uzun metinler gönderildiğinde sunucu belleğini veya veritabanını şişirebilecek (DoS riski taşıyan) eksik uzunluk kontrolleri hangi alanlarda bulunuyor?

### Görev 2.2: Mass Assignment (Over-Posting / Aşırı Veri Bağlama)
* **Analiz Konusu:** İstemcinin gönderdiği JSON nesneleriyle doğrudan eşlenen API modellerini inceleyin.
* **Analiz Soruları:**
  * İstemcinin göndermemesi gereken ancak veritabanı şemasında bulunan bazı kritik alanlar (örneğin rol, yetki veya yönetici bayrakları) doğrudan istemci payload'ından okunup veritabanına yazılıyor mu?
  * Geliştirici, DTO (Data Transfer Object) kullanmak yerine doğrudan veritabanı varlık modelini mi API parametresi olarak içeri almış?

### Görev 2.3: Filtre ve Mantıksal Parametre Güvenliği
* **Analiz Konusu:** Veritabanından veri çeken veya filtreleme yapan controller metotlarını inceleyin.
* **Analiz Soruları:**
  * İstemcinin gönderdiği JSON veya filtre parametreleri, sunucu tarafında hiçbir parse veya sanitizasyon işlemine uğramadan doğrudan sorgu motoruna teslim ediliyor mu?
  * Bu durum veritabanındaki verilerin sınırlarını aşarak yetkisiz verilere ulaşmaya nasıl imkan tanıyabilir?
