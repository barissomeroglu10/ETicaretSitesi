using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ETicaretSitesi.Models;
using ETicaretSitesi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;

namespace ETicaretSitesi.Controllers
{
    public class SepetController : Controller
    {
        private readonly ETicaretDbContext _context;

        public SepetController(ETicaretDbContext context)
        {
            _context = context;
        }

        // GET: Sepet
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var sepetUrunleri = await _context.SepetUrunleri
                .Include(su => su.Urun)
                .Where(su => su.KullaniciId == userId)
                .ToListAsync();

            // Sepet boşsa normal view döndür
            if (!sepetUrunleri.Any())
            {
                var bosSepetModel = new SepetIndexViewModel
                {
                    SepetUrunleri = sepetUrunleri,
                    OnerilenUrunler = new List<Urun>(),
                    ToplamTutar = 0,
                    ToplamUrunSayisi = 0
                };
                return View(bosSepetModel);
            }

            // Sepetteki ürünlere göre yapay zeka önerisi al
            var oneriler = await GetSepetYapayZekaOnerileri(sepetUrunleri, userId);

            var model = new SepetIndexViewModel
            {
                SepetUrunleri = sepetUrunleri,
                OnerilenUrunler = oneriler,
                ToplamTutar = sepetUrunleri.Sum(x => (x.Urun?.Fiyat ?? 0) * x.Adet),
                ToplamUrunSayisi = sepetUrunleri.Sum(x => x.Adet)
            };

            return View(model);
        }

        // Sepet için yapay zeka önerisi mantığı
        private async Task<List<Urun>> GetSepetYapayZekaOnerileri(List<SepetUrun> sepetUrunleri, int kullaniciId)
        {
            var oneriler = new List<Urun>();
            var sepettekiUrunIds = sepetUrunleri.Select(su => su.UrunId).ToList();

            // Sepetteki kategorileri analiz et
            var sepettekiKategoriler = sepetUrunleri
                .Where(su => su.Urun != null)
                .Select(su => su.Urun!.UrunTipi)
                .Distinct()
                .ToList();

            // Ortalama fiyat hesapla
            var ortalamafiyat = sepetUrunleri
                .Where(su => su.Urun != null)
                .Average(su => su.Urun!.Fiyat);

            // 1. Aynı kategorilerden benzer fiyatlı ürünler
            foreach (var kategori in sepettekiKategoriler)
            {
                var kategoriOnerileri = await _context.Urunler
                    .Where(u => u.UrunTipi == kategori && u.AktifMi)
                    .Where(u => !sepettekiUrunIds.Contains(u.Id))
                    .Where(u => Math.Abs(u.Fiyat - (decimal)ortalamafiyat) <= (decimal)ortalamafiyat * 0.5m) // %50 fiyat toleransı
                    .OrderBy(u => Math.Abs(u.Fiyat - (decimal)ortalamafiyat))
                    .Take(1)
                    .ToListAsync();

                oneriler.AddRange(kategoriOnerileri);
            }

            // 2. Tamamlayıcı kategoriler
            var tumKategoriler = Enum.GetValues<UrunTipi>().ToList();
            var eksikKategoriler = tumKategoriler.Except(sepettekiKategoriler).ToList();

            foreach (var kategori in eksikKategoriler)
            {
                var tamamlayiciOneriler = await _context.Urunler
                    .Where(u => u.UrunTipi == kategori && u.AktifMi)
                    .Where(u => !sepettekiUrunIds.Contains(u.Id))
                    .Where(u => !oneriler.Select(o => o.Id).Contains(u.Id))
                    .OrderByDescending(u => u.OlusturmaTarihi) // Yeni ürünleri öncelikle
                    .Take(1)
                    .ToListAsync();

                oneriler.AddRange(tamamlayiciOneriler);
            }

            // 3. Popüler ürünler (en çok satılan)
            if (oneriler.Count < 4)
            {
                var eksikSayi = 4 - oneriler.Count;
                var populerUrunler = await _context.Urunler
                    .Where(u => u.AktifMi)
                    .Where(u => !sepettekiUrunIds.Contains(u.Id))
                    .Where(u => !oneriler.Select(o => o.Id).Contains(u.Id))
                    .OrderBy(u => Guid.NewGuid()) // Rastgele seçim
                    .Take(eksikSayi)
                    .ToListAsync();

                oneriler.AddRange(populerUrunler);
            }

            return oneriler.Take(4).ToList();
        }

