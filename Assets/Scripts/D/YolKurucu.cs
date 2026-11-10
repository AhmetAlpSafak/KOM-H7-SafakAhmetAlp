using UnityEngine;

/// Oyun D: her turda engelleri boş yuvalara rastgele yerleştirir.
public class YolKurucu : MonoBehaviour
{
    [SerializeField] YolEngeli barikatPrefab;     // kırılabilen engel
    [SerializeField] YolEngeli esikPrefab;        // kırılmayan, üstünden aşılması gereken engel
    [SerializeField] Transform oyuncu;            // engel oyuncunun üstüne kurulmasın
    [SerializeField] float ilkYuvaX = 16f;        // ilk yuvanın x konumu
    [SerializeField] float yuvaAraligi = 8f;      // yuvalar arası mesafe
    [SerializeField] int yuvaSayisi = 9;          // yoldaki toplam yuva
    [SerializeField] float oyuncudanEnAz = 3f;    // oyuncuya bundan yakın yuva boş bırakılır

    // Eski engelleri siler, istenen sayıda engeli rastgele yuvalara kurar.
    public void Kur(int adet)
    {
        foreach (Transform eski in transform) Destroy(eski.gameObject);

        // Yuva numaralarını karıştır (Fisher-Yates): ilk "adet" kadarı dolu olacak.
        int[] yuvalar = new int[yuvaSayisi];
        for (int i = 0; i < yuvaSayisi; i++) yuvalar[i] = i;
        for (int i = yuvaSayisi - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (yuvalar[i], yuvalar[j]) = (yuvalar[j], yuvalar[i]);
        }

        int kurulacak = Mathf.Min(adet, yuvaSayisi);
        for (int i = 0; i < kurulacak; i++)
        {
            // Oyuncunun dibindeki yuvayı atla.
            float x = ilkYuvaX + yuvalar[i] * yuvaAraligi;
            if (Mathf.Abs(x - oyuncu.position.x) < oyuncudanEnAz) continue;

            // Her üç engelden biri eşik, diğerleri barikat.
            YolEngeli prefab = i % 3 == 2 ? esikPrefab : barikatPrefab;
            Vector3 konum = new Vector3(x, prefab.transform.position.y, 0f);
            Instantiate(prefab, konum, Quaternion.identity, transform);
        }
    }
}
