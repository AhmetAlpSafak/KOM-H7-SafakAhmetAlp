using UnityEngine;
using UnityEngine.InputSystem;

/// Oyun C: konumu kodla taşınan (Kinematic) raket.
[RequireComponent(typeof(Rigidbody2D))]
public class Raket : MonoBehaviour
{
    [SerializeField] float hiz = 14f;     // raketin yatay hızı
    [SerializeField] float sinirX = 6.7f; // raket bu x değerinin dışına çıkamaz

    Rigidbody2D rb; // raketin fizik gövdesi (Kinematic)
    float girdi;    // -1 sol, 0 yok, +1 sağ

    // Fizik gövdesini bir kez bulur.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // A/D veya ok tuşlarından yatay girdiyi okur.
    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;
        girdi = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) girdi -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) girdi += 1f;
    }

    // Raketi girdi yönünde taşır; duvarların dışına çıkmasını engeller.
    void FixedUpdate()
    {
        float x = Mathf.Clamp(rb.position.x + girdi * hiz * Time.fixedDeltaTime, -sinirX, sinirX);
        rb.MovePosition(new Vector2(x, rb.position.y)); // Kinematic gövde MovePosition ile taşınır
    }
}
