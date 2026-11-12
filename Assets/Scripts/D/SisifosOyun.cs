using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Oyunun üç hâli: başlamayı bekliyor, oynanıyor, bitti.
public enum OyunDurumu { Bekliyor, Oynaniyor, Bitti }

/// Oyun D: başlangıç, başarısızlık, ilerleme ölçüsü ve zorluk burada toplanır.
public class SisifosOyun : MonoBehaviour
{
    [Header("Zorluk (her zirvede artar)")]
    [SerializeField] float baslangicGeriIvme = 8f; // ilk turda yokuşun kayayı geri itme gücü
    [SerializeField] float geriIvmeArtisi = 1.5f;  // her zirvede geri itme bu kadar artar
    [SerializeField] float agirlikArtisi = 0.15f;  // her zirvede kaya bu kadar ağırlaşır
    [SerializeField] int baslangicEngelSayisi = 4; // ilk turdaki engel sayısı

    [Header("Bağlantılar")]
    [SerializeField] Kaya kaya;             // metre ve durum için
    [SerializeField] YolKurucu yol;         // engelleri kuran betik
    [SerializeField] SisifosKamera kamera;  // sarsıntı için

    public OyunDurumu Durum { get; private set; } = OyunDurumu.Bekliyor; // şu anki hâl
    public bool Oynaniyor => Durum == OyunDurumu.Oynaniyor;              // kısa yol

    public float GeriIvme => baslangicGeriIvme + geriIvmeArtisi * zirveSayisi; // bu turun yokuş gücü
    public float Agirlik => 1f + agirlikArtisi * zirveSayisi;                 // bu turun kaya ağırlığı

    int zirveSayisi;          // kaç kez zirveye çıkıldı
    float enYuksekMetre;      // ulaşılan en yüksek metre
    string bitisMesaji = "";  // kaybedince gösterilen yazı
    string vurusMesaji = "";  // omuz sonrası "TAM OMUZ!" gibi yazı
    float vurusMesajKalan;    // vuruş yazısının ekranda kalma süresi
    float bitisAni;           // oyunun bittiği an (gerçek zamanla)

    // Oyunu durmuş hâlde başlatır: Boşluk'a basılana kadar zaman akmaz.
    void Awake()
    {
        Time.timeScale = 0f; // oyuncu başlatana kadar her şey durur
    }

    // İlk turun engellerini kurar.
    void Start()
    {
        yol.Kur(baslangicEngelSayisi);
    }

    // En yüksek metreyi takip eder ve vuruş yazısının süresini azaltır.
    void Update()
    {
        if (Oynaniyor) enYuksekMetre = Mathf.Max(enYuksekMetre, kaya.Metre);
        vurusMesajKalan -= Time.unscaledDeltaTime;
    }

    // LateUpdate: oyunu başlatan basış, aynı karede zıplama sayılmasın.
    void LateUpdate()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;

        // R her zaman baştan başlatır.
        if (k.rKey.wasPressedThisFrame) YenidenBaslat();

        // Beklerken boşluk oyunu başlatır; bittiyse (kısa beklemeden sonra) yeniden başlatır.
        if (Durum == OyunDurumu.Bekliyor && k.spaceKey.wasPressedThisFrame)
        {
            Durum = OyunDurumu.Oynaniyor;
            Time.timeScale = 1f;
        }
        else if (Durum == OyunDurumu.Bitti && k.spaceKey.wasPressedThisFrame
                 && Time.unscaledTime - bitisAni > 0.5f)
        {
            YenidenBaslat();
        }
    }

    // Sahneyi baştan yükler.
    void YenidenBaslat()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Oyunu verilen mesajla bitirir ve zamanı dondurur.
    public void Kaybet(string mesaj)
    {
        if (!Oynaniyor) return;
        Durum = OyunDurumu.Bitti;
        bitisMesaji = mesaj;
        bitisAni = Time.unscaledTime;
        Time.timeScale = 0f;
    }

    // Omuz kayaya değince kaliteye göre yazı gösterir ve kamerayı sarsar.
    public void KayaVuruldu(float kalite)
    {
        vurusMesaji = kalite >= 0.8f ? "TAM OMUZ!" : kalite >= 0.55f ? "İyi omuz" : "Zayıf omuz";
        vurusMesajKalan = 1f;
        kamera.Sars(0.1f + 0.2f * kalite, 0.12f);
    }

    // Engel kırılınca kamerayı sarsar.
    public void EngelKirildi()
    {
        kamera.Sars(0.3f, 0.15f);
    }

    // Kaya zirveye çıkınca: sayaç artar, kaya inişe geçer, kamera sarsılır.
    public void ZirveyeUlasti()
    {
        zirveSayisi++;
        kaya.IniseGec();
        kamera.Sars(0.4f, 0.3f);
    }

    // Kaya inişi bitirdi: yeni tur için yol yeniden kurulur, eski enkaz kalır.
    public void TabanaDondu()
    {
        yol.Kur(baslangicEngelSayisi + zirveSayisi);
    }

    // Ekrana metre, zirve, kontroller ve durum yazılarını çizer.
    void OnGUI()
    {
        GUIStyle yazi = new GUIStyle(GUI.skin.label) { fontSize = 24 };
        GUIStyle buyuk = new GUIStyle(yazi) { fontSize = 40, fontStyle = FontStyle.Bold };

        GUI.Label(new Rect(20, 15, 1100, 40),
            "Kaya: " + kaya.Metre.ToString("0") + " m   En yüksek: " + enYuksekMetre.ToString("0") + " m   Zirve: " + zirveSayisi, yazi);
        GUI.Label(new Rect(20, 50, 1100, 40), "A/D: yürü · Boşluk: uzaktaysan zıpla, kaya yakınsa omuz at · R: baştan", yazi);

        // İnişteyken uyarı, değilse son omzun yazısı.
        if (kaya.Durum == KayaDurumu.Inis && Oynaniyor)
            GUI.Label(new Rect(20, 95, 1100, 60), "KAYA GERİ DÖNÜYOR: ÜSTÜNDEN ATLA!", buyuk);
        else if (vurusMesajKalan > 0f && Oynaniyor)
            GUI.Label(new Rect(20, 95, 1100, 60), vurusMesaji, buyuk);

        // Başlangıç ve bitiş ekranı.
        if (Durum == OyunDurumu.Bekliyor)
            GUI.Label(new Rect(20, 150, 1100, 60), "SİSİFOS — Başlamak için Boşluk", buyuk);
        else if (Durum == OyunDurumu.Bitti)
            GUI.Label(new Rect(20, 150, 1100, 120), bitisMesaji + "\nYeniden başlamak için Boşluk", buyuk);
    }
}
