using UnityEngine;

/// Oyun C: tek prefab'tan iç içe döngüyle duvar örer.
public class TuglaDuvari : MonoBehaviour
{
    [SerializeField] Tugla tuglaPrefab;   // kopyalanacak tuğla kalıbı
    [SerializeField] CarpismaOyun oyun;   // toplam tuğla sayısı bildirilecek yönetici
    [SerializeField] int satir = 5;       // duvarın satır sayısı
    [SerializeField] int sutun = 10;      // duvarın sütun sayısı
    [SerializeField] Vector2 baslangic = new Vector2(-6.75f, 4f); // sol üst tuğlanın konumu
    [SerializeField] Vector2 aralik = new Vector2(1.5f, 0.6f);    // tuğlalar arası mesafe

    // Oyun başında satır satır, sütun sütun tuğlaları dizer ve türlerini atar.
    void Start()
    {
        for (int y = 0; y < satir; y++)
        {
            for (int x = 0; x < sutun; x++)
            {
                Vector2 p = baslangic + new Vector2(x * aralik.x, -y * aralik.y);
                Tugla t = Instantiate(tuglaPrefab, p, Quaternion.identity, transform);
                t.Kur(TurSec(x, y), oyun);
            }
        }
        oyun.ToplamTugla(satir * sutun);
    }

    // Tuğlanın yerine göre türünü seçer: üst sıra iki vuruşluk, bazı tek sıralar patlayan.
    TuglaTuru TurSec(int x, int y)
    {
        if (y == 0) return TuglaTuru.IkiVurus;
        if (y % 2 == 1 && x % 3 == 1) return TuglaTuru.Patlayan;
        return TuglaTuru.Normal;
    }
}
