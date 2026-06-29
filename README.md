# 🛡️ Vulnerable E-Commerce Microservices Lab

Bu laboratuvar, modern mikroservis mimarilerinde sıklıkla yapılan güvenlik hatalarını ve tasarım açıklarını güvenli kod okuma prensipleriyle incelemek amacıyla geliştirilmiştir. Uygulama, birbirinden bağımsız çalışan .NET 8 (C#) mikroservislerinden oluşmaktadır ve önünde **Traefik API Gateway** konumlandırılmıştır.

---

## 🛠️ Gerekli Araçlar ve Ön Gereksinimler

Laboratuvarı yerel makinenizde çalıştırabilmek ve test edebilmek için aşağıdaki araçların kurulu olması gerekmektedir:

1. **Docker & Docker Compose:** Mikroservisleri ve veritabanını konteynerler halinde çalıştırmak için gereklidir.
2. **GNU Make (Makefile desteği):** Derleme, başlatma ve seed komutlarını otomatize etmek için gereklidir (macOS/Linux işletim sistemlerinde varsayılan olarak gelir; Windows kullanıcıları *make* aracını ayrıca kurabilir veya `Makefile` içerisindeki ham docker komutlarını terminalde çalıştırabilir).
3. **Postman (İsteğe Bağlı / API Testi İçin):** Servislerin girdi çıktı parametrelerini taramak ve normal akışı simüle etmek amacıyla `postman/` klasöründeki koleksiyonu import ederek kullanabilirsiniz. Alternatif olarak `curl` veya `httpie` gibi terminal HTTP araçları da kullanılabilir.

---

## 🏗️ Mimari Yapı

Uygulama tamamen Docker üzerinde izole bir ağda çalışmaktadır:
* **API Gateway (Traefik v3.4):** Giriş noktası. `http://securecoding.co` adresine gelen istekleri alt servislerin ilgili rotalarına yönlendirir.
* **Auth Service:** Kullanıcı kayıt, giriş ve kullanıcı arama işlemlerini yönetir. (Port: `8080/tcp`, Rota: `/api/auth/*`)
* **Product Service:** Ürün listeleme, arama, istatistik, içe/dışa aktarma işlemlerini yönetir. (Port: `8080/tcp`, Rota: `/api/products/*`)
* **Order Service:** Sipariş oluşturma ve filtreleme işlemlerini yürütür. (Port: `8080/tcp`, Rota: `/api/orders/*`)
* **Database (MongoDB 7):** Tüm servislerin veri saklama katmanıdır.

---

## 🚀 Kurulum Adımları

Projeyi kendi yerel makinenizde ayağa kaldırmak için aşağıdaki adımları sırasıyla uygulayın:

### 1. Yerel Domain Tanımlaması (Hosts Dosyası)
Traefik API Gateway'in gelen istekleri doğru eşleştirebilmesi için yerel DNS kayıtlarınıza domain eklemeniz gerekmektedir.
İşletim sisteminizin `hosts` dosyasını yönetici yetkisiyle açın ve aşağıdaki satırı ekleyin:

```text
127.0.0.1  securecoding.co
```
* *Dosya Konumları:* 
  * Windows: `C:\Windows\System32\drivers\etc\hosts`
  * macOS / Linux: `/etc/hosts`

### 2. Uygulamayı Başlatma
Kök dizinde yer alan `Makefile` aracılığıyla projeyi Docker üzerinde tek bir komutla derleyip başlatabilirsiniz:

```bash
# Servisleri inşa et ve arka planda ayağa kaldır
make up

# Örnek veritabanı kayıtlarını (seed data) yükle
make seed
```

---

### 3. Postman Koleksiyonunun İçe Aktarılması (Import)
API endpoint'lerini hızlıca test etmek veya istek yapılarını incelemek için:
1. **Postman** uygulamasını açın.
2. Sol üstte bulunan **Import** butonuna tıklayın.
3. Proje dizininde yer alan `docs/postman/SecureCoding_VulnEcommerce.postman_collection.json` dosyasını seçip içeri aktarın.
4. Koleksiyonun ayarlarına girip (Variables sekmesinden) `baseUrl` değişkeninin `http://securecoding.co` olarak ayarlandığından emin olun.

---

## 🎯 Laboratuvar Çalışma Kılavuzu

Proje kod tabanında herhangi bir zafiyet işareti ya da ipucu (`VULN-ID` vb.) **bulunmamaktadır**. Göreviniz kodu satır satır inceleyip tasarım ve kodlama hatalarını yakalamaktır.

Eğitim sürecindeki sprint hedeflerini, analiz yönergelerini ve incelemeniz gereken konuları görmek için lütfen ana görev kılavuzunu inceleyin:

👉 **[TASKS.md](file:///Users/yasin/Desktop/securecoding/tasks/docs/TASKS.md)**

---

## 📂 Klasör Yapısı

```
tasks/
├── compose/                        # Servislere ait Dockerfile dosyaları ve gateway yapılandırması
│   └── traefik/                    # Gateway reverse proxy yapılandırması
├── src/                            # Mikroservislerin C# kaynak kodları
│   ├── AuthService/                # Kullanıcı yönetimi ve kimlik doğrulama servisi
│   ├── OrderService/               # Sipariş yönetimi servisi
│   ├── ProductService/             # Ürün yönetimi servisi
│   └── Shared/                     # Ortak kütüphane ve modeller
├── docs/                           # Dokümantasyonlar, analizler ve Postman koleksiyonu
│   ├── TASKS.md                    # Genel laboratuvar kılavuzu
│   ├── tasks/                      # Haftalık analiz kılavuzları
│   │   ├── task1_mindset_analysis.md
│   │   ├── task2_validation_analysis.md
│   │   └── task3_injection_analysis.md
│   └── postman/                    # Laboratuvar testleri için Postman koleksiyonu
├── docker-compose.yml              # Konteyner orkestrasyon dosyası
├── Makefile                        # Proje yönetim komutları
└── README.md                       # Bu doküman
```

---

## 🛠️ Yararlı Otomasyon Komutları

Kök dizinde çalıştırabileceğiniz temel Makefile komutları:
* `make up` — Docker konteynerlerini ayağa kaldırır ve Traefik gateway'i hazırlar.
* `make down` — Tüm konteynerleri ve ağları kapatır.
* `make restart` — Kod değişikliklerini uygulamak için servisleri yeniden derler ve başlatır.
* `make seed` — Başlangıç için örnek kullanıcılar ve ürünler ekler.
* `make status` — Çalışan servislerin port ve sağlık durumu raporunu verir.