        // POST: Sepet/Ekle (AJAX için)
        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] SepetEkleModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Giriş yapmanız gerekiyor." });
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });
            }
            int userId = kullanici.Id;

            var urun = await _context.Urunler.FindAsync(model.UrunId);
            if (urun == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            // Kullanıcının mevcut sepetini bul veya oluştur
            var sepet = await _context.Sepetler.FirstOrDefaultAsync(s => s.KullaniciId == userId);
            if (sepet == null)
            {
                sepet = new Sepet
                {
                    KullaniciId = userId,
                    OlusturmaTarihi = DateTime.Now
                };
                _context.Sepetler.Add(sepet);
                await _context.SaveChangesAsync();
            }

            var sepetUrunu = await _context.SepetUrunleri
                .FirstOrDefaultAsync(su => su.KullaniciId == userId && su.UrunId == model.UrunId && su.SepetId == sepet.Id);

            if (sepetUrunu != null)
            {
                sepetUrunu.Adet += model.Adet;
                sepetUrunu.BirimFiyat = urun.Fiyat;
                sepetUrunu.EklenmeTarihi = DateTime.Now;
            }
            else
            {
                sepetUrunu = new SepetUrun
                {
                    SepetId = sepet.Id,
                    KullaniciId = userId,
                    UrunId = model.UrunId,
                    Adet = model.Adet,
                    BirimFiyat = urun.Fiyat,
                    EklenmeTarihi = DateTime.Now
                };
                _context.SepetUrunleri.Add(sepetUrunu);
            }

            // Sepet güncelleme tarihini güncelle
            sepet.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Ürün sepete eklendi." });
        }

        // POST: Sepet/UrunEkle
        [HttpPost]
        public async Task<IActionResult> UrunEkle(int urunId, int adet = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var urun = await _context.Urunler.FindAsync(urunId);
            if (urun == null)
            {
                return NotFound();
            }

            // Kullanıcının mevcut sepetini bul veya oluştur
            var sepet = await _context.Sepetler.FirstOrDefaultAsync(s => s.KullaniciId == userId);
            if (sepet == null)
            {
                sepet = new Sepet
                {
                    KullaniciId = userId,
                    OlusturmaTarihi = DateTime.Now
                };
                _context.Sepetler.Add(sepet);
                await _context.SaveChangesAsync();
            }

            var sepetUrunu = await _context.SepetUrunleri
                .FirstOrDefaultAsync(su => su.KullaniciId == userId && su.UrunId == urunId && su.SepetId == sepet.Id);

            if (sepetUrunu != null)
            {
                sepetUrunu.Adet += adet;
                sepetUrunu.BirimFiyat = urun.Fiyat;
                sepetUrunu.EklenmeTarihi = DateTime.Now;
            }
            else
            {
                sepetUrunu = new SepetUrun
                {
                    SepetId = sepet.Id,
                    KullaniciId = userId,
                    UrunId = urunId,
                    Adet = adet,
                    BirimFiyat = urun.Fiyat,
                    EklenmeTarihi = DateTime.Now
                };
                _context.SepetUrunleri.Add(sepetUrunu);
            }

            // Sepet güncelleme tarihini güncelle
            sepet.GuncellemeTarihi = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            // Yapay zeka önerisi sayfasına yönlendir
            return RedirectToAction("Oneriler", new { urunId = urunId });
        }

        // GET: Sepet/Oneriler
        public async Task<IActionResult> Oneriler(int urunId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            // Sepete eklenen ürünü al
            var eklenenUrun = await _context.Urunler.FindAsync(urunId);
            if (eklenenUrun == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Yapay zeka önerisi mantığı
            var oneriler = await GetYapayZekaOnerileri(eklenenUrun, kullanici.Id);

            var model = new SepetOneriViewModel
            {
                EklenenUrun = eklenenUrun,
                OnerilenUrunler = oneriler,
                SepetUrunSayisi = await _context.SepetUrunleri.Where(su => su.KullaniciId == kullanici.Id).SumAsync(su => su.Adet)
            };

            return View(model);
        }

        // Yapay zeka önerisi mantığı
        private async Task<List<Urun>> GetYapayZekaOnerileri(Urun eklenenUrun, int kullaniciId)
        {
            var oneriler = new List<Urun>();

            // 1. Aynı kategorideki diğer ürünler
            var ayniKategoriUrunler = await _context.Urunler
                .Where(u => u.UrunTipi == eklenenUrun.UrunTipi && u.Id != eklenenUrun.Id && u.AktifMi)
                .OrderBy(u => Math.Abs(u.Fiyat - eklenenUrun.Fiyat)) // Fiyat benzerliğine göre sırala
                .Take(2)
                .ToListAsync();
            
            oneriler.AddRange(ayniKategoriUrunler);

            // 2. Farklı kategoriden popüler ürünler
            var farklıKategoriUrunler = await _context.Urunler
                .Where(u => u.UrunTipi != eklenenUrun.UrunTipi && u.AktifMi)
                .Where(u => !oneriler.Select(o => o.Id).Contains(u.Id))
                .OrderByDescending(u => u.OlusturmaTarihi) // Yeni ürünleri öncelikle
                .Take(2)
                .ToListAsync();
            
            oneriler.AddRange(farklıKategoriUrunler);

            // 3. Kullanıcının sepetinde olmayan ürünler
            var sepettekiUrunIds = await _context.SepetUrunleri
                .Where(su => su.KullaniciId == kullaniciId)
                .Select(su => su.UrunId)
                .ToListAsync();

            oneriler = oneriler.Where(u => !sepettekiUrunIds.Contains(u.Id)).ToList();

            // 4. Eğer yeterli öneri yoksa, rastgele aktif ürünler ekle
            if (oneriler.Count < 4)
            {
                var eksikSayi = 4 - oneriler.Count;
                var rastgeleUrunler = await _context.Urunler
                    .Where(u => u.AktifMi && u.Id != eklenenUrun.Id)
                    .Where(u => !oneriler.Select(o => o.Id).Contains(u.Id))
                    .Where(u => !sepettekiUrunIds.Contains(u.Id))
                    .OrderBy(u => Guid.NewGuid()) // Rastgele sıralama
                    .Take(eksikSayi)
                    .ToListAsync();
                
                oneriler.AddRange(rastgeleUrunler);
            }

            return oneriler.Take(4).ToList(); // En fazla 4 öneri
        }

        // POST: Sepet/UrunGuncelle
        [HttpPost]
        public async Task<IActionResult> UrunGuncelle(int id, int adet)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var sepetUrunu = await _context.SepetUrunleri
                .FirstOrDefaultAsync(su => su.Id == id && su.KullaniciId == userId);

            if (sepetUrunu == null)
            {
                return NotFound();
            }

            if (adet <= 0)
            {
                _context.SepetUrunleri.Remove(sepetUrunu);
            }
            else
            {
                sepetUrunu.Adet = adet;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Sepet/UrunSil
        [HttpPost]
        public async Task<IActionResult> UrunSil(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var sepetUrunu = await _context.SepetUrunleri
                .FirstOrDefaultAsync(su => su.Id == id && su.KullaniciId == userId);

            if (sepetUrunu == null)
            {
                return NotFound();
            }

            _context.SepetUrunleri.Remove(sepetUrunu);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Sepet/SiparisOnayla
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SiparisOnayla()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var sepetUrunleri = await _context.SepetUrunleri
                .Include(su => su.Urun)
                .Where(su => su.KullaniciId == userId)
                .ToListAsync();

            if (!sepetUrunleri.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            var siparis = new Siparis
            {
                KullaniciId = userId,
                SiparisTarihi = DateTime.Now,
                SiparisDurumu = SiparisDurumu.Beklemede,
                OdemeDurumu = OdemeDurumu.Beklemede,
                ToplamTutar = sepetUrunleri.Sum(su => su.BirimFiyat * su.Adet)
            };

            _context.Siparisler.Add(siparis);
            await _context.SaveChangesAsync();

            foreach (var sepetUrunu in sepetUrunleri)
            {
                var siparisUrunu = new SiparisUrun
                {
                    SiparisId = siparis.Id,
                    UrunId = sepetUrunu.UrunId,
                    Adet = sepetUrunu.Adet,
                    BirimFiyat = sepetUrunu.BirimFiyat
                };
                _context.SiparisUrunleri.Add(siparisUrunu);

                // Stok güncelleme
                var urun = sepetUrunu.Urun;
                if (urun != null)
                {
                    urun.StokMiktari -= sepetUrunu.Adet;
                }
            }

            // Sepeti temizle
            _context.SepetUrunleri.RemoveRange(sepetUrunleri);
            await _context.SaveChangesAsync();

            return RedirectToAction("Odeme", "Siparis", new { id = siparis.Id });
        }
    }
} 