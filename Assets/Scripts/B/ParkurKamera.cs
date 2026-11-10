using UnityEngine;

/// Oyun B: karakteri yumuşak izleyen kamera.
public class ParkurKamera : MonoBehaviour
{
    [SerializeField] Transform hedef;      // izlenecek karakter
    [SerializeField] float takipHizi = 6f; // büyüdükçe kamera daha sıkı izler
    [SerializeField] float yOfset = 1.5f;  // kamera karakterin bu kadar üstüne bakar

    // Karakter hareketini bitirdikten sonra kamerayı ona doğru yumuşakça kaydırır.
    void LateUpdate()
    {
        Vector3 istenen = new Vector3(hedef.position.x, hedef.position.y + yOfset, transform.position.z);
        // Üstel yumuşatma: kare hızından bağımsız aynı takip hissi.
        transform.position = Vector3.Lerp(transform.position, istenen, 1f - Mathf.Exp(-takipHizi * Time.deltaTime));
    }
}
