# 🛡️ Secure Coding Challenge — Kod Analiz ve Refactoring Laboratuvarı

Bu eğitim laboratuvarının temel amacı, size sunulan kod tabanını dikkatle okuyarak tasarım hatalarını, mantıksal açıkları ve eksik doğrulamaları **kendi çabanızla tespit etme ve analiz etme yeteneğinizi** geliştirmektir. 

Saldırı simülasyonları (exploit testleri) yalnızca yazdığınız düzeltmelerin (refactoring) başarısını doğrulamak amacıyla bir araç olarak kullanılacaktır. Odak noktamız **kaliteli ve güvenli kod okumak ve yazmaktır**.

---

## 📅 Sprint Görev Dosyaları

Laboratuvarda ilerlerken `docs/tasks/` klasöründeki aşağıdaki analiz dosyalarını sırasıyla inceleyin ve kod üzerinde çalışın:

### 1. [Sprint 1 — Güvenli Yazılım Kültürü ve OWASP Teorisi](file:///Users/yasin/Desktop/securecoding/tasks/docs/tasks/task1_mindset_analysis.md)
* **Odak:** OWASP standartları, zafiyet vs. tasarım hatası farkları ve yazılım yaşam döngüsünde (SDLC) güvenlik teorisi araştırması.

### 2. [Sprint 2 — Veri Doğrulama ve Güven Sınırları](file:///Users/yasin/Desktop/securecoding/tasks/docs/tasks/task2_validation_analysis.md)
* **Odak:** Dışarıdan gelen veriye duyulan aşırı güven, eksik tip/sınır kontrolleri ve veri bağlama hataları.

### 3. [Sprint 3 — Yorumlayıcı ve Enjeksiyon Güvenliği](file:///Users/yasin/Desktop/securecoding/tasks/docs/tasks/task3_injection_analysis.md)
* **Odak:** Dinamik sorgu oluşturma mantığı, regex işlemleri ve arka planda tetiklenen sistem araçlarındaki güvenlik zayıflıkları.

### 4. [Sprint 4 — İstemci Tarafı Güvenliği ve XSS](file:///Users/yasin/Desktop/securecoding/tasks/docs/tasks/task4_xss_analysis.md)
* **Odak:** Ön yüzde DOM-based XSS ve veri tabanında Stored XSS zafiyetlerinin incelenmesi, tespiti ve çözümü.

---

## 📝 Çalışma ve Teslim Metodu

Her görevde bulduğunuz problem için şu analiz adımlarını tamamlamanız beklenmektedir:

1. **Kod İncelemesi (Code Review):** Kusurlu mantığın veya eksik kontrollerin olduğu dosya adını ve satır aralıklarını belirleyin.
2. **Güvenlik Analizi:** Bu kod yapısının neden zayıflık oluşturduğunu ve risklerini Türkçe teknik terimlerle açıklayın.
3. **Güvenli Tasarım Önerisi (Opsiyonel):** Zayıflığı gidermek için nasıl bir kod düzeltmesi (refactoring) yapılması gerektiğini açıklayabilir veya kodu güvenli haliyle yeniden yazabilirsiniz.
