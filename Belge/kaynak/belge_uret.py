# H7 tasarım belgesini (Belge/H7_TasarimBelgesi.pdf) üreten betik.
# Çalıştırma (proje kökünden):  py Belge/kaynak/belge_uret.py
# Oyun testi notunu doldurmak için aşağıdaki ESLI_TEST listesini düzenleyip yeniden çalıştır.

from pathlib import Path
from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.units import cm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (SimpleDocTemplate, Paragraph, Spacer, Table,
                                TableStyle, KeepTogether)
from reportlab.graphics.shapes import Drawing, Rect, String, Line, Polygon

# ---------- Dosya yolları ----------
KOK = Path(__file__).resolve().parents[2]          # proje kökü
CIKTI = KOK / "Belge" / "H7_TasarimBelgesi.pdf"   # üretilecek PDF

# ---------- Yazı tipi: Türkçe karakterler için Arial ----------
pdfmetrics.registerFont(TTFont("Arial", r"C:\Windows\Fonts\arial.ttf"))
pdfmetrics.registerFont(TTFont("Arial-B", r"C:\Windows\Fonts\arialbd.ttf"))
pdfmetrics.registerFont(TTFont("Arial-I", r"C:\Windows\Fonts\ariali.ttf"))
pdfmetrics.registerFontFamily("Arial", normal="Arial", bold="Arial-B", italic="Arial-I")

# ---------- Biçemler ----------
GOVDE = ParagraphStyle("govde", fontName="Arial", fontSize=9.5, leading=12.5, spaceAfter=4)
BASLIK = ParagraphStyle("baslik", fontName="Arial-B", fontSize=17, leading=21, spaceAfter=2)
ALT = ParagraphStyle("alt", fontName="Arial", fontSize=9, leading=12, textColor=colors.HexColor("#555555"))
H1 = ParagraphStyle("h1", fontName="Arial-B", fontSize=12.5, leading=16, spaceBefore=10, spaceAfter=4)
H2 = ParagraphStyle("h2", fontName="Arial-B", fontSize=10, leading=13, spaceBefore=6, spaceAfter=3)
HUCRE = ParagraphStyle("hucre", fontName="Arial", fontSize=8, leading=10)
HUCRE_B = ParagraphStyle("hucreb", fontName="Arial-B", fontSize=8, leading=10)

GRI = colors.HexColor("#E6E6E6")
CIZGI = colors.HexColor("#999999")


def p(metin, stil=GOVDE):
    """Kısa yol: metinden paragraf üretir."""
    return Paragraph(metin, stil)


def tablo(satirlar, genislikler):
    """İlk satırı başlık olan, hücreleri kendiliğinden kaydırılan tablo üretir."""
    veri = [[Paragraph(str(h), HUCRE_B) for h in satirlar[0]]]
    veri += [[Paragraph(str(h), HUCRE) for h in s] for s in satirlar[1:]]
    t = Table(veri, colWidths=[g * cm for g in genislikler], repeatRows=1)
    t.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), GRI),
        ("GRID", (0, 0), (-1, -1), 0.4, CIZGI),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("TOPPADDING", (0, 0), (-1, -1), 2),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 2),
        ("LEFTPADDING", (0, 0), (-1, -1), 3),
        ("RIGHTPADDING", (0, 0), (-1, -1), 3),
    ]))
    return t


def ok(d, x1, y1, x2, y2):
    """Şemaya (x1,y1)'den (x2,y2)'ye uçlu ok çizer."""
    d.add(Line(x1, y1, x2, y2, strokeWidth=1.2))
    import math
    a = math.atan2(y2 - y1, x2 - x1)
    u = 7  # ok ucu boyu
    d.add(Polygon([x2, y2,
                   x2 - u * math.cos(a - 0.4), y2 - u * math.sin(a - 0.4),
                   x2 - u * math.cos(a + 0.4), y2 - u * math.sin(a + 0.4)],
                  fillColor=colors.black, strokeWidth=0))


