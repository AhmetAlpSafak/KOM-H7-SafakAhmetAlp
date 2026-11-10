using UnityEngine;

/// Oyun D: eğik kamera düz zemini yokuş gibi gösterir; darbelerde hafifçe sarsılır.
public class SisifosKamera : MonoBehaviour
{
    [SerializeField] Transform oyuncu;                          // izlenen Sisifos
    [SerializeField] Vector3 ofset = new Vector3(4f, 3f, -10f); // kamera oyuncunun biraz önüne bakar
    [SerializeField] float takipHizi = 6f;                      // büyüdükçe kamera daha sıkı izler

    Vector3 konum;        // sarsıntısız kamera konumu
    float sarsintiKalan;  // sarsıntının bitmesine kalan süre
    float sarsintiGucu;   // sarsıntının genliği

    // Başlangıç konumunu kaydeder.
    void Start()
    {
        konum = transform.position;
    }

    // Oyuncuyu yatayda yumuşakça izler, varsa üstüne sarsıntı ekler.
    void LateUpdate()
    {
        // Yalnızca x izlenir; yükseklik sabit kalır. Gerçek zaman kullanılır: oyun dursa da çalışır.
        Vector3 hedef = new Vector3(oyuncu.position.x + ofset.x, ofset.y, ofset.z);
        konum = Vector3.Lerp(konum, hedef, 1f - Mathf.Exp(-takipHizi * Time.unscaledDeltaTime));

        // Sarsıntı süresi bitene kadar her karede rastgele küçük bir kayma ekle.
        Vector3 sarsinti = Vector3.zero;
        if (sarsintiKalan > 0f)
        {
            sarsintiKalan -= Time.unscaledDeltaTime;
            sarsinti = Random.insideUnitCircle * sarsintiGucu;
        }
        transform.position = konum + sarsinti;
    }

    // Diğer betikler darbe anında çağırır: verilen güç ve sürede sarsıntı başlatır.
    public void Sars(float guc, float sure)
    {
        sarsintiGucu = guc;
        sarsintiKalan = sure;
    }
}
