using ETicaretSitesi.Models;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesi.Data
{
    public static class SeedData
    {
        public static void Initialize(ETicaretDbContext context)
        {
            if (context.Urunler.Any())
            {
                return;
            }

            var urunler = new Urun[]
            {
                new Urun { Id = 1, Ad = "MacBook Pro M2", Aciklama = "Apple M2 çip, 13 inç Retina ekran, 8GB RAM, 256GB SSD", Fiyat = 42999.99m, StokMiktari = 10, UrunTipi = UrunTipi.Laptop, ResimUrl = "/images/laptop1.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 2, Ad = "Dell XPS 13", Aciklama = "Intel Core i7, 13.4 inç 4K ekran, 16GB RAM, 512GB SSD", Fiyat = 34999.99m, StokMiktari = 8, UrunTipi = UrunTipi.Laptop, ResimUrl = "/images/laptop2.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 3, Ad = "Lenovo ThinkPad X1", Aciklama = "Intel Core i5, 14 inç FHD ekran, 16GB RAM, 512GB SSD", Fiyat = 27999.99m, StokMiktari = 12, UrunTipi = UrunTipi.Laptop, ResimUrl = "/images/laptop3.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 4, Ad = "HP Spectre x360", Aciklama = "Intel Core i7, 13.5 inç OLED ekran, 16GB RAM, 1TB SSD", Fiyat = 39999.99m, StokMiktari = 6, UrunTipi = UrunTipi.Laptop, ResimUrl = "/images/laptop4.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 5, Ad = "Asus ROG Zephyrus", Aciklama = "AMD Ryzen 9, 15.6 inç 165Hz ekran, 32GB RAM, 1TB SSD", Fiyat = 45999.99m, StokMiktari = 4, UrunTipi = UrunTipi.Laptop, ResimUrl = "/images/laptop5.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 6, Ad = "iPhone 15 Pro", Aciklama = "A17 Pro çip, 6.1 inç Super Retina XDR ekran, 256GB", Fiyat = 49999.99m, StokMiktari = 9, UrunTipi = UrunTipi.Telefon, ResimUrl = "/images/telefon1.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 7, Ad = "Samsung Galaxy S24 Ultra", Aciklama = "Snapdragon 8 Gen 3, 6.8 inç Dynamic AMOLED ekran, 256GB", Fiyat = 44999.99m, StokMiktari = 12, UrunTipi = UrunTipi.Telefon, ResimUrl = "/images/telefon2.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 8, Ad = "Google Pixel 8 Pro", Aciklama = "Google Tensor G3, 6.7 inç LTPO OLED ekran, 256GB", Fiyat = 39999.99m, StokMiktari = 10, UrunTipi = UrunTipi.Telefon, ResimUrl = "/images/telefon3.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 9, Ad = "Xiaomi 14 Pro", Aciklama = "Snapdragon 8 Gen 3, 6.73 inç AMOLED ekran, 512GB", Fiyat = 34999.99m, StokMiktari = 8, UrunTipi = UrunTipi.Telefon, ResimUrl = "/images/telefon4.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true },
                new Urun { Id = 10, Ad = "OnePlus 12", Aciklama = "Snapdragon 8 Gen 3, 6.82 inç AMOLED ekran, 256GB", Fiyat = 29999.99m, StokMiktari = 14, UrunTipi = UrunTipi.Telefon, ResimUrl = "/images/telefon5.jpg", OlusturmaTarihi = DateTime.Now, AktifMi = true }
            };

            context.Urunler.AddRange(urunler);

            if (!context.Kullanicilar.Any())
            {
                var admin = new Kullanici
                {
                    Id = 1,
                    Ad = "Admin",
                    Soyad = "User",
                    Email = "admin@example.com",
                    Sifre = "Admin123!",
                    Telefon = "5551234567",
                    Adres = "Admin Adresi",
                    AdminMi = true
                };

                context.Kullanicilar.Add(admin);
            }

            context.SaveChanges();
        }
    }
} 