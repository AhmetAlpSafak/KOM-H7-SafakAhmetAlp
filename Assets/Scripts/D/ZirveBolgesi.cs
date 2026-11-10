using UnityEngine;

/// Oyun D: kaya bu tetikleyiciye girince zirve sayılır ve iniş başlar.
public class ZirveBolgesi : MonoBehaviour
{
    [SerializeField] SisifosOyun oyun; // zirve haberi verilecek yönetici
    [SerializeField] Kaya kaya;        // kayanın durumu kontrol edilir

    // Tırmanan kaya sarı direğe girince zirve sayılır (inişteyken tekrar sayılmaz).
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.CompareTag("Kaya") && kaya.Durum == KayaDurumu.Tirmanis) oyun.ZirveyeUlasti();
    }
}
