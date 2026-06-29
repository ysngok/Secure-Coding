# 📅 Sprint 3 — Yorumlayıcı ve Enjeksiyon Güvenliği (Injection)

Bu sprintte amacımız, kullanıcıdan (istemciden) alınan girdilerin sunucu tarafındaki yorumlayıcılara (veritabanı sorgu motorları, regex motoru veya işletim sistemi kabuğu) doğrudan veya güvensiz bir şekilde geçilmesinden kaynaklanan enjeksiyon zayıflıklarını kod okuma yoluyla tespit etmektir.

---

## 🔍 İnceleme ve Analiz Konuları

### Görev 3.1: NoSQL ve Sorgu Motoru Enjeksiyonları
* **Analiz Konusu:** Veritabanı sorgularının (sorgu filtreleri, aggregate yapıları) inşa edildiği kod bloklarını ve servisleri satır satır okuyun.
* **Analiz Soruları:**
  * Kullanıcı girdilerinin (kullanıcı adı, şifre veya arama kelimeleri) MongoDB filtre nesneleri (`BsonDocument`) içine doğrudan veya güvensiz yöntemlerle yerleştirildiği yerleri bulun. Bu durum kimlik doğrulamayı atlatmaya (auth bypass) nasıl yol açabilir?
  * Veritabanında dinamik JavaScript kodu çalıştırılmasına izin veren fonksiyonları (örneğin `$where` kullanımı) arayın. Kullanıcı girdisinin bu JS kodunun içine doğrudan eklenip eklenmediğini inceleyin.
  * Agregasyon (`Aggregation`) boru hatlarının (pipeline) dinamik dizelerle oluşturulduğu kod bloklarını bulun. Dize birleştirme (string concatenation) yoluyla sorgunun mantığının nasıl değiştirilebileceğini analiz edin.

### Görev 3.2: İşletim Sistemi Komut Enjeksiyonu (OS Command Injection)
* **Analiz Konusu:** Sunucu tarafında dış süreç (external process) başlatan, sistem komutları koşturan (`Process.Start` vb.) veya shell (kabuk) çalıştıran servis katmanlarını tarayın.
* **Analiz Soruları:**
  * İstemcinin gönderdiği dosya adları, dış parametreler veya parametrik girdiler, işletim sistemi komut satırına (örneğin `/bin/bash` argümanlarına) sanitize edilmeden veya kaçış karakteri eklenmeden doğrudan dize birleştirme ile geçiliyor mu?
  * Güvenilmeyen bir girdinin işletim sistemi seviyesinde rastgele komutlar çalıştırmasına (RCE) yol açabilecek kod bloklarını tespit edin.

### Görev 3.3: Düzenli İfade Enjeksiyonu (Regex Injection & ReDoS)
* **Analiz Konusu:** Kullanıcının arama yapmak veya veri doğrulamak için gönderdiği girdilere dayalı düzenli ifadelerin (Regular Expressions) dinamik olarak derlendiği yerleri bulun.
* **Analiz Soruları:**
  * Kullanıcının doğrudan kendi regex desenlerini sunucuya gönderebildiği veya arama sorgusunun direkt olarak `Regex` nesnesine parametre olarak geçtiği yerler var mı?
  * Kötü niyetli ve karmaşık bir regex deseninin sunucu işlemcisini kilitlemesi (Regular Expression Denial of Service - ReDoS) durumuna karşı kodda herhangi bir zaman aşımı (Timeout) mekanizması kullanılmış mı?
