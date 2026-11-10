using UnityEngine;

/// Oyun B: içinden geçilen bitiş bölgesi (tetikleyici).
public class BitisBayragi : MonoBehaviour
{
    [SerializeField] ParkurOyun oyun; // bitiş haberi verilecek oyun yöneticisi

    // Oyuncu bayrağın içine girince parkur biter.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.CompareTag("Player")) oyun.Bitir();
    }
}
