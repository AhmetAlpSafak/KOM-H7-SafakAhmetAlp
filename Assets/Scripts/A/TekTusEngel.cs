using UnityEngine;

/// Oyun A: sola kayan engel. Hızı üreticiden okur, böylece hepsi aynı hızda gider.
[RequireComponent(typeof(Rigidbody2D))]
public class TekTusEngel : MonoBehaviour
{
    Rigidbody2D rb;        // engeli hareket ettiren fizik gövdesi
    EngelUretici uretici;  // ortak hız buradan okunur
    TekTusOyun oyun;       // engel aşılınca skor için haber verilir
    Transform oyuncu;      // aşıldı mı kontrolü için karakterin konumu
    bool asildi;           // skor aynı engel için iki kez sayılmasın

    // Üretici, engeli oluşturduktan hemen sonra bağlantıları buradan verir.
    public void Baslat(EngelUretici u, TekTusOyun o, Transform karakter)
    {
        uretici = u;
        oyun = o;
        oyuncu = karakter;
        rb = GetComponent<Rigidbody2D>();
    }

    // Engeli sola kaydırır, aşıldıysa skoru artırır, ekrandan çıkınca siler.
    void FixedUpdate()
    {
        rb.linearVelocityX = -uretici.Hiz;

        // Engelin sağ kenarı karakterin gerisine geçtiyse engel aşılmıştır.
        float sagKenar = transform.position.x + transform.localScale.x * 0.5f;
        if (!asildi && sagKenar < oyuncu.position.x)
        {
            asildi = true;
            oyun.EngelAsildi();
        }
        // Ekranın solundan çıkan engel silinir.
        if (sagKenar < -14f) Destroy(gameObject);
    }
}