def kutu(d, x, y, g, y_, ust, alt):
    """Şemaya iki satırlı mekanik kutusu çizer."""
    d.add(Rect(x, y, g, y_, fillColor=GRI, strokeColor=colors.black, strokeWidth=1))
    d.add(String(x + g / 2, y + y_ - 15, ust, fontName="Arial-B", fontSize=10, textAnchor="middle"))
    d.add(String(x + g / 2, y + 7, alt, fontName="Arial", fontSize=7.5, textAnchor="middle"))


def bag_semasi():
    """D'deki üç bağın kutu-ok şeması."""
    d = Drawing(17 * cm, 6 * cm)  # üstteki geri dönüş oku ve yazısı sığsın
    kutu(d, 10, 95, 130, 42, "Hareket hissi", "koşu hızı, ivme, fren")
    kutu(d, 10, 8, 130, 42, "Tek tuş (Boşluk)", "zıpla / omuz kararı")
    kutu(d, 340, 50, 135, 42, "Çarpışma (omuz-kaya)", "itiş, çıkış açısı, kırılma")
    # 1) Hareket -> Çarpışma
    ok(d, 140, 118, 340, 84)
    d.add(String(240, 112, "1. koşu hızı → itiş gücü", fontName="Arial", fontSize=8, textAnchor="middle"))
    # 2) Tek tuş -> Çarpışma
    ok(d, 140, 30, 340, 58)
    d.add(String(240, 18, "2. basış mesafesi/yüksekliği → kalite, açı", fontName="Arial", fontSize=8, textAnchor="middle"))
    # 3) Çarpışma -> Hareket (geri dönüş oku)
    d.add(Line(407, 92, 407, 146, strokeWidth=1.2))
    d.add(Line(407, 146, 75, 146, strokeWidth=1.2))
    ok(d, 75, 146, 75, 138)
    d.add(String(240, 150, "3. kırılan barikat → enkaz → ivme ×0,3, fren ×0,2", fontName="Arial", fontSize=8, textAnchor="middle"))
    return d


# ---------- Oyun testi notu (sınıftaki eşli testten sonra doldur) ----------
ESLI_TEST = [
    "Gözlem 1: ……………………………………………………………………………………",
    "Gözlem 2: ……………………………………………………………………………………",
    "Gözlem 3: ……………………………………………………………………………………",
    "Yapılan düzeltme: …………………………………………………………………………",
]

# ---------- İçerik ----------
s = []
s.append(p("H7 Tasarım Belgesi · Üç Mekanik, Dört Oyun", BASLIK))
s.append(p("Karmaşık Oyun Mekanikleri · Ahmet Alp Şafak · Birleşik oyun: <b>Sisifos</b> · Unity 6000.1.6f1", ALT))
s.append(Spacer(1, 6))

# 1. Mekanik envanteri
s.append(p("1. Mekanik envanteri", H1))
s.append(tablo([
    ["Oyun", "Fiil", "Girdi", "Geri bildirim", "Başarısızlık"],
    ["A · Tek Tuş", "zıpla, süzül", "Boşluk/tık: bas = zıpla; düşerken basılı tut = süzül",
     "süzülürken karakter maviye döner; aşılan engel sayacı", "engele çarpmak; aynı tuşla yeniden başlar"],
    ["B · Hareket Hissi", "koş, zıpla, profil değiştir", "A/D; Boşluk (tutma süresi = yükseklik); E: Taş/Tüy",
     "profil rengi; süre ve düşme sayacı", "çukura düşmek: başa döner, düşme sayısı artar; R baştan"],
    ["C · Çarpışma", "sektir, kır", "A/D: raket; Boşluk: fırlat",
     "tuğla çatlar/patlar, skor ve can", "top alta kaçar; 3 can biter; R baştan"],
    ["D · Sisifos", "koş, it (omuz), zıpla", "A/D; Boşluk: kaya uzaktaysa zıpla, menzildeyse omuz",
     "yeşil = ideal omuz mesafesi, sarı = omuz, \"TAM OMUZ!\" yazısı, kamera sarsıntısı, kahverengi = enkaz",
     "kaya oyuncuyu ezer (hızlı gelen kaya ya da yetmeyen omuz); Boşluk/R baştan"],
], [2.3, 2.3, 4.2, 4.3, 4.1]))
s.append(Spacer(1, 3))
s.append(p("<b>Zorluk:</b> A'da her engelde hız +0,08 (en çok 12) ve aralık ×0,992. "
           "D'de her zirvede yokuşun geri itmesi +1,5, kaya ağırlığı +0,15 ve yoldaki engel sayısı +1. "
           "<b>İlerleme ölçüsü:</b> A aşılan engel, B süre, C skor, D kayanın metresi ve zirve sayısı."))

