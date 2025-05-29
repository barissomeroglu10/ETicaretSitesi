using Microsoft.AspNetCore.Mvc;
using ETicaretSitesi.Models;
using ETicaretSitesi.Data;
using ETicaretSitesi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Claims;
using ETicaretSitesi.Models.Helpers;

namespace ETicaretSitesi.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class AdminController : Controller
    {
        private readonly ETicaretDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRecommendationService _recommendationService;

        public AdminController(ETicaretDbContext context, IWebHostEnvironment webHostEnvironment, IRecommendationService recommendationService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index()
        {
            // Admin kontrolü
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminGiris", "Kullanici");
            }

            var siparisler = await _context.Siparisler
                .Include(s => s.Kullanici)
                .OrderByDescending(s => s.SiparisTarihi)
                .Take(10)
                .ToListAsync();

            var urunler = await _context.Urunler
                .OrderByDescending(u => u.Id)
                .Take(10)
                .ToListAsync();

            var kullanicilar = await _context.Kullanicilar
                .OrderByDescending(k => k.KayitTarihi)
                .Take(10)
                .ToListAsync();

            var toplamGelir = await _context.Siparisler
                .Where(s => s.OdemeDurumu == OdemeDurumu.Odendi)
                .SumAsync(s => s.ToplamTutar);

            return View(new AdminDashboardViewModel
            {
                Siparisler = siparisler,
                Urunler = urunler,
                Kullanicilar = kullanicilar,
                SonSiparisler = siparisler,
                ToplamUrun = await _context.Urunler.CountAsync(),
                ToplamSiparis = await _context.Siparisler.CountAsync(),
                ToplamKullanici = await _context.Kullanicilar.CountAsync(),
                ToplamGelir = toplamGelir
            });
        }

        public IActionResult Urunler()
        {
            var urunler = _context.Urunler.ToList();
            return View(urunler);
        }

        public IActionResult UrunEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunEkle(Urun urun, IFormFile? gorsel)
        {
            if (ModelState.IsValid)
            {
                // Görsel yükleme işlemi
                if (gorsel != null && gorsel.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(gorsel.FileName);
                    var extension = Path.GetExtension(gorsel.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    var filePath = Path.Combine(uploadsFolder, newFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await gorsel.CopyToAsync(fileStream);
                    }
                    
                    var imageUrl = $"/images/{newFileName}";
                    urun.GorselUrl = imageUrl;
                    urun.ResimUrl = imageUrl; // Ana sayfada kullanılan alan
                }

                urun.OlusturmaTarihi = DateTime.Now;
                urun.AktifMi = true;
                
                _context.Urunler.Add(urun);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Ürün başarıyla eklendi.";
                return RedirectToAction(nameof(Urunler));
            }
            return View(urun);
        }

        public async Task<IActionResult> UrunDuzenle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunDuzenle(int id, Urun urun, IFormFile? gorsel)
        {
            if (id != urun.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut ürünü veritabanından al
                    var mevcutUrun = await _context.Urunler.FindAsync(id);
                    if (mevcutUrun == null)
                    {
                        return NotFound();
                    }

                    // Görsel yükleme işlemi
                    if (gorsel != null && gorsel.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(gorsel.FileName);
                        var extension = Path.GetExtension(gorsel.FileName);
                        var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        
                        var filePath = Path.Combine(uploadsFolder, newFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await gorsel.CopyToAsync(fileStream);
                        }
                        
                        var imageUrl = $"/images/{newFileName}";
                        urun.GorselUrl = imageUrl;
                        urun.ResimUrl = imageUrl; // Ana sayfada kullanılan alan
                    }
                    else
                    {
                        // Yeni görsel yüklenmemişse mevcut görseli koru
                        urun.GorselUrl = mevcutUrun.GorselUrl;
                        urun.ResimUrl = mevcutUrun.ResimUrl;
                    }

                    // Diğer alanları güncelle
                    mevcutUrun.Ad = urun.Ad;
                    mevcutUrun.Aciklama = urun.Aciklama;
                    mevcutUrun.Fiyat = urun.Fiyat;
                    mevcutUrun.StokMiktari = urun.StokMiktari;
                    mevcutUrun.UrunTipi = urun.UrunTipi;
                    mevcutUrun.AktifMi = urun.AktifMi;
                    mevcutUrun.GorselUrl = urun.GorselUrl;
                    mevcutUrun.ResimUrl = urun.ResimUrl; // Ana sayfada kullanılan alan
                    mevcutUrun.GuncellemeTarihi = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["Mesaj"] = "Ürün başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UrunExists(urun.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Urunler));
            }
            return View(urun);
        }

        public async Task<IActionResult> UrunSil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        [HttpPost, ActionName("UrunSil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunSilOnay(int id)
        {
            var urun = await _context.Urunler.FindAsync(id);
            if (urun != null)
            {
                _context.Urunler.Remove(urun);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Ürün başarıyla silindi.";
            }
            return RedirectToAction(nameof(Urunler));
        }

        public async Task<IActionResult> Siparisler()
        {
            var siparisler = await _context.Siparisler
                .Include(s => s.Kullanici)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        public async Task<IActionResult> SiparisDetay(int id)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisUrunleri)
                .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SiparisDurumuGuncelle(int id, SiparisDurumu yeniDurum)
        {
            var siparis = await _context.Siparisler.FindAsync(id);
            if (siparis == null)
            {
                return NotFound();
            }

            siparis.SiparisDurumu = yeniDurum;
            siparis.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SiparisDetay), new { id });
        }

        public async Task<IActionResult> Kullanicilar()
        {
            var kullanicilar = await _context.Kullanicilar
                .OrderByDescending(k => k.KayitTarihi)
                .ToListAsync();

            return View(kullanicilar);
        }

        public async Task<IActionResult> KullaniciDetay(int id)
        {
            var kullanici = await _context.Kullanicilar
                .Include(k => k.Siparisler)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        public async Task<IActionResult> KullaniciDuzenle(int id)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);

            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KullaniciDuzenle(int id, Kullanici kullanici)
        {
            if (id != kullanici.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kullanici);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KullaniciExists(kullanici.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Kullanicilar));
            }
            return View(kullanici);
        }

        public async Task<IActionResult> UrunDetay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrunDurumuGuncelle(int id, bool aktifMi)
        {
            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }

            urun.AktifMi = aktifMi;
            urun.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "Ürün durumu başarıyla güncellendi.";
            return RedirectToAction(nameof(UrunDetay), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KullaniciDurumuGuncelle(int id, bool aktifMi)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                return NotFound();
            }

            kullanici.AktifMi = aktifMi;

            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "Kullanıcı durumu başarıyla güncellendi.";
            return RedirectToAction(nameof(KullaniciDetay), new { id });
        }

        // GET: Admin/KullaniciEkle
        public IActionResult KullaniciEkle()
        {
            return View();
        }

        // POST: Admin/KullaniciEkle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KullaniciEkle(Kullanici kullanici)
        {
            // E-posta benzersizlik kontrolü
            var mevcutKullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Email == kullanici.Email);
            
            if (mevcutKullanici != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                return View(kullanici);
            }

            if (ModelState.IsValid)
            {
                kullanici.KayitTarihi = DateTime.Now;
                kullanici.AktifMi = true;
                kullanici.KullaniciAdi = kullanici.Email; // E-posta adresini kullanıcı adı olarak kullan
                
                // Rol alanını AdminMi durumuna göre set et
                kullanici.Rol = kullanici.AdminMi ? "Admin" : "Kullanici";
                
                // Şifreyi hashle (basit hash - üretimde daha güçlü hash kullanılmalı)
                kullanici.Sifre = SifreHelper.HashSifre(kullanici.Sifre);

                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();
                
                TempData["Mesaj"] = "Kullanıcı başarıyla eklendi.";
                return RedirectToAction(nameof(Kullanicilar));
            }

            return View(kullanici);
        }

        // POST: Admin/KullaniciSil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KullaniciSil(int id)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Kullanicilar));
            }

            // Admin kullanıcıları silinemez
            if (kullanici.AdminMi)
            {
                TempData["Hata"] = "Admin kullanıcıları silinemez.";
                return RedirectToAction(nameof(Kullanicilar));
            }

            // Sadece aktif kullanıcılar için sipariş kontrolü yap
            // Pasif kullanıcılar siparişleri olsa bile silinebilir
            if (kullanici.AktifMi)
            {
                var kullaniciSiparisleri = await _context.Siparisler
                    .Where(s => s.KullaniciId == id)
                    .CountAsync();

                if (kullaniciSiparisleri > 0)
                {
                    TempData["Hata"] = "Bu kullanıcının mevcut siparişleri bulunduğu için silinemez. Önce kullanıcıyı pasif hale getirebilirsiniz.";
                    return RedirectToAction(nameof(Kullanicilar));
                }
            }

            // Kullanıcının sepetini temizle
            var sepetUrunleri = await _context.SepetUrunleri
                .Where(su => su.KullaniciId == id)
                .ToListAsync();
            
            if (sepetUrunleri.Any())
            {
                _context.SepetUrunleri.RemoveRange(sepetUrunleri);
            }

            var sepet = await _context.Sepetler
                .FirstOrDefaultAsync(s => s.KullaniciId == id);
            
            if (sepet != null)
            {
                _context.Sepetler.Remove(sepet);
            }

            // Kullanıcıyı sil
            _context.Kullanicilar.Remove(kullanici);
            await _context.SaveChangesAsync();
            
            TempData["Mesaj"] = "Kullanıcı başarıyla silindi.";
            return RedirectToAction(nameof(Kullanicilar));
        }

        private bool UrunExists(int id)
        {
            return _context.Urunler.Any(e => e.Id == id);
        }

        private bool KullaniciExists(int id)
        {
            return _context.Kullanicilar.Any(e => e.Id == id);
        }

        // AI Model Yönetimi
        public async Task<IActionResult> AIYonetimi()
        {
            var isModelTrained = await _recommendationService.IsModelTrainedAsync();
            ViewBag.IsModelTrained = isModelTrained;
            
            // İstatistikler
            var toplamSiparis = await _context.Siparisler.CountAsync();
            var tamamlananSiparis = await _context.Siparisler.Where(s => s.OdemeDurumu == OdemeDurumu.Odendi).CountAsync();
            var toplamKullanici = await _context.Kullanicilar.CountAsync();
            var toplamUrun = await _context.Urunler.CountAsync();
            
            ViewBag.ToplamSiparis = toplamSiparis;
            ViewBag.TamamlananSiparis = tamamlananSiparis;
            ViewBag.ToplamKullanici = toplamKullanici;
            ViewBag.ToplamUrun = toplamUrun;
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ModelEgit()
        {
            try
            {
                await _recommendationService.TrainModelAsync();
                TempData["Mesaj"] = "AI model başarıyla eğitildi! Artık akıllı ürün önerileri aktif.";
                return Json(new { success = true, message = "Model başarıyla eğitildi" });
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Model eğitimi sırasında hata oluştu: " + ex.Message;
                return Json(new { success = false, message = "Model eğitimi başarısız: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ModelDurumu()
        {
            try
            {
                var isTrained = await _recommendationService.IsModelTrainedAsync();
                return Json(new { success = true, isTrained = isTrained });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestOneriler(int? kullaniciId, int? urunId)
        {
            try
            {
                if (kullaniciId.HasValue)
                {
                    var userRecommendations = await _recommendationService.GetRecommendationsForUserAsync(kullaniciId.Value, 5);
                    return Json(new { success = true, type = "user", recommendations = userRecommendations.Select(u => new { u.Id, u.Ad, u.Fiyat }) });
                }
                else if (urunId.HasValue)
                {
                    var productRecommendations = await _recommendationService.GetRecommendationsForProductAsync(urunId.Value, 5);
                    return Json(new { success = true, type = "product", recommendations = productRecommendations.Select(u => new { u.Id, u.Ad, u.Fiyat }) });
                }
                else
                {
                    return Json(new { success = false, message = "Kullanıcı ID veya Ürün ID gerekli" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
} 