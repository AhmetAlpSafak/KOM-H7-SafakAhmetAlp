using UnityEngine;

/// Oyun B: bir ayar profili. Aynı kod, farklı sayılarla başka bir karakter olur.
[System.Serializable]
public class HareketProfili
{
    public string ad = "Profil";        // ekranda görünen ad
    public Color renk = Color.white;    // karakterin bu profildeki rengi
    public float azamiHiz = 8f;         // en yüksek yürüme hızı
    public float ivme = 60f;            // tuşa basınca hızlanma oranı
    public float fren = 80f;            // tuş bırakılınca yavaşlama oranı
    public float ziplamaHizi = 13f;     // zıplamada verilen yukarı hız
    public float kisaZiplama = 0.5f;    // tuş erken bırakılınca kalan yükselme oranı
    public float coyoteSuresi = 0.1f;   // kenardan düştükten sonra zıplama hakkı (saniye)
    public float tamponSuresi = 0.12f;  // erken basılan zıplamanın saklanma süresi (saniye)
    public float dususCarpani = 1.3f;   // düşerken yerçekimi kaç kat artar
}