# 2. Ayar tabloları
s.append(p("2. Ayar tabloları", H1))
s.append(p("A · Tek Tuş", H2))
s.append(tablo([
    ["Değişken", "Değer", "Oyuncunun hissettiği etki"],
    ["ziplamaHizi / normalYercekimi", "12 / 3", "kısa engel tam zamanında basılırsa rahat aşılır, geç basış affetmez"],
    ["suzulmeYercekimi", "0,5", "basılı tutunca belirgin yavaşlama: uzun engel ancak süzülerek geçilir"],
    ["baslangicHizi → azamiHiz, hizArtisi", "6 → 12, +0,08", "ilk engeller öğretir, ~75 engelden sonra tepki süresi daralır"],
    ["uzunEngelOlasiligi / uzunGenislikCarpani", "0,3 / 0,7", "üç engelden biri uzun; genişlik hızla büyüdüğü için süzülme şart kalır"],
], [5.5, 2.6, 9.1]))

s.append(p("B · Hareket Hissi (iki profil, E ile değişir)", H2))
s.append(tablo([
    ["Değişken", "Taş", "Tüy", "Hissedilen etki"],
    ["azamiHiz", "6", "9", "Taş ağır ve temkinli, Tüy hızlı"],
    ["ivme / fren", "30 / 35", "90 / 120", "Taş kalkışta ve duruşta kayar, Tüy anında tepki verir"],
    ["ziplamaHizi / kisaZiplama", "12 / 0,8", "15 / 0,4", "Tüyde tutma süresi yüksekliği çok değiştirir, Taşta az"],
    ["coyoteSuresi", "0,05", "0,12", "Tüy kenardan geç basışı affeder, Taş cezalandırır"],
    ["tamponSuresi", "0,06", "0,15", "Tüyde yere inmeden basılan zıplama kaybolmaz"],
    ["dususCarpani (yercekimi 3)", "1,5", "1,1", "Taş taş gibi düşer, Tüy süzülür gibi iner"],
], [4.5, 2, 2, 8.7]))

s.append(p("C · Çarpışma", H2))
s.append(tablo([
    ["Değişken", "Değer", "Oyuncunun hissettiği etki"],
    ["baslangicHizi / hizArtisi / azamiHiz", "8 / +0,15 / 16", "her tuğla topu biraz hızlandırır; duvar inceldikçe gerilim artar"],
    ["enAzDikey", "0,25", "top yatay döngüye sıkışmaz, oyun hep akar"],
    ["raket açısı (fark, 1)", "-1..+1", "raketin kenarıyla vurmak topu yana yollar: nişan almak mümkün"],
    ["patlamaYaricapi", "1,7", "kırmızı tuğla komşularını da kırar: hedef seçmek ödüllendirilir"],
    ["can", "3", "üç hata hakkı; denemeye cesaretlendirir"],
], [5.5, 2.6, 9.1]))

