using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ETicaretSitesi.Models;
using ETicaretSitesi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ETicaretSitesi.Controllers
{
    [Authorize]
    public class ProfilController : Controller
    {
        private readonly ETicaretDbContext _context;

        public ProfilController(ETicaretDbContext context)
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

            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> Guncelle(Kullanici model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
            
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Kullanici");
            }

            kullanici.Ad = model.Ad;
            kullanici.Soyad = model.Soyad;
            kullanici.Telefon = model.Telefon;
            kullanici.Adres = model.Adres;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Profil bilgileriniz güncellendi.";

            return RedirectToAction("Index");
        }
    }
} 