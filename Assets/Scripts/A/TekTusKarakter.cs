using UnityEngine;
using UnityEngine.InputSystem;

/// Oyun A: tek tuş, iki zamanlama. Bas = zıpla. Düşerken basılı tut = süzül.
[RequireComponent(typeof(Rigidbody2D))]
public class TekTusKarakter : MonoBehaviour
{
    [Header("Zıplama")]
    [SerializeField] float ziplamaHizi = 12f;     // zıplamada verilen yukarı hız
    [SerializeField] float normalYercekimi = 3f;  // süzülmezken yerçekimi çarpanı

    [Header("Süzülme (düşerken basılı tut)")]
    [SerializeField] float suzulmeYercekimi = 0.5f; // süzülürken düşük yerçekimi: yavaş düşüş

    [Header("Bağlantılar")]
    [SerializeField] SpriteRenderer gorsel; // süzülünce rengi değişen görsel
    [SerializeField] TekTusOyun oyun;       // engele çarpınca kaybettirmek için

    Rigidbody2D rb;   // karakterin fizik gövdesi
    bool yerde;       // zemine değiyor mu
    bool ziplaIstegi; // Update'te basıldı, FixedUpdate'te zıplanacak
    bool tusBasili;   // tuş şu an basılı mı (süzülme için)

    // Fizik gövdesini bir kez bulur.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Girdi Update'te okunur: wasPressedThisFrame yalnızca tek karede doğrudur.
    void Update()
    {
        if (oyun.Bitti) return;

        // Boşluk ya da sol tık aynı tuş sayılır.
        Keyboard klavye = Keyboard.current;
        Mouse fare = Mouse.current;
        bool basildi = (klavye != null && klavye.spaceKey.wasPressedThisFrame)
                    || (fare != null && fare.leftButton.wasPressedThisFrame);
        tusBasili = (klavye != null && klavye.spaceKey.isPressed)
                 || (fare != null && fare.leftButton.isPressed);

        // Yalnızca yerdeyken basış zıplama isteği olur.
        if (basildi && yerde) ziplaIstegi = true;
    }

    // Rigidbody2D'ye fizik adımında dokunulur.
    void FixedUpdate()
    {
        // Bekleyen zıplama isteğini uygula.
        if (ziplaIstegi)
        {
            rb.linearVelocityY = ziplamaHizi;
            ziplaIstegi = false;
            yerde = false;
        }

        // Havada, düşerken ve tuş basılıysa süzül: yerçekimi azalır, renk maviye döner.
        bool suzuluyor = !yerde && tusBasili && rb.linearVelocityY < 0f;
        rb.gravityScale = suzuluyor ? suzulmeYercekimi : normalYercekimi;
        gorsel.color = suzuluyor ? new Color(0.5f, 0.9f, 1f) : Color.white;
    }

    // Zemine değince yere inmiş sayılır, engele değince oyun biter.
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Zemin")) yerde = true;
        else if (c.gameObject.CompareTag("Engel")) oyun.Kaybet();
    }
}
