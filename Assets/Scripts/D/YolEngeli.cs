using UnityEngine;

/// Oyun D: kaya yeterince hızlı çarparsa kırılır ve arkasında enkaz bırakır.
public class YolEngeli : MonoBehaviour
{
    [SerializeField] float kirmaEsigi = 6f;  // kırmak için gereken en az çarpma hızı (eşikte 999: kırılmaz)
    [SerializeField] GameObject enkazPrefab; // kırılınca yere bırakılan enkaz

    bool kirildi; // aynı engel iki kez kırılmasın

    /// Kırıldıysa true döner. İnişte kaya her engeli kırar.
    public bool Carp(float hiz, bool inis)
    {
        if (kirildi) return true;
        // Tırmanışta yavaş çarpan kaya engeli kıramaz.
        if (!inis && hiz < kirmaEsigi) return false;

        // Engeli kır: yerine enkaz koy, engeli sil.
        kirildi = true;
        Vector3 konum = new Vector3(transform.position.x, enkazPrefab.transform.position.y, 0f);
        Instantiate(enkazPrefab, konum, Quaternion.identity);
        Destroy(gameObject);
        return true;
    }
}
