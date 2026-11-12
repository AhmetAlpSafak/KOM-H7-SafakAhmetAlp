# Yapay Zekâ Beyanı (çalışma notu)

Araç: Claude (Anthropic), Claude masaüstü uygulaması ve Claude Code.

| Tarih | Ne yapıldı | Dosyalar |
|---|---|---|
| 2026-11-09 | Dört oyunun betikleri ve sahne kurucu yazıldı; sahneler kuruldu ve otomatik testle denendi | `Assets/Scripts/A, B, C, D, Editor` |
| 2026-11-10 | CLAUDE.md, .gitignore, README hazırlandı | kök klasör |
| 2026-11-10 | Sisifos kolaylaştırıldı: omuz zamanlama penceresi genişletildi (ideal 1, tolerans 2, en az kalite 0.55), taban itiş 11, barikat kırma eşiği 6 | `SisifosHareket.cs`, `YolEngeli.cs`, `SahneKurucu.cs`, `D_Birlesik.unity`, `D_Barikat.prefab` |
| 2026-11-10 | Kaya ezmesi yumuşatıldı: üstünden atlama payı -0.4, ezme hızı 9, alttan vuruş açısı 35° | `Kaya.cs`, `D_Birlesik.unity` |
| 2026-11-10 | Kaya eşiği aşabilsin diye çıkış açıları yükseltildi (düz 25°, dik 55°), kırılmayan engelden sekme 0.25'e indi | `Kaya.cs`, `D_Birlesik.unity` |
| 2026-11-10 | Bütün betiklere açıklama satırları eklendi (kod mantığı değişmedi); yorum kuralı CLAUDE.md'ye yazıldı | `Assets/Scripts/A, B, C, D, Editor`, `CLAUDE.md` |
| 2026-11-10 | Tasarım belgesi taslağı (6 bölüm, ayar ve bağ tabloları, şema) üretildi; eşli test notu boş bırakıldı | `Belge/H7_TasarimBelgesi.pdf`, `Belge/kaynak/belge_uret.py` |
| 2026-11-10 | Git deposu kuruldu, ilk commit atıldı; D_Birlesik için Windows build başlatıldı | `.git`, `Build/D_Birlesik` |
| 2026-11-12 | Oynanış videosu mp4'e çevrildi (yeniden kodlamadan); iki eksik açıklama satırı eklendi | `Belge/oynanis_D.mp4`, `Kaya.cs`, `SisifosOyun.cs` |