s.append(p("D · Sisifos (oynayarak ayarlanan değerler gerekçeleriyle)", H2))
s.append(tablo([
    ["Değişken", "Değer", "Gerekçe / hissedilen etki"],
    ["azamiHiz / ivme / fren", "9 / 55 / 70", "kayaya koşarak yaklaşmak için çabuk hızlanma; fren ivmeden yüksek ki omuz mesafesinde durulabilsin"],
    ["coyoteSuresi / tamponSuresi", "0,1 / 0,12", "inişte kayanın üstünden atlarken geç ya da erken basış affedilir; ölüm haksız hissettirmez"],
    ["ziplamaHizi / yercekimi / dususCarpani", "14 / 3,2 / 1,3", "zıplama 0,9 yarıçaplı kayanın üstünü aşacak kadar yüksek, düşüş tok"],
    ["enkazIvmeCarpani / enkazFrenCarpani", "0,3 / 0,2", "enkaz üstünde koşu yavaş başlar ve durmak zorlaşır: kırmanın bedeli"],
    ["menzil / idealMesafe / tolerans", "3,2 / 1 / 2", "<b>ayarlandı:</b> önce ideal 1,2, tolerans 1,2 idi; kayaya dokunurken basmak en düşük kaliteyi veriyordu, pencere genişletildi"],
    ["enAzKalite", "0,55 (önce 0,35)", "<b>ayarlandı:</b> kötü zamanlanmış omuz bile kayayı ilerletmeli; yoksa oyuncu hiçbir şey olmuyor sanıyordu"],
    ["tabanItis / kosuItisCarpani", "11 (önce 9) / 0,8", "<b>ayarlandı:</b> durarak omuz 11, tam hızda 11+9×0,8≈18 itiş; koşmak hâlâ belirgin fark yaratır"],
    ["Barikat kirmaEsigi", "6 (önce 9)", "<b>ayarlandı:</b> 9'da yalnızca kusursuz omuz kırıyordu; 6'da iyi omuz yeter"],
    ["duzAci / dikAci", "25° / 55° (önce 6° / 48°)", "<b>ayarlandı:</b> yerden omuz kayayı ~25° kaldırıyordu, 1 m'lik eşik aşılamıyordu; şimdi ~39°"],
    ["sekmeOrani", "0,25 (önce 0,6)", "<b>ayarlandı:</b> eşiği aşamayan kaya hızla geri sekip oyuncuyu eziyordu; artık yavaş döner"],
    ["ezmeHizi / atlamaPayi", "9 (önce 7) / -0,4 (önce 0,3)", "<b>ayarlandı:</b> üstünden atlarken kafa değince ölünüyordu; yavaş kaya ve sıyırma artık öldürmez"],
    ["baslangicGeriIvme / geriIvmeArtisi / agirlikArtisi", "8 / +1,5 / +0,15", "her zirve kayayı ağırlaştırır ve yokuşu sertleştirir: Sisifos'un bitmeyen döngüsü"],
], [4.7, 3.4, 9.1]))

# 3. Bağ tablosu ve şeması
s.append(p("3. Bağ tablosu ve şeması", H1))
s.append(tablo([
    ["Kaynak mekanik", "Ne üretir?", "Hedef mekanik", "Neyi değiştirir?", "Kapatılınca ne olur?"],
    ["Hareket hissi", "omuz anındaki koşu hızı", "Çarpışma", "itiş = (11 + hız × 0,8) × kalite",
     "koşarak gelmenin anlamı kalmaz; durup omuz atmak yeter, hamle alma kararı kaybolur"],
    ["Tek tuş", "basış anındaki mesafe ve omuz yüksekliği", "Çarpışma",
     "itiş kalitesi (0,55–1) ve çıkış açısı: yerden basış kayayı kaldırır (eşik), havadan basış düz yollar (barikat)",
     "\"şimdi mi, sonra mı?\" sorusu kalkar; her omuz aynı olur, engele göre karar verilmez"],
    ["Çarpışma", "kırılan barikat → enkaz", "Hareket hissi",
     "enkaz üstünde ivme ×0,3, fren ×0,2",
     "kırmanın bedeli olmaz; her barikatı kırmak tek doğru yol olur, yol sonradan zorlaşmaz"],
], [2.5, 3.3, 2.3, 4.6, 4.5]))
s.append(Spacer(1, 10))
s.append(KeepTogether([bag_semasi()]))

# 4. Bağlantı testi
s.append(p("4. Bağlantı testi", H1))
s.append(p("<b>Tek tuş kapatılırsa</b> (omuz kayaya değince kendiliğinden atılsa): oyuncu yalnızca sağa yürür. "
           "Kayaya ne zaman ve hangi yükseklikten vurulacağı kararı kaybolur; eşik ile barikat arasındaki seçim "
           "(yerden mi, havadan mı?) ortadan kalkar. Oyun bir itme animasyonuna dönüşür: <b>bağ güçlü</b>."))
