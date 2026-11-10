# KOM-H7 · Üç Mekanik, Dört Oyun

Karmaşık Oyun Mekanikleri, Hafta 7 ödevi. Ahmet Alp Şafak.

## Açma
1. Unity Hub → Add → bu klasörü seç. Sürüm: **Unity 6000.1.6f1**.
2. `Assets/Scenes` içinden bir sahneyi aç, Play'e bas.
3. Oyun D'nin Windows sürümü: `Build/D_Birlesik/` içindeki `.exe`.

## Sahneler ve kontroller

| Sahne | Oyun | Kontroller |
|---|---|---|
| `A_TekTus` | Tek tuş: engellerin üstünden atla, uzun engellerde süzül | Boşluk/tık: zıpla · Düşerken basılı tut: süzül · Kaybedince bas: baştan |
| `B_HareketHissi` | Kısa parkur, iki ayar profili (Taş, Tüy) | A/D: yürü · Boşluk: zıpla (tutma süresi = yükseklik) · E: profil · R: baştan |
| `C_Carpisma` | Breakout: normal, iki vuruşluk ve patlayan tuğla | A/D: raket · Boşluk: fırlat · R: baştan |
| `D_Birlesik` | **Sisifos**: kayayı yokuş yukarı taşı | A/D: yürü · Boşluk: kaya uzaktaysa zıpla, yakındaysa omuz at · R: baştan |

## Sisifos'ta bağlar
- **Hareket → Çarpışma:** omuz anındaki koşu hızı kayaya verilen itişi büyütür.
- **Tek tuş → Çarpışma:** kayaya doğru mesafede basmak tam itiş, kötü zamanlama zayıf itiş verir.
- **Çarpışma → Hareket:** kayanın kırdığı engeller enkaz bırakır; enkaz üstünde ivme ve fren düşer.

Ayrıntılar: `Belge/H7_TasarimBelgesi.pdf`.
