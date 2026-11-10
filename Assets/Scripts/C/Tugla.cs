using UnityEngine;

// Tuğla türleri: tek vuruşta kırılan, iki vuruşta kırılan ve çevresini patlatan.
public enum TuglaTuru { Normal, IkiVurus, Patlayan }

/// Oyun C: tek prefab, üç davranış. Özel tür: Patlayan kırılınca çevresini de kırar.
public class Tugla : MonoBehaviour
{
    [SerializeField] float patlamaYaricapi = 1.7f; // Patlayan tuğlanın etki alanı

    TuglaTuru tur;          // bu tuğlanın türü
    int kalanVurus = 1;     // kırılmak için gereken vuruş sayısı
    bool kirildi;           // zincirleme patlamada iki kez kırılmasın
    CarpismaOyun oyun;      // skor için haber verilecek yönetici
    SpriteRenderer gorsel;  // türe göre boyanan görsel

    // Duvar örülürken türü atar, vuruş sayısını ve rengi ayarlar.
    public void Kur(TuglaTuru yeniTur, CarpismaOyun o)
    {
        tur = yeniTur;
        oyun = o;
        gorsel = GetComponent<SpriteRenderer>();
        kalanVurus = tur == TuglaTuru.IkiVurus ? 2 : 1;
        // Mavi: iki vuruş, kırmızı: patlayan, yeşil: normal.
        gorsel.color = tur == TuglaTuru.IkiVurus ? new Color(0.3f, 0.6f, 1f)
                     : tur == TuglaTuru.Patlayan ? new Color(1f, 0.35f, 0.25f)
                     : new Color(0.55f, 0.85f, 0.5f);
    }

    // Top çarpınca bir vuruş düşer; vuruş bittiyse kırılır, bitmediyse çatlar.
    public void Vur()
    {
        kalanVurus--;
        if (kalanVurus <= 0) Kir();
        else gorsel.color = new Color(0.65f, 0.8f, 1f); // çatladı
    }

    // Tuğlayı kırar; Patlayan türse çevresindeki tuğlaları da kırar.
    void Kir()
    {
        if (kirildi) return;
        kirildi = true;
        oyun.TuglaKirildi();

        if (tur == TuglaTuru.Patlayan)
        {
            // Yarıçap içindeki bütün tuğlaları bul ve kır (zincirleme olabilir).
            foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, patlamaYaricapi))
                if (c.CompareTag("Tugla")) c.GetComponent<Tugla>().Kir();
        }
        Destroy(gameObject);
    }
}
