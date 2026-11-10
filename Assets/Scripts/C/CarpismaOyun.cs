using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// Oyun C: can, skor, kazanma ve yeniden başlatma.
public class CarpismaOyun : MonoBehaviour
{
    [SerializeField] Top top;     // can gidince raketin üstüne geri konacak top
    [SerializeField] int can = 3; // kaç kez top kaçırılabilir

    int kalan;   // kırılmayı bekleyen tuğla sayısı
    int skor;    // kırılan her tuğla 10 puan
    bool bitti;  // oyun bitti mi (kazanma ya da kaybetme)

    // Zamanın normal aktığından emin olur.
    void Awake()
    {
        Time.timeScale = 1f;
    }

    // R tuşuyla sahneyi baştan yükler.
    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k != null && k.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Duvar örülünce toplam tuğla sayısını bildirir.
    public void ToplamTugla(int adet)
    {
        kalan = adet;
    }

    // Bir tuğla kırılınca skoru artırır; tuğla kalmadıysa oyun kazanılır.
    public void TuglaKirildi()
    {
        skor += 10;
        kalan--;
        if (kalan <= 0) OyunuBitir();
    }

    // Top alttan kaçınca can azalır; can bittiyse oyun biter, yoksa top yeniden hazırlanır.
    public void TopKacti()
    {
        can--;
        if (can <= 0) OyunuBitir();
        else top.Sifirla();
    }

    // Oyunu bitirir ve zamanı dondurur.
    void OyunuBitir()
    {
        bitti = true;
        Time.timeScale = 0f;
    }

    // Ekrana skor, can, kontroller ve bitiş yazısını çizer.
    void OnGUI()
    {
        GUIStyle yazi = new GUIStyle(GUI.skin.label) { fontSize = 24 };
        GUI.Label(new Rect(20, 15, 1000, 40), "Skor: " + skor + "  Can: " + can + "  ·  A/D: raket · Boşluk: fırlat · R: baştan", yazi);
        if (bitti)
            GUI.Label(new Rect(20, 300, 1000, 40), (kalan <= 0 ? "KAZANDIN!" : "OYUN BİTTİ.") + " R ile yeniden başla.", yazi);
    }
}