s.append(p("<b>Hareket hissi kapatılırsa</b> (sabit hız, ivme ve fren yok): koşu hızı hep aynı olduğundan itiş de hep aynıdır; "
           "geri çekilip hamle almanın değeri kalmaz. Enkazın ivme/fren cezası da etkisiz olur, yani iki bağ birden kopar. "
           "Oyun, zamanlamadan ibaret bir ritim oyununa döner: <b>bağ güçlü</b>."))
s.append(p("<b>Çarpışma kapatılırsa</b> (kaya oyuncuyla katı cisim gibi itilse, tasarlanmış açı ve kırılma olmasa): "
           "eşik hiç aşılamaz, barikat kırılmaz, enkaz oluşmaz. Ezilme tehdidi de kalkar. "
           "Oyun \"kutuyu duvara dayayıp itme\" oyununa dönüşür, başka bir oyun olur: <b>bağ güçlü</b>."))

# 5. Oyun testi notu
s.append(p("5. Oyun testi notu", H1))
s.append(p("<b>Ön test (kendi oyunum, ders öncesi).</b> Gözlenen üç sorun ve yapılan düzeltmeler: "
           "(1) Omuz çoğu zaman barikatı kıramıyordu → zamanlama penceresi genişletildi, kırma eşiği 9'dan 6'ya indi. "
           "(2) Kayanın üstünden atlarken kafa değince ölünüyordu → atlama payı -0,4, ezme hızı 9 yapıldı. "
           "(3) Kaya eşiği aşamıyor, geri sekip oyuncuyu eziyordu → çıkış açıları 25°/55°, sekme 0,25 yapıldı."))
s.append(p("<b>Eşli test (sınıfta, arkadaş açıklamasız 5 dk oynadı):</b>"))
for satir in ESLI_TEST:
    s.append(p(satir))

# 6. Yapay zekâ beyanı
s.append(p("6. Yapay zekâ beyanı", H1))
s.append(p("Araç: Claude (Anthropic), Claude Code ve Claude masaüstü uygulaması üzerinden. Kullanım ve dosyalar:"))
s.append(tablo([
    ["İş", "Dosyalar"],
    ["Dört oyunun betikleri ve sahneleri üreten kurucu yazıldı", "Assets/Scripts/A, B, C, D, Editor/SahneKurucu.cs"],
    ["CLAUDE.md, .gitignore ve README hazırlandı", "kök klasör"],
    ["Oyun testindeki geri bildirimime göre D'nin zorluk ayarları değiştirildi (omuz, açı, sekme, ezilme)",
     "SisifosHareket.cs, Kaya.cs, YolEngeli.cs, D_Birlesik.unity, D_Barikat.prefab"],
    ["Bütün betiklere açıklama satırları eklendi (mantık değişmedi)", "Assets/Scripts/**"],
    ["Bu belgenin taslağı yazıldı; değerler kodla ve oynayarak doğrulandı", "Belge/kaynak/belge_uret.py, Belge/H7_TasarimBelgesi.pdf"],
], [10.5, 6.7]))
s.append(Spacer(1, 3))
s.append(p("Ayrıntılı tarihli kayıt: Belge/YZ_Beyani.md. Ayar değişikliklerine kendi oyun testimdeki gözlemlere göre karar verdim."))


def sayfa_altligi(c, doc):
    """Her sayfanın altına ders adı ve sayfa numarası yazar."""
    c.setFont("Arial", 7.5)
    c.setFillColor(colors.HexColor("#777777"))
    c.drawString(2 * cm, 1.2 * cm, "KARMAŞIK OYUN MEKANİKLERİ · H7 TASARIM BELGESİ · AHMET ALP ŞAFAK")
    c.drawRightString(A4[0] - 2 * cm, 1.2 * cm, str(doc.page))


# ---------- PDF'i üret ----------
belge = SimpleDocTemplate(str(CIKTI), pagesize=A4, leftMargin=1.9 * cm, rightMargin=1.9 * cm,
                          topMargin=1.6 * cm, bottomMargin=1.8 * cm,
                          title="H7 Tasarım Belgesi", author="Ahmet Alp Şafak")
belge.build(s, onFirstPage=sayfa_altligi, onLaterPages=sayfa_altligi)
print("Yazıldı:", CIKTI)
