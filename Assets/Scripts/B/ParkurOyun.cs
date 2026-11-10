using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// Oyun B: süre, düşme sayısı, bitiş ve yeniden başlatma.
public class ParkurOyun : MonoBehaviour
{
    [SerializeField] HareketHissi karakter;                        // izlenen oyuncu
    [SerializeField] Vector2 baslangic = new Vector2(-7f, -2.3f);  // düşünce dönülen nokta
    [SerializeField] float olumY = -12f;                           // bu yüksekliğin altı düşme sayılır

    float sure;  // parkurda geçen süre
    int dusme;   // kaç kez düşüldü
    bool bitti;  // bitiş bayrağına ulaşıldı mı

    // Zamanın normal aktığından emin olur.
    void Awake()
    {
        Time.timeScale = 1f;
    }

    // R ile yeniden başlatır, süreyi sayar, düşen karakteri başa ışınlar.
    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k != null && k.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (bitti) return;
        sure += Time.deltaTime;

        // Karakter haritanın altına düştüyse sayacı artır ve başa döndür.
        if (karakter.transform.position.y < olumY)
        {
            dusme++;
            karakter.Isinla(baslangic);
        }
    }

    // Bitiş bayrağı çağırır: süre durur.
    public void Bitir()
    {
        bitti = true;
    }

    // Ekrana süre, düşme sayısı, profil ve kontrolleri yazar.
    void OnGUI()
    {
        GUIStyle yazi = new GUIStyle(GUI.skin.label) { fontSize = 24 };
        GUI.Label(new Rect(20, 15, 1000, 40), "Süre: " + sure.ToString("0.0") + "  Düşme: " + dusme + "  Profil: " + karakter.ProfilAdi, yazi);
        GUI.Label(new Rect(20, 50, 1000, 40), "A/D: yürü · Boşluk: zıpla (tutma süresi yükseklik) · E: profil · R: baştan", yazi);
        if (bitti) GUI.Label(new Rect(20, 95, 1000, 40), "BİTTİ! Diğer profille denemek için E ve R.", yazi);
    }
}
