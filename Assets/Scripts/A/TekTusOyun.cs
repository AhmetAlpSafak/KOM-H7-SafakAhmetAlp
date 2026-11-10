using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// Oyun A: skor, başarısızlık ve aynı tuşla yeniden başlatma.
public class TekTusOyun : MonoBehaviour
{
    [SerializeField] float yenidenBaslamaBeklemesi = 0.4f; // ölünce yanlışlıkla hemen yeniden başlamasın

    public bool Bitti { get; private set; } // oyun bitti mi; diğer betikler okur
    int asilanEngel; // skor
    float bitisAni;  // oyunun bittiği an (gerçek zamanla)

    // Önceki oyundan durmuş zaman kalmasın diye zamanı normale çevirir.
    void Awake()
    {
        Time.timeScale = 1f;
    }

    // Oyun bittiyse ve bekleme geçtiyse, basışla sahneyi baştan yükler.
    void Update()
    {
        if (!Bitti || Time.unscaledTime - bitisAni < yenidenBaslamaBeklemesi) return;

        Keyboard klavye = Keyboard.current;
        Mouse fare = Mouse.current;
        bool basildi = (klavye != null && klavye.spaceKey.wasPressedThisFrame)
                    || (fare != null && fare.leftButton.wasPressedThisFrame);
        if (basildi) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Engel aşılınca skoru bir artırır.
    public void EngelAsildi()
    {
        asilanEngel++;
    }

    // Oyunu bitirir ve zamanı dondurur.
    public void Kaybet()
    {
        if (Bitti) return;
        Bitti = true;
        bitisAni = Time.unscaledTime;
        Time.timeScale = 0f;
    }

    // Ekrana skor, kontroller ve bitiş yazısını çizer.
    void OnGUI()
    {
        GUIStyle yazi = new GUIStyle(GUI.skin.label) { fontSize = 24 };
        GUI.Label(new Rect(20, 15, 900, 40), "Aşılan engel: " + asilanEngel, yazi);
        GUI.Label(new Rect(20, 50, 900, 40), "Boşluk/tık: zıpla · Düşerken basılı tut: süzül", yazi);
        if (Bitti) GUI.Label(new Rect(20, 95, 900, 40), "ÇARPTIN. Yeniden başlamak için bas.", yazi);
    }
}
