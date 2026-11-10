using UnityEngine;
using UnityEngine.InputSystem;

/// Oyun D: hareket hissi ve tek tuş kararı.
/// Boşluk: kaya menzilde değilse zıplar, menzildeyse omuz atar.
[RequireComponent(typeof(Rigidbody2D))]
public class SisifosHareket : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] float azamiHiz = 9f; // en yüksek koşu hızı
    [SerializeField] float ivme = 55f;    // tuşa basınca hızlanma oranı
    [SerializeField] float fren = 70f;    // tuş bırakılınca yavaşlama oranı

    [Header("Zıplama")]
    [SerializeField] float ziplamaHizi = 14f;    // zıplamada verilen yukarı hız
    [SerializeField] float kisaZiplama = 0.5f;   // tuş erken bırakılınca kalan yükselme oranı
    [SerializeField] float yercekimi = 3.2f;     // temel yerçekimi çarpanı
    [SerializeField] float dususCarpani = 1.3f;  // düşerken yerçekimi kaç kat artar
    [SerializeField] float coyoteSuresi = 0.1f;  // kenardan düştükten sonra zıplama hakkı
    [SerializeField] float tamponSuresi = 0.12f; // erken basılan zıplamanın saklanma süresi

    [Header("Enkaz (çarpışma → hareket bağı)")]
    [SerializeField] float enkazIvmeCarpani = 0.3f; // enkaz üstünde ivme bu orana düşer
    [SerializeField] float enkazFrenCarpani = 0.2f; // enkaz üstünde fren bu orana düşer (kayar)

    [Header("Omuz (tek tuş → çarpışma bağı)")]
    [SerializeField] float menzil = 3.2f;          // kayaya bu mesafeden yakınsa boşluk omuz olur
    [SerializeField] float idealMesafe = 1f;       // en iyi omuz bu mesafede atılır
    [SerializeField] float tolerans = 2f;          // idealden bu kadar uzaklaşınca kalite en aza iner
    [SerializeField] float enAzKalite = 0.55f;     // en kötü zamanlamada bile kalan kalite
    [SerializeField] float omuzSuresi = 0.3f;      // omuz hamlesinin sürdüğü zaman
    [SerializeField] float omuzHizi = 13f;         // omuz sırasında ileri atılma hızı
    [SerializeField] float omuzBeklemesi = 0.25f;  // iki omuz arası bekleme
    [SerializeField] float tabanItis = 11f;        // koşmadan atılan omuzun itişi
    [SerializeField] float kosuItisCarpani = 0.8f; // hareket → çarpışma bağı

    [Header("Bağlantılar")]
    [SerializeField] Transform ayak;              // yere değme kontrol noktası
    [SerializeField] float ayakYaricapi = 0.15f;  // ayak kontrol dairesinin yarıçapı
    [SerializeField] LayerMask zeminKatmani;      // hangi katmanlar zemin sayılır
    [SerializeField] Transform omuzNoktasi;       // kayaya değen omuzun yüksekliği
    [SerializeField] SpriteRenderer gorsel;       // duruma göre boyanan görsel
    [SerializeField] SisifosOyun oyun;            // oyun sürüyor mu
    [SerializeField] Kaya kaya;                   // mesafe ve durum için kaya

    Rigidbody2D rb;         // oyuncunun fizik gövdesi
    BoxCollider2D govde;    // oyuncunun kutu çarpıştırıcısı
    float girdi;            // yatay girdi: -1 sol, 0 yok, +1 sağ
    float coyote;           // kalan coyote süresi
    float tampon;           // kalan zıplama tamponu
    float omuzKalan;        // omuz hamlesinin bitmesine kalan süre
    float omuzBekleme;      // yeni omuz atılabilmesine kalan süre
    float omuzKalitesi;     // son omzun zamanlama kalitesi (0-1)
    float omuzKosuHizi;     // omuz atıldığı andaki koşu hızı
    bool yerde;             // zemine değiyor mu
    bool tusBasili;         // boşluk basılı mı
    bool kisaKes;           // zıplama kısaltılacak mı
    bool kayaOnde;          // yavaş kaya önümü kesiyor mu (bu fizik adımı için)
    float kayaHizi;         // önümü kesen kayanın hızı
    int enkazSayisi;        // şu an üstünde durulan enkaz parçası sayısı

    public bool OmuzAktif => omuzKalan > 0f;                           // omuz hamlesi sürüyor mu
    public float Kalite => omuzKalitesi;                               // son omzun kalitesi
    public float OmuzY => omuzNoktasi.position.y;                      // omuz yüksekliği
    public float AyakY => rb.position.y - govde.size.y * 0.5f;         // ayakların yüksekliği

    // İtiş = (taban + koşu hızı katkısı) × zamanlama kalitesi.
    public float ItisGucu => (tabanItis + omuzKosuHizi * kosuItisCarpani) * omuzKalitesi;

    bool EnkazUstunde => yerde && enkazSayisi > 0; // enkaza basıyor mu

    // Bileşenleri bir kez bulur.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        govde = GetComponent<BoxCollider2D>();
    }

    // Girdiyi okur, sayaçları günceller ve boşluğun zıplama mı omuz mu olacağına karar verir.
    void Update()
    {
        if (!oyun.Oynaniyor) return;

        Keyboard k = Keyboard.current;
        if (k == null) return;

        // A/D veya ok tuşlarından yatay girdi.
        girdi = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) girdi -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) girdi += 1f;

        // Yükselirken yerde sayılmaz; ayak dairesi zemine değiyorsa yerdedir.
        yerde = rb.linearVelocityY <= 0.01f
             && Physics2D.OverlapCircle(ayak.position, ayakYaricapi, zeminKatmani);
        coyote = yerde ? coyoteSuresi : coyote - Time.deltaTime;
        tampon -= Time.deltaTime;
        omuzBekleme -= Time.deltaTime;

        // Omuz süresini say; kayaya değmeden biterse bekleme başlar.
        if (omuzKalan > 0f)
        {
            omuzKalan -= Time.deltaTime;
            if (omuzKalan <= 0f) omuzBekleme = omuzBeklemesi; // boşa giden omuz
        }

        // Tek tuşun kararı: aynı basış, kayaya uzaklığa göre iki farklı eylem olur.
        if (k.spaceKey.wasPressedThisFrame)
        {
            if (OmuzAtilabilir()) OmuzBaslat();
            else tampon = tamponSuresi;
        }

        // Tuş bırakılınca zıplama kısaltılacak.
        tusBasili = k.spaceKey.isPressed;
        if (k.spaceKey.wasReleasedThisFrame) kisaKes = true;

        RengiGuncelle();
    }

    // Yatay hızı, omuz atılmasını, kaya engelini, zıplamayı ve yerçekimini uygular.
    void FixedUpdate()
    {
        // Enkaz üstünde ivme ve fren düşer: kaygan zemin.
        float gecerliIvme = EnkazUstunde ? ivme * enkazIvmeCarpani : ivme;
        float gecerliFren = EnkazUstunde ? fren * enkazFrenCarpani : fren;

        // Girdi varsa ivmeyle, yoksa frenle hedef hıza yaklaş.
        float hedef = girdi * azamiHiz;
        float oran = Mathf.Abs(girdi) > 0.01f ? gecerliIvme : gecerliFren;
        float vx = Mathf.MoveTowards(rb.linearVelocityX, hedef, oran * Time.fixedDeltaTime);

        // Omuz sırasında oyuncu ileri atılır.
        if (OmuzAktif) vx = Mathf.Max(vx, omuzHizi);

        if (kayaOnde)
        {
            vx = Mathf.Min(vx, kayaHizi); // yavaş kayanın içinden yürünmez
            kayaOnde = false;
        }
        rb.linearVelocityX = vx;

        // Tamponda basış ve coyote süresi varsa zıpla.
        if (tampon > 0f && coyote > 0f)
        {
            rb.linearVelocityY = ziplamaHizi;
            tampon = 0f;
            coyote = 0f;
            kisaKes = !tusBasili;
        }

        // Tuş erken bırakıldıysa yükselişi kes: kısa zıplama.
        if (kisaKes)
        {
            if (rb.linearVelocityY > 0f) rb.linearVelocityY *= kisaZiplama;
            kisaKes = false;
        }

        // Düşerken yerçekimi artar.
        rb.gravityScale = rb.linearVelocityY < 0f ? yercekimi * dususCarpani : yercekimi;
    }

    // Oyuncunun ön yüzü ile kayanın sol kenarı arasındaki boşluk.
    float KayayaBosluk()
    {
        return kaya.SolKenar - (rb.position.x + govde.size.x * 0.5f);
    }

    // Omuz atılabilir mi: kaya tırmanıyor, bekleme bitti, kaya önde, menzil içinde ve üstünde değiliz.
    bool OmuzAtilabilir()
    {
        return kaya.Durum == KayaDurumu.Tirmanis
            && !OmuzAktif
            && omuzBekleme <= 0f
            && rb.position.x < kaya.transform.position.x
            && AyakY < kaya.UstKenar
            && KayayaBosluk() <= menzil;
    }

    // Boşluk ideal mesafeye ne kadar yakınsa kalite o kadar yüksek (enAzKalite..1).
    float KaliteHesapla(float bosluk)
    {
        return Mathf.Clamp(1f - Mathf.Abs(bosluk - idealMesafe) / tolerans, enAzKalite, 1f);
    }

    // Omuz hamlesini başlatır: kaliteyi ve o anki koşu hızını kaydeder.
    void OmuzBaslat()
    {
        omuzKalitesi = KaliteHesapla(KayayaBosluk());
        omuzKosuHizi = Mathf.Max(0f, rb.linearVelocityX); // basış anındaki koşu hızı
        omuzKalan = omuzSuresi;
        tampon = 0f;
    }

    // Kaya, omuz temasını işledikten sonra çağırır.
    public void OmuzVurdu()
    {
        omuzKalan = 0f;
        omuzBekleme = omuzBeklemesi;
        rb.linearVelocityX = 0f;
    }

    // Kaya, yavaşça önümü kestiğinde çağırır: bu adımda kayadan hızlı gidemem.
    public void KayaOnde(float hiz)
    {
        kayaOnde = true;
        kayaHizi = hiz;
    }

    // Duruma göre renk: sarı omuz, yeşil iyi omuz anı, kahverengi enkaz, beyaz normal.
    void RengiGuncelle()
    {
        if (OmuzAktif) gorsel.color = new Color(1f, 0.85f, 0.2f);
        else if (OmuzAtilabilir() && KaliteHesapla(KayayaBosluk()) >= 0.8f) gorsel.color = new Color(0.35f, 1f, 0.45f);
        else if (EnkazUstunde) gorsel.color = new Color(0.75f, 0.55f, 0.35f);
        else gorsel.color = Color.white;
    }

    // Enkaz parçasına girince sayacı artırır.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.CompareTag("Enkaz")) enkazSayisi++;
    }

    // Enkaz parçasından çıkınca sayacı azaltır.
    void OnTriggerExit2D(Collider2D diger)
    {
        if (diger.CompareTag("Enkaz")) enkazSayisi--;
    }
}
