using Microsoft.AspNetCore.Mvc;
using ETicaretSitesi.Models;
using ETicaretSitesi.Data;
using ETicaretSitesi.Models.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ETicaretSitesi.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly ETicaretDbContext _context;

        public KullaniciController(ETicaretDbContext context)
        {
            _context = context;
        }

        public IActionResult Giris()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Giris(string email, string sifre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
            {
                ModelState.AddModelError("", "E-posta ve şifre gereklidir.");
                return View();
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Email == email);

            if (kullanici != null && !SifreHelper.SifreDogrula(sifre, kullanici.Sifre))
            {
                kullanici = null;
            }

            if (kullanici != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kullanici.Ad),
                    new Claim(ClaimTypes.Email, kullanici.Email),
                    new Claim(ClaimTypes.Role, kullanici.Rol),
                    new Claim("UserId", kullanici.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Geçersiz email veya şifre");
            return View();
        }

        public IActionResult Kayit()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kayit(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                var emailKontrol = await _context.Kullanicilar
                    .AnyAsync(k => k.Email == kullanici.Email);

                if (emailKontrol)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                    return View(kullanici);
                }

                kullanici.Rol = "Kullanici"; // Varsayılan rol
                kullanici.AktifMi = true;
                kullanici.KayitTarihi = DateTime.Now;
                kullanici.Sifre = SifreHelper.HashSifre(kullanici.Sifre);

                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();

                return RedirectToAction("Giris");
            }

            return View(kullanici);
        }

        public IActionResult AdminGiris()
        {
            // Admin scheme ile giriş yapmış mı kontrol et
            if (HttpContext.User.Identity?.IsAuthenticated == true && 
                HttpContext.User.Identity.AuthenticationType == "AdminScheme")
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminGiris(string email, string sifre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
            {
                ModelState.AddModelError("", "E-posta ve şifre gereklidir.");
                return View();
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Email == email);

            if (kullanici != null && !SifreHelper.SifreDogrula(sifre, kullanici.Sifre))
            {
                kullanici = null;
            }

            if (kullanici != null && kullanici.Rol == "Admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kullanici.Ad),
                    new Claim(ClaimTypes.Email, kullanici.Email),
                    new Claim(ClaimTypes.Role, kullanici.Rol),
                    new Claim("UserId", kullanici.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AdminScheme");
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(
                    "AdminScheme",
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError("", "Geçersiz admin bilgileri");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCikis()
        {
            await HttpContext.SignOutAsync("AdminScheme");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
} 