using UnityEngine;

/// Oyun C: tetikleyici. Top içinden geçer, yalnızca can eksilir; sekme olmaz.
public class OlumBolgesi : MonoBehaviour
{
    [SerializeField] CarpismaOyun oyun; // top kaçınca haber verilecek yönetici

    // Bölgeye giren nesne topsa bir can gider.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.GetComponent<Top>() != null) oyun.TopKacti();
    }
}
