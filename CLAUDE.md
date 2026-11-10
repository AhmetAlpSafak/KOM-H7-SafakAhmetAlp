# KOM-H7-SafakAhmetAlp

Karmaşık Oyun Mekanikleri dersi, Hafta 7 ödevi. Unity 6000.1.6f1, Universal 2D (URP), yeni Input System.
Öğrenci: Ahmet Alp Şafak. Sohbet dili Türkçe; kod adları ve yorumlar da Türkçe.

## Harita
- `Assets/Scenes/` — A_TekTus, B_HareketHissi, C_Carpisma, D_Birlesik (asıl oyun: Sisifos)
- `Assets/Scripts/A|B|C|D/` — oyun betikleri; `Assets/Scripts/Editor/SahneKurucu.cs` sahneleri üretir
- `Assets/Prefabs`, `Assets/Sprites`, `Assets/Fizik` — kurucunun ürettiği varlıklar
- `Build/D_Birlesik/` — Windows build (depoya girer); `Belge/` — tasarım belgesi ve video

## Kurallar
- Girdi `Update`'te, Rigidbody2D'ye dokunan fizik `FixedUpdate`'te. `rb.linearVelocity` kullan.
- Her MonoBehaviour kendi dosyasında, dosya adı = sınıf adı.
- Kod sınıfta savunulacak: basit, okunur, her ayar `[SerializeField]` ile Inspector'da. Karmaşık hile yok.
- Açıklama satırı zorunlu: her alanın yanına ne işe yaradığını, her metodun üstüne ne yaptığını Türkçe yorumla yaz. Yeni eklenen kod da aynı şekilde yorumlanır.
- D'deki üç bağı koru: koşu hızı → itiş gücü, omuz zamanlaması → itiş kalitesi, kırılan engel → enkaz (ivme/fren düşer).
- `KOM/Tum Sahneleri Kur` menüsü sahneleri baştan üretir ve Inspector ayarlarını siler. Öğrenci ayar yapmaya başladıysa çalıştırmadan önce sor.
- Değişiklikten sonra derle, Console'da hata olmadığını kontrol et.
- Yapay zekâ beyanı zorunlu: anlamlı her değişikliği `Belge/YZ_Beyani.md` dosyasına bir satırla ekle (tarih, ne yapıldı, hangi dosyalar).

## Komutlar
- Unity editörü: `unity status`, `unity command` (komut listesi). Proje açıkken sahne ve varlık dosyalarını elle düzenleme.
- Git: `Library/ Temp/ Obj/ Logs/ UserSettings/` asla commit edilmez. Teslimde son commit'e `h7-teslim` etiketi.
