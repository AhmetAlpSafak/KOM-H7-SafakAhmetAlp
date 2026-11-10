using UnityEngine;
using UnityEngine.InputSystem;

/// Oyun B: ivme, fren, değişken zıplama, coyote time, tampon ve iki profil.
[RequireComponent(typeof(Rigidbody2D))]
public class HareketHissi : MonoBehaviour
{
    [Header("Profiller (E tuşu değiştirir)")]
    // Taş: ağır, yavaş tepki veren karakter.
    [SerializeField] HareketProfili tas = new HareketProfili
    {
        ad = "Taş", renk = new Color(0.65f, 0.65f, 0.7f),
        azamiHiz = 6f, ivme = 30f, fren = 35f, ziplamaHizi = 12f,
        kisaZiplama = 0.8f, coyoteSuresi = 0.05f, tamponSuresi = 0.06f, dususCarpani = 1.5f
    };
    // Tüy: hafif, anında tepki veren ve affedici karakter.
    [SerializeField] HareketProfili tuy = new HareketProfili
    {
        ad = "Tüy", renk = new Color(1f, 0.85f, 0.45f),
        azamiHiz = 9f, ivme = 90f, fren = 120f, ziplamaHizi = 15f,
        kisaZiplama = 0.4f, coyoteSuresi = 0.12f, tamponSuresi = 0.15f, dususCarpani = 1.1f
    };

    [Header("Fizik")]
    [SerializeField] float yercekimi = 3f;        // temel yerçekimi çarpanı
    [SerializeField] Transform ayak;              // yere değme kontrolünün yapıldığı nokta
    [SerializeField] float ayakYaricapi = 0.15f;  // ayak kontrol dairesinin yarıçapı
    [SerializeField] LayerMask zeminKatmani;      // hangi katmanlar zemin sayılır
    [SerializeField] SpriteRenderer gorsel;       // profil rengini gösteren görsel

    Rigidbody2D rb;          // karakterin fizik gövdesi
    HareketProfili aktif;    // şu an kullanılan profil
    float girdi;             // yatay girdi: -1 sol, 0 yok, +1 sağ
    float coyote;            // kenardan düştükten sonra hâlâ zıplanabilecek süre
    float tampon;            // yere inmeden az önce basılan zıplamanın saklandığı süre
    bool tusBasili;          // boşluk şu an basılı mı
    bool kisaKes;            // tuş erken bırakıldı: zıplama kısaltılacak

    public string ProfilAdi => aktif.ad; // ekranda gösterilen profil adı

    // Fizik gövdesini bulur ve Taş profiliyle başlar.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ProfilSec(tas);
    }

    // Girdiyi okur, yere değmeyi kontrol eder, coyote ve tampon sayaçlarını günceller.
    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;

        // A/D veya ok tuşlarından yatay girdi.
        girdi = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) girdi -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) girdi += 1f;

        // E tuşu iki profil arasında geçiş yapar.
        if (k.eKey.wasPressedThisFrame) ProfilSec(aktif == tas ? tuy : tas);

        // Yükselirken "yerde" sayılmaz; yoksa zıplamadan hemen sonra coyote yeniden dolardı.
        bool yerde = rb.linearVelocityY <= 0.01f
                  && Physics2D.OverlapCircle(ayak.position, ayakYaricapi, zeminKatmani);

        // Yerdeyken coyote dolu kalır, havada azalır. Basış tampona yazılır, sonra azalır.
        coyote = yerde ? aktif.coyoteSuresi : coyote - Time.deltaTime;
        tampon = k.spaceKey.wasPressedThisFrame ? aktif.tamponSuresi : tampon - Time.deltaTime;

        // Tuş bırakılınca zıplama kısaltılacak.
        tusBasili = k.spaceKey.isPressed;
        if (k.spaceKey.wasReleasedThisFrame) kisaKes = true;
    }

    // Yatay hızı ivme/frenle hedefe yaklaştırır, zıplamayı ve yerçekimini uygular.
    void FixedUpdate()
    {
        // Girdi varsa ivmeyle, yoksa frenle hedef hıza yaklaş.
        float hedef = girdi * aktif.azamiHiz;
        float oran = Mathf.Abs(girdi) > 0.01f ? aktif.ivme : aktif.fren;
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, hedef, oran * Time.fixedDeltaTime);

        // Tamponda basış ve coyote süresi varsa zıpla.
        if (tampon > 0f && coyote > 0f)
        {
            rb.linearVelocityY = aktif.ziplamaHizi;
            tampon = 0f;
            coyote = 0f;
            kisaKes = !tusBasili; // tampondan gelen kısa basış da kısa zıplama olsun
        }

        // Tuş erken bırakıldıysa yükselme hızını kes: kısa zıplama.
        if (kisaKes)
        {
            if (rb.linearVelocityY > 0f) rb.linearVelocityY *= aktif.kisaZiplama;
            kisaKes = false;
        }

        // Düşerken yerçekimi artar: zıplama daha tok hissettirir.
        rb.gravityScale = rb.linearVelocityY < 0f ? yercekimi * aktif.dususCarpani : yercekimi;
    }

    // Aktif profili değiştirir ve rengini uygular.
    void ProfilSec(HareketProfili profil)
    {
        aktif = profil;
        gorsel.color = profil.renk;
    }

    // Karakteri hızını sıfırlayarak verilen konuma taşır (düşünce başa dönmek için).
    public void Isinla(Vector2 konum)
    {
        rb.position = konum;
        transform.position = konum; // interpolasyon eski konumu bir kare daha göstermesin
        rb.linearVelocity = Vector2.zero;
    }

    // Editörde seçiliyken ayak kontrol dairesini çizer.
    void OnDrawGizmosSelected()
    {
        if (ayak != null) Gizmos.DrawWireSphere(ayak.position, ayakYaricapi);
    }
}
