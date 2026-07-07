# 📅 Sprint 4 — İstemci Tarafı Güvenliği ve XSS (Cross-Site Scripting)

Bu sprintte amacımız, kullanıcıdan alınan verilerin tarayıcı tarafında güvensiz bir şekilde render edilmesinden kaynaklanan **Siteler Arası Betik Çalıştırma (XSS - Cross-Site Scripting)** zafiyetlerini incelemek, tespit etmek ve güvenli kodlama pratikleriyle çözmektir. Uygulamamızda şu an toplam **9 farklı XSS zayıflık noktası** bulunmaktadır.

---

## 🔍 İnceleme ve Analiz Konuları

### Görev 4.1: Yansıtılan ve DOM Tabanlı XSS (Reflected & DOM-based XSS)
* **Analiz Konusu:** Uygulamadaki ön uç DOM manipülasyonu ve sunucu tarafı yansıtma yapan 3 farklı vektörü inceleyin:
  - **Vektör 1 (DOM XSS - Search Query):** `src/Frontend/app.js` içerisindeki arama parametresinin (`q`) URL'den alınıp `#search-results-text` içerisine doğrudan `.innerHTML` ile yansıtılması.
  - **Vektör 2 (DOM XSS - Category Hash):** URL'deki `#category=...` hash parametresinin izlenip `#category-filter-text` içerisine doğrudan `.innerHTML` ile yazdırılması.
  - **Vektör 3 (Reflected XSS - Echo Service):** Product API servisindeki `ProductController.cs` içerisinde yer alan `/api/products/echo?message=...` ucuna gönderilen girdinin HTML formatında doğrudan istemciye geri dönülmesi.
* **Analiz Soruları:**
  - Vektör 1 ve Vektör 2'yi tetiklemek için sırasıyla nasıl payloadlar kullanılabilir? (Örnek: `?q=<svg onload=alert(1)>`)
  - Vektör 3'ü tetiklemek amacıyla tarayıcıda `/api/products/echo?message=<script>alert('echo')</script>` çağrısı yapıldığında tarayıcı neden ve nasıl bu kodu çalıştırır?
  - Yansıtılan XSS zafiyetlerini gidermek için backend tarafında HTML Encode işlemi nasıl uygulanmalıdır?

---

### Görev 4.2: Depolanan XSS (Stored XSS)
* **Analiz Konusu:** Ürünlerin listelendiği kart yapılarını (`renderProducts` fonksiyonu), ürün detay modalını (`openProductDetails` fonksiyonu) ve yorumlar alanını inceleyin. Burada 6 farklı Depolanan (Stored) XSS vektörü yer almaktadır:
  - **Vektör 4 (Stored XSS - Product Name):** Ürün adının hem ana sayfadaki ürün kartlarında hem de detay modalında `.innerHTML` ile render edilmesi.
  - **Vektör 5 (Stored XSS - Product Description):** Ürün açıklamasının ürün kartlarında ve detay sayfasında `.innerHTML` ile render edilmesi.
  - **Vektör 6 (Stored XSS - Product Category):** Ürün kategorisinin detay sayfasında `.innerHTML` ile render edilmesi.
  - **Vektör 7 (Stored XSS - Comment Username):** Müşteri yorumunu yazan kullanıcının isminin yorum kartında `.innerHTML` ile render edilmesi.
  - **Vektör 8 (Stored XSS - Comment Content):** Müşteri yorumu içeriğinin yorum kartında `.innerHTML` ile render edilmesi.
  - **Vektör 9 (Attribute-based Stored XSS - Image URL):** Ürün görsel URL'sinin (`imageUrl`) `<img>` taginin `src` özniteliğine (`src="${imageUrl}"`) kaçış karakteri olmadan yazılması.
* **Analiz Soruları:**
  - Vektör 9 (Öznitelik Tabanlı XSS) durumunda, saldırganın `imageUrl` alanına yazacağı `x" onerror="alert('breakout')` girdisinin HTML yapısını nasıl kırdığını ve event handler'ı nasıl çalıştırdığını açıklayın.
  - Yorumlar alanına bırakılan kötü niyetli bir yorum (Vektör 8), siteyi ziyaret edip o ürüne bakan diğer kullanıcıların tarayıcısında nasıl bir risk oluşturur? Çerezlerin (Cookies) veya localStorage alanının çalınması senaryosunu değerlendirin.
  - Stored XSS zafiyetini önlemek için hem JS frontend tarafında hem de .NET Core backend tarafında hangi kod düzeltmeleri (sanitasyon/encoding) yapılmalıdır?

---

## 📝 Çözüm Yönergeleri & Öneriler

### 1. Ön Yüz (JavaScript) Düzeltmeleri
DOM'a veri bağlarken asla `.innerHTML` doğrudan kullanıcı verisiyle kullanılmamalıdır. Düz metin veriler için tarayıcının veriyi kod olarak çalıştırmasını engelleyen alternatifler tercih edilmelidir:
- `.textContent` veya `.innerText` kullanımı.
- Eğer HTML etiketlerine izin verilmek zorundaysa (örneğin zengin metin editörleri), girdiler render edilmeden önce güvenilir bir sanitasyon kütüphanesi (örn. `DOMPurify`) ile temizlenmelidir.

### 2. Arka Yüz (.NET Core) Düzeltmeleri
Veritabanına kaydedilen veriler sunucuda saklanmadan önce veya API çıkışında encode edilebilir.
- HTML kodlarını etkisiz kılmak için `System.Text.Encodings.Web.HtmlEncoder.Default.Encode()` metodundan faydalanılabilir.
- Alternatif olarak, sunucu tarafında gelen girdileri temizlemek için `Ganss.Xss.HtmlSanitizer` gibi popüler NuGet paketleri kullanılabilir.
