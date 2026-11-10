using UnityEngine;
using UnityEngine.InputSystem;

/// Oyun C: raketin neresine değdiği açıyı belirler; gelen açı bilerek yok sayılır.
[RequireComponent(typeof(Rigidbody2D))]
public class Top : MonoBehaviour
{
    [SerializeField] Transform raket;           // fırlatılmadan önce topun durduğu raket
    [SerializeField] float baslangicHizi = 8f;  // fırlatma hızı
    [SerializeField] float hizArtisi = 0.15f;   // her tuğla çarpışmasında
    [SerializeField] float azamiHiz = 16f;      // top bundan hızlı olmaz
    [SerializeField] float enAzDikey = 0.25f;   // yatay döngüye sıkışmasın

    Rigidbody2D rb;          // topun fizik gövdesi
    float hiz;               // şu anki sabit hız
    bool hazir = true;       // top raketin üstünde fırlatılmayı bekliyor
    bool firlatIstegi;       // Update'te basıldı, FixedUpdate'te fırlatılacak

    // Fizik gövdesini bulur ve hızı başlangıca ayarlar.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hiz = baslangicHizi;
    }

    // Top hazırsa boşluk basışını fırlatma isteği olarak saklar.
    void Update()
    {
        Keyboard k = Keyboard.current;
        if (hazir && k != null && k.spaceKey.wasPressedThisFrame) firlatIstegi = true;
    }

    // Hazırken topu raketin üstünde tutar; uçarken hızını sabit ve yönünü dikey tutar.
    void FixedUpdate()
    {
        if (hazir)
        {
            // Top raketin hemen üstünde, hareketsiz bekler.
            rb.position = (Vector2)raket.position + Vector2.up * 0.55f;
            rb.linearVelocity = Vector2.zero;
            if (firlatIstegi)
            {
                // Hafif sağa ve yukarı fırlat.
                firlatIstegi = false;
                hazir = false;
                rb.linearVelocity = new Vector2(0.4f, 1f).normalized * hiz;
            }
            return;
        }

        // Kayan nokta hataları birikmesin diye hız her adımda sabite çekilir.
        Vector2 yon = rb.linearVelocity.normalized;
        // Yön fazla yataylaştıysa en az dikey payı ver.
        if (Mathf.Abs(yon.y) < enAzDikey)
        {
            yon.y = (yon.y < 0f ? -1f : 1f) * enAzDikey;
            yon.Normalize();
        }
        rb.linearVelocity = yon * hiz;
    }

    // Rakete çarpınca çıkış açısını vuruş noktası belirler; tuğlaya çarpınca tuğlayı vurur.
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Raket"))
        {
            // fark: raketin solunda -1, ortasında 0, sağında +1.
            float yari = c.collider.bounds.size.x / 2f;
            float fark = (transform.position.x - c.transform.position.x) / yari;
            rb.linearVelocity = new Vector2(fark, 1f).normalized * hiz;
        }
        else if (c.gameObject.CompareTag("Tugla"))
        {
            // Tuğlaya hasar ver ve topu biraz hızlandır.
            c.gameObject.GetComponent<Tugla>().Vur();
            hiz = Mathf.Min(hiz + hizArtisi, azamiHiz);
        }
    }

    // Can gidince topu raketin üstüne ve başlangıç hızına döndürür.
    public void Sifirla()
    {
        hazir = true;
        hiz = baslangicHizi;
    }
}
