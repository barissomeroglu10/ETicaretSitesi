using Microsoft.AspNetCore.Mvc;
using ETicaretSitesi.Models;
using ETicaretSitesi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ETicaretSitesi.Controllers
{
    [Authorize]
    public class SiparisController : Controller
    {
        private readonly ETicaretDbContext _context;

        public SiparisController(ETicaretDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var siparisler = await _context.Siparisler
                .Include(s => s.SiparisUrunleri)
                .ThenInclude(su => su.Urun)
                .Where(s => s.KullaniciId == userId)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var siparis = await _context.Siparisler
                .Include(s => s.SiparisUrunleri)
                .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }

        public async Task<IActionResult> Olustur()
        {
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
                TempData["Hata"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Sepet");
            }

            // Stok kontrolü
            foreach (var item in sepetUrunleri)
            {
                if (item.Urun != null && item.Urun.StokMiktari < item.Adet)
                {
                    TempData["Hata"] = $"{item.Urun.Ad} için yeterli stok yok.";
                    return RedirectToAction("Index", "Sepet");
                }
            }

            // Kullanıcı zaten yukarıda kontrol edildi

            return View(new SiparisOlusturViewModel
            {
                SepetUrunleri = sepetUrunleri,
                ToplamTutar = sepetUrunleri.Where(s => s.Urun != null).Sum(s => s.Urun!.Fiyat * s.Adet)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Olustur(SiparisOlusturViewModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            if (!ModelState.IsValid)
            {
                var sepetUrunleriListesi = await _context.SepetUrunleri
                    .Include(su => su.Urun)
                    .Where(su => su.KullaniciId == userId)
                    .ToListAsync();

                model.SepetUrunleri = sepetUrunleriListesi;
                model.ToplamTutar = sepetUrunleriListesi.Where(s => s.Urun != null).Sum(s => s.Urun!.Fiyat * s.Adet);
                return View(model);
            }

            var sepetUrunleriListesi2 = await _context.SepetUrunleri
                .Include(su => su.Urun)
                .Where(su => su.KullaniciId == userId)
                .ToListAsync();

            if (!sepetUrunleriListesi2.Any())
            {
                TempData["Hata"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Sepet");
            }

            var siparis = new Siparis
            {
                KullaniciId = userId,
                Adres = model.Adres,
                Telefon = model.Telefon,
                ToplamTutar = sepetUrunleriListesi2.Where(s => s.Urun != null).Sum(s => s.Urun!.Fiyat * s.Adet),
                SiparisUrunleri = sepetUrunleriListesi2.Where(s => s.Urun != null).Select(s => new SiparisUrun
                {
                    UrunId = s.UrunId,
                    Adet = s.Adet,
                    BirimFiyat = s.Urun!.Fiyat
                }).ToList()
            };

            _context.Siparisler.Add(siparis);

            // Stok güncelleme
            foreach (var item in sepetUrunleriListesi2)
            {
                if (item.Urun != null)
                {
                    item.Urun.StokMiktari -= item.Adet;
                }
            }

            // Sepeti temizle
            _context.SepetUrunleri.RemoveRange(sepetUrunleriListesi2);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Odeme), new { id = siparis.Id });
        }

        public async Task<IActionResult> Odeme(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (siparis == null)
            {
                return NotFound();
            }

            if (siparis.OdemeDurumu != OdemeDurumu.Beklemede)
            {
                TempData["Hata"] = "Bu sipariş için ödeme yapılamaz.";
                return RedirectToAction(nameof(Index));
            }

            return View(siparis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OdemeOnayla(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (siparis == null)
            {
                return NotFound();
            }

            if (siparis.OdemeDurumu != OdemeDurumu.Beklemede)
            {
                TempData["Hata"] = "Bu sipariş için ödeme yapılamaz.";
                return RedirectToAction(nameof(Index));
            }

            siparis.OdemeDurumu = OdemeDurumu.Odendi;
            siparis.SiparisDurumu = SiparisDurumu.Onaylandi;
            siparis.GuncellemeTarihi = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Ödemeniz başarıyla alındı. Siparişiniz hazırlanmaya başlanacak.";
            return RedirectToAction(nameof(Detay), new { id = siparis.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OdemeIptal(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var siparis = await _context.Siparisler
                .Include(s => s.SiparisUrunleri)
                .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (siparis == null)
            {
                TempData["Hata"] = "Sipariş bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (siparis.OdemeDurumu != OdemeDurumu.Beklemede)
            {
                TempData["Hata"] = "Bu sipariş iptal edilemez.";
                return RedirectToAction(nameof(Index));
            }

            // Stokları geri ekle
            foreach (var siparisUrunu in siparis.SiparisUrunleri)
            {
                var urun = await _context.Urunler.FindAsync(siparisUrunu.UrunId);
                if (urun != null)
                {
                    urun.StokMiktari += siparisUrunu.Adet;
                }
            }

            siparis.OdemeDurumu = OdemeDurumu.IptalEdildi;
            siparis.SiparisDurumu = SiparisDurumu.IptalEdildi;
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Siparişiniz iptal edildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iptal(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }
            int userId = kullanici.Id;

            var siparis = await _context.Siparisler
                .Include(s => s.SiparisUrunleri)
                .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (siparis == null)
            {
                return NotFound();
            }

            if (siparis.SiparisDurumu != SiparisDurumu.Beklemede)
            {
                TempData["Hata"] = "Sadece bekleyen siparişler iptal edilebilir.";
                return RedirectToAction(nameof(Detay), new { id });
            }

            // Stokları geri ekle
            foreach (var siparisUrunu in siparis.SiparisUrunleri)
            {
                if (siparisUrunu.Urun != null)
                {
                    siparisUrunu.Urun.StokMiktari += siparisUrunu.Adet;
                }
            }

            siparis.SiparisDurumu = SiparisDurumu.IptalEdildi;
            siparis.OdemeDurumu = OdemeDurumu.IptalEdildi;
            siparis.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "Sipariş başarıyla iptal edildi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Siparis/HemenAl - Direkt ödeme için hızlı sipariş
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HemenAl(int urunId, int adet = 1)
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
                TempData["Hata"] = "Ürün bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            if (urun.StokMiktari < adet)
            {
                TempData["Hata"] = "Yeterli stok yok.";
                return RedirectToAction("UrunDetay", "Home", new { id = urunId });
            }

            // Mevcut sepeti temizle (hemen al için)
            var mevcutSepetUrunleri = await _context.SepetUrunleri
                .Where(su => su.KullaniciId == userId)
                .ToListAsync();
            
            if (mevcutSepetUrunleri.Any())
            {
                _context.SepetUrunleri.RemoveRange(mevcutSepetUrunleri);
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

            // Ürünü sepete ekle
            var sepetUrunu = new SepetUrun
            {
                SepetId = sepet.Id,
                KullaniciId = userId,
                UrunId = urunId,
                Adet = adet,
                BirimFiyat = urun.Fiyat,
                EklenmeTarihi = DateTime.Now
            };
            _context.SepetUrunleri.Add(sepetUrunu);

            // Sepet güncelleme tarihini güncelle
            sepet.GuncellemeTarihi = DateTime.Now;
            
            await _context.SaveChangesAsync();

            // Direkt ödeme ekranına yönlendir
            return RedirectToAction("SepettenOdeme", "Siparis");
        }

        // GET: Siparis/SepettenOdeme - Sepetteki ürünlerle direkt ödeme
        public async Task<IActionResult> SepettenOdeme()
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
                TempData["Hata"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Home");
            }

            // Stok kontrolü
            foreach (var item in sepetUrunleri)
            {
                if (item.Urun != null && item.Urun.StokMiktari < item.Adet)
                {
                    TempData["Hata"] = $"{item.Urun.Ad} için yeterli stok yok.";
                    return RedirectToAction("Index", "Sepet");
                }
            }

            // Sipariş oluştur
            var siparis = new Siparis
            {
                KullaniciId = userId,
                Adres = kullanici.Adres ?? "Adres belirtilmemiş",
                Telefon = kullanici.Telefon ?? "Telefon belirtilmemiş",
                ToplamTutar = sepetUrunleri.Where(s => s.Urun != null).Sum(s => s.Urun!.Fiyat * s.Adet),
                SiparisUrunleri = sepetUrunleri.Where(s => s.Urun != null).Select(s => new SiparisUrun
                {
                    UrunId = s.UrunId,
                    Adet = s.Adet,
                    BirimFiyat = s.Urun!.Fiyat
                }).ToList()
            };

            _context.Siparisler.Add(siparis);

            // Stok güncelleme
            foreach (var item in sepetUrunleri)
            {
                if (item.Urun != null)
                {
                    item.Urun.StokMiktari -= item.Adet;
                }
            }

            // Sepeti temizle
            _context.SepetUrunleri.RemoveRange(sepetUrunleri);

            await _context.SaveChangesAsync();

            // Ödeme sayfasına yönlendir
            return RedirectToAction(nameof(Odeme), new { id = siparis.Id });
        }
    }
} 