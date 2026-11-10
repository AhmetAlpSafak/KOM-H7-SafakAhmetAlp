using UnityEngine;

// Kayanın iki durumu: oyuncu yukarı itiyor ya da zirveden sonra aşağı yuvarlanıyor.
public enum KayaDurumu { Tirmanis, Inis }

/// Oyun D: yokuş sürekli geri kuvvetle taklit edilir (fiziği tasarlıyoruz).
/// Omuz teması tasarlanmış çarpışmadır: gelen açı yok sayılır, çıkış açısını omuz yüksekliği belirler.
[RequireComponent(typeof(Rigidbody2D))]
public class Kaya : MonoBehaviour
{
    [Header("Yol")]
    [SerializeField] float tabanX = 6f;              // yokuşun dibi: kaya bunun gerisine gidemez
    [SerializeField] float hizSiniri = 18f;          // kayanın yatay hızı bunu aşamaz
    [SerializeField] float inisKuvvetCarpani = 1.5f; // inişte geri kuvvet kaç kat güçlü

    [Header("Ezme")]
    [SerializeField] float ezmeHizi = 9f;         // kaya bundan hızlı geliyorsa oyuncuyu ezer
    [SerializeField] float karsilamaOrani = 0.8f; // itiş, geliş hızının bu kadarına yetmeli
    [SerializeField] float atlamaPayi = -0.4f;    // eksi: ayak merkezin biraz altındaysa da "üstünden geçti" sayılır
    [SerializeField] float gecisPayi = 0.6f;      // kayanın altına girmiş oyuncu takılı kalmasın

    [Header("Çıkış açısı (derece)")]
    [SerializeField] float dikAci = 55f;   // alttan vuruş
    [SerializeField] float duzAci = 25f;   // merkezden vuruş: yerden omuz eşiği aşacak kadar kaldırsın
    [SerializeField] float kirilmaHizKorunumu = 0.7f; // engel kırınca kayada kalan hız oranı
    [SerializeField] float sekmeOrani = 0.25f; // kırılmayan engelden yavaş geri seksin, oyuncuyu ezmesin

    [Header("Bağlantılar")]
    [SerializeField] SisifosOyun oyun;      // zorluk, kaybetme ve olay bildirimleri
    [SerializeField] SisifosHareket oyuncu; // omuz durumu ve konumu okunur
    [SerializeField] Transform gorsel;      // yuvarlanma için döndürülen görsel

    Rigidbody2D rb;          // kayanın fizik gövdesi
    CircleCollider2D govde;  // kayanın daire çarpıştırıcısı
    Vector2 oncekiHiz;       // çarpışmadan önceki hız (çarpışma anında hız zaten değişmiş olur)

    public KayaDurumu Durum { get; private set; } = KayaDurumu.Tirmanis; // şu anki durum
    public float Metre => Mathf.Max(0f, transform.position.x - tabanX);  // dipten kaç metre yukarıda
    public float SolKenar => transform.position.x - govde.radius;       // oyuncuya bakan kenar
    public float UstKenar => transform.position.y + govde.radius;       // kayanın tepesi

