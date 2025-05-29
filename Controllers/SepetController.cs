using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ETicaretSitesi.Models;
using ETicaretSitesi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            return View(sepetUrunleri);
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
            return RedirectToAction(nameof(Index));
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