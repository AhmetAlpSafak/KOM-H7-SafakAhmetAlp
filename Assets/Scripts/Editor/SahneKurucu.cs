using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// KOM menüsünden dört sahneyi, prefabları ve ayarları baştan üretir.
public static class SahneKurucu
{
    // Üretilen varlıkların kaydedildiği klasörler.
    const string SpriteKlasoru = "Assets/Sprites";
    const string FizikKlasoru = "Assets/Fizik";
    const string PrefabKlasoru = "Assets/Prefabs";
    const string SahneKlasoru = "Assets/Scenes";
    // URP'nin ışıksız sprite malzemesi: 2D ışık kurmadan sprite'lar görünsün.
    const string UnlitMalzeme = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat";

    static Sprite kare, daire;                          // bütün görsellerde kullanılan iki şekil
    static Material malzeme;                            // sprite malzemesi
    static PhysicsMaterial2D kaygan, seken, kayaYuzeyi; // sürtünmesiz, tam seken ve az seken yüzeyler
    static int zeminKatmani;                            // "Zemin" katmanının numarası

    // Menüden çalışır: etiketleri, varlıkları ve dört sahneyi baştan üretir.
    [MenuItem("KOM/Tum Sahneleri Kur")]
    public static void HepsiniKur()
    {
        EtiketVeKatmanEkle();
        VarliklariHazirla();

        OyunA();
        OyunB();
        OyunC();
        OyunD();

        // Unity'nin örnek sahnesini sil, build listesini D başta olacak şekilde ayarla.
        AssetDatabase.DeleteAsset(SahneKlasoru + "/SampleScene.unity");
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(SahneYolu("D_Birlesik"), true),
            new EditorBuildSettingsScene(SahneYolu("A_TekTus"), true),
            new EditorBuildSettingsScene(SahneYolu("B_HareketHissi"), true),
            new EditorBuildSettingsScene(SahneYolu("C_Carpisma"), true),
        };
        AssetDatabase.SaveAssets();
        Debug.Log("KOM: dört sahne kuruldu.");
    }

    // ---------- Oyun A ----------
    // Tek tuş sahnesi: zemin, engel prefabı, karakter ve yönetici.
    static void OyunA()
    {
        YeniSahne(5f, new Color(0.13f, 0.15f, 0.22f), Vector3.zero);
        Kutu("Zemin", new Vector3(0f, -4f), new Vector2(40f, 2f), new Color(0.35f, 0.38f, 0.45f), "Zemin", zeminKatmani);

        // Engel prefabı: kodla kaydırıldığı için Kinematic gövde.
        GameObject temp = new GameObject("A_Engel");
        Gorsellestir(temp, kare, new Color(0.95f, 0.4f, 0.35f));
        temp.tag = "Engel";
        Rigidbody2D erb = temp.AddComponent<Rigidbody2D>();
        erb.bodyType = RigidbodyType2D.Kinematic;
        temp.AddComponent<BoxCollider2D>().size = Vector2.one;
        temp.AddComponent<TekTusEngel>();
        TekTusEngel engelPrefab = PrefabYap(temp, "A_Engel").GetComponent<TekTusEngel>();

        // Karakter ve oyun yöneticisi.
        GameObject karakter = Karakter("Karakter", new Vector3(-5f, -2.45f), new Vector2(1f, 1f), out SpriteRenderer gorsel, out _);
        karakter.GetComponent<Rigidbody2D>().gravityScale = 3f;
        TekTusKarakter kod = karakter.AddComponent<TekTusKarakter>();

        GameObject yonetici = new GameObject("Oyun");
        TekTusOyun oyun = yonetici.AddComponent<TekTusOyun>();
        EngelUretici uretici = yonetici.AddComponent<EngelUretici>();

        // Inspector bağlantılarını doldur ve kaydet.
        Ata(kod, "gorsel", gorsel);
        Ata(kod, "oyun", oyun);
        Ata(uretici, "engelPrefab", engelPrefab);
        Ata(uretici, "oyun", oyun);
        Ata(uretici, "karakter", karakter.transform);
        SahneyiKaydet("A_TekTus");
    }

    // ---------- Oyun B ----------
    // Parkur sahnesi: platformlar, karakter, bitiş bayrağı ve takip kamerası.
    static void OyunB()
    {
        Camera kamera = YeniSahne(6f, new Color(0.12f, 0.17f, 0.2f), new Vector3(-7f, 0f));
        // Her satır bir platform: { sol kenar, sağ kenar, üst yüzey yüksekliği }.
        float[,] platformlar =
        {
            { -9f, -1f, -3f }, { 2f, 6f, -2f }, { 9.8f, 15f, -2f }, { 18f, 20f, -0.8f },
            { 22.5f, 25f, -2.2f }, { 28f, 29.5f, -0.8f }, { 32f, 40f, -2f },
        };
        for (int i = 0; i < platformlar.GetLength(0); i++)
        {
            float sol = platformlar[i, 0], sag = platformlar[i, 1], ust = platformlar[i, 2];
            Kutu("Platform" + i, new Vector3((sol + sag) * 0.5f, ust - 0.5f), new Vector2(sag - sol, 1f),
                 new Color(0.4f, 0.48f, 0.42f), "Zemin", zeminKatmani);
        }
        Kutu("SolDuvar", new Vector3(-9.5f, 5f), new Vector2(1f, 30f), new Color(0.3f, 0.33f, 0.36f), "Untagged", 0);

        GameObject karakter = Karakter("Karakter", new Vector3(-7f, -2.3f), new Vector2(0.8f, 1.2f), out SpriteRenderer gorsel, out Transform ayak);
        HareketHissi kod = karakter.AddComponent<HareketHissi>();

        GameObject yonetici = new GameObject("Oyun");
        ParkurOyun oyun = yonetici.AddComponent<ParkurOyun>();

        // Bitiş bayrağı: içinden geçilen tetikleyici.
        GameObject bayrak = Gorsel("Bitis", new Vector3(38f, 0f), new Vector2(0.5f, 4f), new Color(0.4f, 1f, 0.5f), kare);
        BoxCollider2D tetik = bayrak.AddComponent<BoxCollider2D>();
        tetik.size = Vector2.one;
        tetik.isTrigger = true;
        BitisBayragi bitis = bayrak.AddComponent<BitisBayragi>();

        ParkurKamera takip = kamera.gameObject.AddComponent<ParkurKamera>();

        Ata(kod, "ayak", ayak);
        Ata(kod, "gorsel", gorsel);
        Maske(kod, "zeminKatmani", 1 << zeminKatmani);
        Ata(oyun, "karakter", kod);
        Ata(bitis, "oyun", oyun);
        Ata(takip, "hedef", karakter.transform);
        SahneyiKaydet("B_HareketHissi");
    }

    // ---------- Oyun C ----------
    // Tuğla kırma sahnesi: seken duvarlar, raket, top, tuğla prefabı ve ölüm bölgesi.
    static void OyunC()
    {
        YeniSahne(5.6f, new Color(0.1f, 0.11f, 0.17f), Vector3.zero);
        // Üç kenar seken duvar; alt kenar açık (ölüm bölgesi).
        Color duvar = new Color(0.32f, 0.34f, 0.42f);
        SekenDuvar("SolDuvar", new Vector3(-8.25f, 0f), new Vector2(0.5f, 12f), duvar);
        SekenDuvar("SagDuvar", new Vector3(8.25f, 0f), new Vector2(0.5f, 12f), duvar);
        SekenDuvar("Tavan", new Vector3(0f, 5.25f), new Vector2(17f, 0.5f), duvar);

        // Raket: kodla taşınan Kinematic gövde.
        GameObject raket = Kutu("Raket", new Vector3(0f, -4.5f), new Vector2(2.4f, 0.4f), Color.white, "Raket", 0);
        Rigidbody2D rrb = raket.AddComponent<Rigidbody2D>();
        rrb.bodyType = RigidbodyType2D.Kinematic;
        rrb.interpolation = RigidbodyInterpolation2D.Interpolate;
        raket.AddComponent<Raket>();

        // Top: yerçekimsiz, hızlıyken içinden geçmesin diye Continuous çarpışma.
        GameObject top = Gorsel("Top", new Vector3(0f, -3.9f), new Vector2(0.4f, 0.4f), new Color(1f, 0.9f, 0.4f), daire);
        Rigidbody2D trb = top.AddComponent<Rigidbody2D>();
        trb.gravityScale = 0f;
        trb.freezeRotation = true;
        trb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        trb.interpolation = RigidbodyInterpolation2D.Interpolate;
        trb.sharedMaterial = seken;
        CircleCollider2D tc = top.AddComponent<CircleCollider2D>();
        tc.radius = 0.5f;
        tc.sharedMaterial = seken;
        Top topKod = top.AddComponent<Top>();

        // Tek tuğla prefabı; türü duvar örülürken atanır.
        GameObject tempTugla = new GameObject("C_Tugla");
        tempTugla.transform.localScale = new Vector3(1.4f, 0.5f, 1f);
        Gorsellestir(tempTugla, kare, Color.white);
        tempTugla.tag = "Tugla";
        BoxCollider2D tb = tempTugla.AddComponent<BoxCollider2D>();
        tb.size = Vector2.one;
        tb.sharedMaterial = seken;
        tempTugla.AddComponent<Tugla>();
        Tugla tuglaPrefab = PrefabYap(tempTugla, "C_Tugla").GetComponent<Tugla>();

        GameObject yonetici = new GameObject("Oyun");
        CarpismaOyun oyun = yonetici.AddComponent<CarpismaOyun>();

        GameObject duvarKoku = new GameObject("TuglaDuvari");
        TuglaDuvari duvarKod = duvarKoku.AddComponent<TuglaDuvari>();

        // Ölüm bölgesi: raketin altındaki görünmez tetikleyici.
        GameObject olum = new GameObject("OlumBolgesi");
        olum.transform.position = new Vector3(0f, -6.5f);
        BoxCollider2D oc = olum.AddComponent<BoxCollider2D>();
        oc.size = new Vector2(20f, 1f);
        oc.isTrigger = true;
        OlumBolgesi olumKod = olum.AddComponent<OlumBolgesi>();

        Ata(topKod, "raket", raket.transform);
        Ata(oyun, "top", topKod);
        Ata(duvarKod, "tuglaPrefab", tuglaPrefab);
        Ata(duvarKod, "oyun", oyun);
        Ata(olumKod, "oyun", oyun);
        SahneyiKaydet("C_Carpisma");
    }

    // ---------- Oyun D ----------
    // Sisifos sahnesi: eğik kamera, yol, engel prefabları, oyuncu, kaya, zirve ve yönetici.
    static void OyunD()
    {
        Camera kamera = YeniSahne(8f, new Color(0.16f, 0.13f, 0.2f), new Vector3(5f, 3f));
        kamera.transform.rotation = Quaternion.Euler(0f, 0f, -10f); // sağ taraf yokuş yukarı görünür

        // Uzun düz zemin, iki uç duvarı ve her 10 metrede bir metre çizgisi.
        Kutu("Zemin", new Vector3(50f, -7f), new Vector2(150f, 14f), new Color(0.42f, 0.35f, 0.3f), "Zemin", zeminKatmani);
        Kutu("SolDuvar", new Vector3(-1f, 10f), new Vector2(1f, 40f), new Color(0.3f, 0.25f, 0.25f), "Untagged", 0);
        Kutu("SagDuvar", new Vector3(102f, 10f), new Vector2(1f, 40f), new Color(0.3f, 0.25f, 0.25f), "Untagged", 0);
        for (int metre = 10; metre < 90; metre += 10)
            Gorsel("Metre" + metre, new Vector3(6f + metre, 2f), new Vector2(0.15f, 4f), new Color(0.5f, 0.45f, 0.55f), kare, -1);

        // Prefablar
        // Enkaz: yerdeki ince şerit; uzun tetikleyicisi üstünde duran oyuncuyu algılar.
        GameObject tempEnkaz = new GameObject("D_Enkaz");
        tempEnkaz.transform.position = new Vector3(0f, 0.1f);
        tempEnkaz.transform.localScale = new Vector3(5f, 0.2f, 1f);
        Gorsellestir(tempEnkaz, kare, new Color(0.3f, 0.22f, 0.16f), 1);
        tempEnkaz.tag = "Enkaz";
        BoxCollider2D ec = tempEnkaz.AddComponent<BoxCollider2D>();
        ec.size = new Vector2(1f, 6f);
        ec.offset = new Vector2(0f, 2.5f);
        ec.isTrigger = true;
        GameObject enkazPrefab = PrefabYap(tempEnkaz, "D_Enkaz");

        // Barikat kırılabilir (eşik 6); eşik kırılmaz (999), üstünden aşılmalı.
        YolEngeli barikatPrefab = EngelPrefab("D_Barikat", new Vector2(0.6f, 2.2f), new Color(0.75f, 0.5f, 0.25f), 6f, enkazPrefab);
        YolEngeli esikPrefab = EngelPrefab("D_Esik", new Vector2(1.2f, 1f), new Color(0.55f, 0.55f, 0.6f), 999f, enkazPrefab);

        // Oyuncu ve omuz noktası (gövde merkezinin biraz altında).
        GameObject sisifos = Karakter("Sisifos", new Vector3(1f, 0.76f), new Vector2(0.8f, 1.5f), out SpriteRenderer gorsel, out Transform ayak);
        sisifos.tag = "Player";
        SisifosHareket hareket = sisifos.AddComponent<SisifosHareket>();
        GameObject omuz = new GameObject("OmuzNoktasi");
        omuz.transform.SetParent(sisifos.transform, false);
        omuz.transform.localPosition = new Vector3(0f, -0.25f);

        // Kaya: dönmeyen daire gövde, ayrı döndürülen görsel ve omuzu algılayan biraz büyük Temas tetikleyicisi.
        GameObject kayaGo = new GameObject("Kaya");
        kayaGo.transform.position = new Vector3(6f, 0.91f);
        kayaGo.tag = "Kaya";
        Rigidbody2D krb = kayaGo.AddComponent<Rigidbody2D>();
        krb.gravityScale = 2f;
        krb.freezeRotation = true;
        krb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        krb.interpolation = RigidbodyInterpolation2D.Interpolate;
        krb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        krb.sharedMaterial = kayaYuzeyi;
        CircleCollider2D kc = kayaGo.AddComponent<CircleCollider2D>();
        kc.radius = 0.9f;
        kc.sharedMaterial = kayaYuzeyi;
        GameObject kayaGorsel = Gorsel("Gorsel", Vector3.zero, new Vector2(1.8f, 1.8f), new Color(0.62f, 0.62f, 0.66f), daire, 2, kayaGo.transform);
        Gorsel("Iz", new Vector3(0.28f, 0f), new Vector2(0.35f, 0.1f), new Color(0.3f, 0.3f, 0.35f), kare, 3, kayaGorsel.transform);
        GameObject temas = new GameObject("Temas");
        temas.transform.SetParent(kayaGo.transform, false);
        CircleCollider2D tc = temas.AddComponent<CircleCollider2D>();
        tc.radius = 1.02f;
        tc.isTrigger = true;
        Kaya kaya = kayaGo.AddComponent<Kaya>();

        // Zirve: 90. metredeki sarı direk (tetikleyici).
        GameObject zirve = Gorsel("Zirve", new Vector3(96f, 3f), new Vector2(0.5f, 6f), new Color(1f, 0.85f, 0.3f), kare, -1);
        BoxCollider2D zc = zirve.AddComponent<BoxCollider2D>();
        zc.size = Vector2.one;
        zc.isTrigger = true;
        ZirveBolgesi zirveKod = zirve.AddComponent<ZirveBolgesi>();

        GameObject yonetici = new GameObject("Oyun");
        SisifosOyun oyun = yonetici.AddComponent<SisifosOyun>();
        GameObject yolGo = new GameObject("Yol");
        YolKurucu yol = yolGo.AddComponent<YolKurucu>();
        SisifosKamera takip = kamera.gameObject.AddComponent<SisifosKamera>();

        // Bütün Inspector bağlantılarını doldur ve kaydet.
        Ata(hareket, "ayak", ayak);
        Maske(hareket, "zeminKatmani", 1 << zeminKatmani);
        Ata(hareket, "omuzNoktasi", omuz.transform);
        Ata(hareket, "gorsel", gorsel);
        Ata(hareket, "oyun", oyun);
        Ata(hareket, "kaya", kaya);
        Ata(kaya, "oyun", oyun);
        Ata(kaya, "oyuncu", hareket);
        Ata(kaya, "gorsel", kayaGorsel.transform);
        Ata(zirveKod, "oyun", oyun);
        Ata(zirveKod, "kaya", kaya);
        Ata(oyun, "kaya", kaya);
        Ata(oyun, "yol", yol);
        Ata(oyun, "kamera", takip);
        Ata(yol, "barikatPrefab", barikatPrefab);
        Ata(yol, "esikPrefab", esikPrefab);
        Ata(yol, "oyuncu", sisifos.transform);
        Ata(takip, "oyuncu", sisifos.transform);
        SahneyiKaydet("D_Birlesik");
    }

    // Verilen boyut, renk ve kırılma eşiğiyle bir yol engeli prefabı üretir.
    static YolEngeli EngelPrefab(string ad, Vector2 boyut, Color renk, float esik, GameObject enkaz)
    {
        GameObject temp = new GameObject(ad);
        temp.transform.position = new Vector3(0f, boyut.y * 0.5f);
        temp.transform.localScale = new Vector3(boyut.x, boyut.y, 1f);
        Gorsellestir(temp, kare, renk);
        temp.tag = "Engel";
        temp.layer = zeminKatmani;
        temp.AddComponent<BoxCollider2D>().size = Vector2.one;
        YolEngeli engel = temp.AddComponent<YolEngeli>();
        Ata(engel, "enkazPrefab", enkaz);
        SerializedObject so = new SerializedObject(engel);
        so.FindProperty("kirmaEsigi").floatValue = esik;
        so.ApplyModifiedPropertiesWithoutUndo();
        return PrefabYap(temp, ad).GetComponent<YolEngeli>();
    }

    // ---------- Yardımcılar ----------
    // Boş sahne açar ve verilen ayarlarla ortografik kamera ekler.
    static Camera YeniSahne(float boyut, Color arkaPlan, Vector3 konum)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.transform.position = new Vector3(konum.x, konum.y, -10f);
        Camera kamera = go.AddComponent<Camera>();
        kamera.orthographic = true;
        kamera.orthographicSize = boyut;
        kamera.clearFlags = CameraClearFlags.SolidColor;
        kamera.backgroundColor = arkaPlan;
        go.AddComponent<AudioListener>();
        return kamera;
    }

    // Açık sahneyi Scenes klasörüne verilen adla kaydeder.
    static void SahneyiKaydet(string ad)
    {
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), SahneYolu(ad));
    }

    // Sahne adından dosya yolunu üretir.
    static string SahneYolu(string ad)
    {
        return SahneKlasoru + "/" + ad + ".unity";
    }

    // Nesneye renkli bir SpriteRenderer ekler; sira büyükse önde çizilir.
    static void Gorsellestir(GameObject go, Sprite sprite, Color renk, int sira = 0)
    {
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = renk;
        sr.sharedMaterial = malzeme;
        sr.sortingOrder = sira;
    }

    // Konumu, boyutu ve rengi verilen, çarpıştırıcısız bir görsel nesne üretir.
    static GameObject Gorsel(string ad, Vector3 konum, Vector2 boyut, Color renk, Sprite sprite, int sira = 0, Transform ebeveyn = null)
    {
        GameObject go = new GameObject(ad);
        if (ebeveyn != null) go.transform.SetParent(ebeveyn, false);
        go.transform.localPosition = konum;
        go.transform.localScale = new Vector3(boyut.x, boyut.y, 1f);
        Gorsellestir(go, sprite, renk, sira);
        return go;
    }

    // Kutu çarpıştırıcılı, etiketli ve katmanlı bir görsel üretir (zemin, duvar, raket).
    static GameObject Kutu(string ad, Vector3 konum, Vector2 boyut, Color renk, string etiket, int katman)
    {
        GameObject go = Gorsel(ad, konum, boyut, renk, kare);
        go.tag = etiket;
        go.layer = katman;
        go.AddComponent<BoxCollider2D>().size = Vector2.one;
        return go;
    }

    // Topun kayıpsız sekmesi için seken malzemeli duvar üretir.
    static void SekenDuvar(string ad, Vector3 konum, Vector2 boyut, Color renk)
    {
        Kutu(ad, konum, boyut, renk, "Untagged", 0).GetComponent<BoxCollider2D>().sharedMaterial = seken;
    }

    // Kök ölçeksizdir; böylece collider ve ayak noktası dünya birimiyle okunur.
    static GameObject Karakter(string ad, Vector3 konum, Vector2 boyut, out SpriteRenderer sr, out Transform ayak)
    {
        GameObject kok = new GameObject(ad);
        kok.tag = "Player";
        kok.transform.position = konum;
        Rigidbody2D rb = kok.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.sharedMaterial = kaygan;
        BoxCollider2D kutu = kok.AddComponent<BoxCollider2D>();
        kutu.size = boyut;
        kutu.sharedMaterial = kaygan;
        GameObject g = Gorsel("Gorsel", Vector3.zero, boyut, Color.white, kare, 5, kok.transform);
        sr = g.GetComponent<SpriteRenderer>();
        GameObject a = new GameObject("Ayak");
        a.transform.SetParent(kok.transform, false);
        a.transform.localPosition = new Vector3(0f, -boyut.y * 0.5f);
        ayak = a.transform;
        return kok;
    }

    // Geçici nesneyi prefab olarak kaydeder ve sahnedeki kopyasını siler.
    static GameObject PrefabYap(GameObject temp, string ad)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, PrefabKlasoru + "/" + ad + ".prefab");
        Object.DestroyImmediate(temp);
        return prefab;
    }

    // Bir [SerializeField] alanına nesne bağlar (Inspector'da sürükleyip bırakmanın kodla yapılışı).
    static void Ata(Object hedef, string alan, Object deger)
    {
        SerializedObject so = new SerializedObject(hedef);
        so.FindProperty(alan).objectReferenceValue = deger;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // Bir LayerMask alanına katman maskesi yazar.
    static void Maske(Object hedef, string alan, int maske)
    {
        SerializedObject so = new SerializedObject(hedef);
        so.FindProperty(alan).intValue = maske;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // Klasör yoksa oluşturur.
    static void KlasorYap(string yol)
    {
        if (AssetDatabase.IsValidFolder(yol)) return;
        AssetDatabase.CreateFolder(Path.GetDirectoryName(yol).Replace('\\', '/'), Path.GetFileName(yol));
    }

    // Klasörleri, iki sprite'ı, sprite malzemesini ve üç fizik malzemesini hazırlar.
    static void VarliklariHazirla()
    {
        KlasorYap(SpriteKlasoru);
        KlasorYap(FizikKlasoru);
        KlasorYap(PrefabKlasoru);
        KlasorYap(SahneKlasoru);

        kare = SpriteYap("Kare", 32, false);
        daire = SpriteYap("Daire", 128, true);
        malzeme = AssetDatabase.LoadAssetAtPath<Material>(UnlitMalzeme);
        kaygan = FizikMalzemesi("Kaygan", 0f, 0f);
        seken = FizikMalzemesi("Seken", 0f, 1f);
        kayaYuzeyi = FizikMalzemesi("KayaYuzeyi", 0f, 0.1f);
    }

    // Beyaz kare ya da daire PNG'si çizer, kaydeder ve sprite olarak içe aktarır.
    static Sprite SpriteYap(string ad, int boyut, bool daireMi)
    {
        string yol = SpriteKlasoru + "/" + ad + ".png";
        // Her pikseli boya; dairede merkezden uzak pikseller saydam olur (kenar yumuşak).
        Texture2D doku = new Texture2D(boyut, boyut, TextureFormat.RGBA32, false);
        float merkez = (boyut - 1) * 0.5f;
        for (int y = 0; y < boyut; y++)
        {
            for (int x = 0; x < boyut; x++)
            {
                float mesafe = new Vector2(x - merkez, y - merkez).magnitude;
                float a = daireMi ? Mathf.Clamp01(boyut * 0.5f - mesafe) : 1f;
                doku.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        File.WriteAllBytes(yol, doku.EncodeToPNG());
        Object.DestroyImmediate(doku);
        AssetDatabase.ImportAsset(yol, ImportAssetOptions.ForceUpdate);

        // Sprite ayarı: 1 sprite = 1 dünya birimi olsun.
        TextureImporter ayar = (TextureImporter)AssetImporter.GetAtPath(yol);
        ayar.textureType = TextureImporterType.Sprite;
        ayar.spriteImportMode = SpriteImportMode.Single;
        ayar.spritePixelsPerUnit = boyut;
        ayar.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(yol);
    }

    // Fizik malzemesini bulur ya da oluşturur, sürtünme ve sekme değerlerini yazar.
    static PhysicsMaterial2D FizikMalzemesi(string ad, float surtunme, float sekme)
    {
        string yol = FizikKlasoru + "/" + ad + ".physicsMaterial2D";
        PhysicsMaterial2D m = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(yol);
        if (m == null)
        {
            m = new PhysicsMaterial2D(ad);
            AssetDatabase.CreateAsset(m, yol);
        }
        m.friction = surtunme;
        m.bounciness = sekme;
        EditorUtility.SetDirty(m);
        return m;
    }

    // Proje ayarlarına eksik etiketleri ve "Zemin" katmanını ekler.
    static void EtiketVeKatmanEkle()
    {
        SerializedObject tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty etiketler = tm.FindProperty("tags");
        foreach (string etiket in new[] { "Zemin", "Engel", "Raket", "Tugla", "Kaya", "Enkaz" })
        {
            bool mevcut = false;
            for (int i = 0; i < etiketler.arraySize; i++)
                if (etiketler.GetArrayElementAtIndex(i).stringValue == etiket) mevcut = true;
            if (mevcut) continue;
            etiketler.InsertArrayElementAtIndex(etiketler.arraySize);
            etiketler.GetArrayElementAtIndex(etiketler.arraySize - 1).stringValue = etiket;
        }

        // "Zemin" katmanı yoksa ilk boş kullanıcı katmanına (6 ve sonrası) yaz.
        SerializedProperty katmanlar = tm.FindProperty("layers");
        bool katmanVar = false;
        for (int i = 6; i < katmanlar.arraySize; i++)
            if (katmanlar.GetArrayElementAtIndex(i).stringValue == "Zemin") katmanVar = true;
        if (!katmanVar)
        {
            for (int i = 6; i < katmanlar.arraySize; i++)
            {
                if (katmanlar.GetArrayElementAtIndex(i).stringValue != "") continue;
                katmanlar.GetArrayElementAtIndex(i).stringValue = "Zemin";
                break;
            }
        }
        tm.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
        zeminKatmani = LayerMask.NameToLayer("Zemin");
    }
}
