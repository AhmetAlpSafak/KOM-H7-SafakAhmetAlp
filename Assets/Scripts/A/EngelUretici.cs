using UnityEngine;

/// Oyun A: kısa ve uzun engeller üretir. Zorluk iki sayıyla artar: hız ve aralık.
public class EngelUretici : MonoBehaviour
{
    [SerializeField] TekTusEngel engelPrefab; // kopyalanacak engel kalıbı
    [SerializeField] TekTusOyun oyun;         // oyun bitti mi diye sorulur
    [SerializeField] Transform karakter;      // engel, karakteri geçti mi diye buna bakar

    [Header("Zorluk")]
    [SerializeField] float baslangicHizi = 6f;    // ilk engellerin kayma hızı
    [SerializeField] float hizArtisi = 0.08f;     // her engelde
    [SerializeField] float azamiHiz = 12f;        // hız bundan fazla artmaz
    [SerializeField] float enAzAralik = 1.1f;     // iki engel arası en kısa bekleme (saniye)
    [SerializeField] float enCokAralik = 1.9f;    // iki engel arası en uzun bekleme (saniye)
    [SerializeField] float aralikCarpani = 0.992f; // her engelde aralık biraz daralır

    [Header("Engel şekli")]
    [SerializeField] float zeminUstY = -3f;            // zeminin üst kenarının yüksekliği
    [SerializeField] float dogmaX = 12f;               // engellerin ekranın sağında doğduğu yer
    [SerializeField] float uzunEngelOlasiligi = 0.3f;  // bir engelin uzun çıkma ihtimali (0-1)
    [SerializeField] float uzunGenislikCarpani = 0.7f; // genişlik = hız × çarpan: süzülmeden geçilemez

    public float Hiz { get; private set; } // bütün engeller bu hızı okur
    float sayac = 1f;  // sıradaki engele kalan süre
    float carpan = 1f; // aralığı zamanla küçülten toplam çarpan

    // Oyun başında hızı başlangıç değerine ayarlar.
    void Awake()
    {
        Hiz = baslangicHizi;
    }

    // Süre dolunca yeni engel üretir ve zorluğu bir adım artırır.
    void Update()
    {
        if (oyun.Bitti) return;

        // Sayaç bitmediyse bu kare engel yok.
        sayac -= Time.deltaTime;
        if (sayac > 0f) return;

        // Engelin türünü ve boyutunu seç: uzun engel alçak ve geniş, kısa engel yüksek ve dar.
        bool uzun = Random.value < uzunEngelOlasiligi;
        float genislik = uzun ? Hiz * uzunGenislikCarpani : 1f;
        float yukseklik = uzun ? 1f : 1.3f;

        // Engeli zeminin üstüne, sol kenarı doğma noktasında olacak şekilde oluştur.
        Vector3 konum = new Vector3(dogmaX + genislik * 0.5f, zeminUstY + yukseklik * 0.5f, 0f);
        TekTusEngel engel = Instantiate(engelPrefab, konum, Quaternion.identity);
        engel.transform.localScale = new Vector3(genislik, yukseklik, 1f);
        engel.Baslat(this, oyun, karakter);

        // Zorluğu artır: hız biraz yükselir, aralık biraz daralır.
        Hiz = Mathf.Min(Hiz + hizArtisi, azamiHiz);
        carpan *= aralikCarpani;

        // Sıradaki engelin bekleme süresi; uzun engelden sonra geçmek için ek süre verilir.
        float bekleme = Random.Range(enAzAralik, enCokAralik) * carpan;
        if (uzun) bekleme += genislik / Hiz + 0.6f;
        sayac = bekleme;
    }
}
