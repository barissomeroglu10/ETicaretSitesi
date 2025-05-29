using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ETicaretSitesi.Models;
using ETicaretSitesi.Data;
using ETicaretSitesi.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ETicaretSitesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ETicaretDbContext _context;
        private readonly IRecommendationService _recommendationService;

        public HomeController(ILogger<HomeController> logger, ETicaretDbContext context, IRecommendationService recommendationService)
        {
            _logger = logger;
            _context = context;
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index(string? kategori)
        {
            var urunler = _context.Urunler.Where(u => u.AktifMi);
            
            if (!string.IsNullOrEmpty(kategori))
            {
                if (Enum.TryParse<UrunTipi>(kategori, out var urunTipi))
                {
                    urunler = urunler.Where(u => u.UrunTipi == urunTipi);
                }
            }
            
            var urunListesi = await urunler.ToListAsync();
            ViewBag.SecilenKategori = kategori;
            
            // Kişiselleştirilmiş öneriler ekle (giriş yapmış kullanıcı için)
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    try
                    {
                        var recommendations = await _recommendationService.GetRecommendationsForUserAsync(userId, 6);
                        ViewBag.PersonalizedRecommendations = recommendations;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Kişiselleştirilmiş öneriler alınırken hata oluştu");
                    }
                }
            }
            
            return View(urunListesi);
        }

        public async Task<IActionResult> UrunDetay(int id)
        {
            var urun = await _context.Urunler.FirstOrDefaultAsync(u => u.Id == id && u.AktifMi);
            if (urun == null)
            {
                return NotFound();
            }

            // Bu ürünle ilgili önerileri al
            try
            {
                var relatedProducts = await _recommendationService.GetRecommendationsForProductAsync(id, 4);
                ViewBag.RelatedProducts = relatedProducts.Where(p => p.Id != id).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlgili ürün önerileri alınırken hata oluştu");
                ViewBag.RelatedProducts = new List<Urun>();
            }

            return View(urun);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 