    // Bileşenleri bir kez bulur.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        govde = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        // Oyuncu kayayı katı cisim olarak itemez; yalnızca omuz teması sonuç üretir.
        Physics2D.IgnoreCollision(govde, oyuncu.GetComponent<Collider2D>());
    }

    // Görseli hıza göre döndürür: kaya kaymıyor, yuvarlanıyor gibi görünür.
    void Update()
    {
        gorsel.Rotate(0f, 0f, -rb.linearVelocityX / govde.radius * Mathf.Rad2Deg * Time.deltaTime);
    }

    // Yokuşu taklit eden geri kuvveti uygular, hızı sınırlar, dipte durdurur.
    void FixedUpdate()
    {
        // Yokuş: kayayı sürekli sola (aşağı) iten kuvvet. İnişte daha güçlü.
        float geri = oyun.GeriIvme * (Durum == KayaDurumu.Inis ? inisKuvvetCarpani : 1f);
        rb.AddForce(Vector2.left * geri);

        // Yatay hızı sınır içinde tut.
        Vector2 v = rb.linearVelocity;
        v.x = Mathf.Clamp(v.x, -hizSiniri, hizSiniri);
        rb.linearVelocity = v;

        // Dibe ulaştıysa orada dur.
        if (rb.position.x <= tabanX && rb.linearVelocityX < 0f) TabandaDur();

        oncekiHiz = rb.linearVelocity;
    }

    // Kayayı dipte durdurur; inişten geldiyse yeni tur başlar.
    void TabandaDur()
    {
        rb.position = new Vector2(tabanX, rb.position.y);
        rb.linearVelocityX = 0f;
        if (Durum == KayaDurumu.Inis)
        {
            Durum = KayaDurumu.Tirmanis;
            oyun.TabanaDondu();
        }
    }

    // Zirveye ulaşınca çağrılır: kaya durur ve aşağı yuvarlanmaya başlar.
    public void IniseGec()
    {
        Durum = KayaDurumu.Inis;
        rb.linearVelocity = Vector2.zero;
    }

    // Kök nesne, çocuktaki Temas tetikleyicisinin olaylarını da alır.
    void OnTriggerStay2D(Collider2D diger)
    {
        if (!diger.CompareTag("Player")) return;

        // Oyuncu kayanın arkasındaysa ya da üstünden geçiyorsa temas sayılmaz.
        bool oyuncuYukarida = oyuncu.transform.position.x > transform.position.x;
        bool oyuncuUstunde = oyuncu.AyakY > transform.position.y + atlamaPayi;
        if (oyuncuYukarida || oyuncuUstunde) return;

        // Kayanın oyuncuya doğru (sola) gelme hızı.
        float gelisHizi = Mathf.Max(0f, -rb.linearVelocityX);

        // Tırmanışta omuz atılmışsa temas bir itiştir.
        if (Durum == KayaDurumu.Tirmanis && oyuncu.OmuzAktif)
        {
            OmuzuKarsila(gelisHizi);
            return;
        }

        // Omuz yoksa ve kaya hızlı geliyorsa oyuncu ezilir.
        if (gelisHizi > ezmeHizi)
        {
            oyun.Kaybet("Kaya seni ezdi.");
            return;
        }

        // Yavaş kaya oyuncuyu ezmez, yalnızca önünü keser: oyuncu kayanın içinden yürüyemez.
        bool oyuncuKayaninOnunde = oyuncu.transform.position.x < transform.position.x - govde.radius * gecisPayi;
        if (oyuncuKayaninOnunde) oyuncu.KayaOnde(rb.linearVelocityX);
    }

    // Omuz temasını işler: itiş yetmezse oyuncu ezilir, yeterse kaya fırlatılır.
    void OmuzuKarsila(float gelisHizi)
    {
        // Kaya ağırlaştıkça aynı omuz daha az hız verir.
        float itis = oyuncu.ItisGucu / oyun.Agirlik;

        // Hızlı gelen kayayı durdurmaya itiş yetmiyorsa ezilirsin.
        if (gelisHizi > ezmeHizi && itis < gelisHizi * karsilamaOrani)
        {
            oyun.Kaybet("Omuzun yetmedi, kaya seni ezdi.");
            return;
        }

        // Kayayı omuz yüksekliğine göre açıyla fırlat.
        rb.linearVelocity = CikisYonu(oyuncu.OmuzY) * itis;
        oyuncu.OmuzVurdu();
        oyun.KayaVuruldu(oyuncu.Kalite);
    }

    // Alttan vuruş kayayı sıçratır, üstten vuruş yokuşa yapışık gönderir.
    Vector2 CikisYonu(float omuzY)
    {
        // fark: omuz kayanın altındaysa eksi, merkezindeyse 0, üstündeyse artı (-1..1).
        float fark = Mathf.Clamp((omuzY - transform.position.y) / govde.radius, -1f, 1f);
        float aci = fark < 0f ? Mathf.Lerp(duzAci, dikAci, -fark) : Mathf.Lerp(duzAci, 0f, fark);
        float radyan = aci * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radyan), Mathf.Sin(radyan));
    }

    // Engele çarpınca: yeterince hızlıysa engeli kırar, değilse geri seker.
    void OnCollisionEnter2D(Collision2D c)
    {
        if (!c.gameObject.CompareTag("Engel")) return;

        YolEngeli engel = c.gameObject.GetComponent<YolEngeli>();
        float carpmaHizi = Mathf.Abs(oncekiHiz.x);
        bool inis = Durum == KayaDurumu.Inis;

        if (engel.Carp(carpmaHizi, inis))
        {
            // Kırdı: inişte hız korunur, tırmanışta biraz yavaşlar.
            rb.linearVelocity = inis ? oncekiHiz : oncekiHiz * kirilmaHizKorunumu;
            oyun.EngelKirildi();
        }
        else if (carpmaHizi > 0.5f)
        {
            // Kıramadı: ters yöne, hızının bir kısmıyla geri sek.
            rb.linearVelocityX = -Mathf.Sign(oncekiHiz.x) * carpmaHizi * sekmeOrani;
        }
    }
}
