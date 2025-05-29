using ETicaretSitesi.Models;
using ETicaretSitesi.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ETicaretDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Kullanicilar.Any())
            {
                return;
            }

            var admin = new Kullanici
            {
                Ad = "Admin",
                Soyad = "User",
                Email = "admin@example.com",
                Sifre = SifreHelper.HashSifre("Admin123!"),
                Telefon = "5551234567",
                Adres = "Admin Adresi",
                AdminMi = true,
                Rol = "Admin",
                KullaniciAdi = "admin",
                AktifMi = true,
                KayitTarihi = DateTime.Now
            };

            context.Kullanicilar.Add(admin);
            context.SaveChanges();

            // Ürünleri ekle
            if (!context.Urunler.Any())
            {
                var urunler = new List<Urun>
                {
                    new Urun
                    {
                        Ad = "MacBook Pro 14\"",
                        Aciklama = "Apple M2 Pro çip, 16GB RAM, 512GB SSD",
                        Fiyat = 45000,
                        StokMiktari = 10,
                        UrunTipi = UrunTipi.Laptop,
                        ResimUrl = "/images/laptop1.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "Dell XPS 13",
                        Aciklama = "Intel Core i7, 16GB RAM, 1TB SSD",
                        Fiyat = 35000,
                        StokMiktari = 15,
                        UrunTipi = UrunTipi.Laptop,
                        ResimUrl = "/images/laptop2.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "HP Pavilion Gaming",
                        Aciklama = "AMD Ryzen 7, 16GB RAM, RTX 3060",
                        Fiyat = 28000,
                        StokMiktari = 8,
                        UrunTipi = UrunTipi.Laptop,
                        ResimUrl = "/images/laptop3.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "Lenovo ThinkPad X1",
                        Aciklama = "Intel Core i7, 32GB RAM, 1TB SSD",
                        Fiyat = 42000,
                        StokMiktari = 5,
                        UrunTipi = UrunTipi.Laptop,
                        ResimUrl = "/images/laptop4.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "ASUS ROG Strix",
                        Aciklama = "Intel Core i9, 32GB RAM, RTX 4070",
                        Fiyat = 55000,
                        StokMiktari = 3,
                        UrunTipi = UrunTipi.Laptop,
                        ResimUrl = "/images/laptop5.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "iPhone 15 Pro",
                        Aciklama = "A17 Pro çip, 256GB, Titanium",
                        Fiyat = 32000,
                        StokMiktari = 20,
                        UrunTipi = UrunTipi.Telefon,
                        ResimUrl = "/images/telefon1.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "Samsung Galaxy S24 Ultra",
                        Aciklama = "Snapdragon 8 Gen 3, 512GB, S Pen",
                        Fiyat = 28000,
                        StokMiktari = 25,
                        UrunTipi = UrunTipi.Telefon,
                        ResimUrl = "/images/telefon2.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "Google Pixel 8 Pro",
                        Aciklama = "Google Tensor G3, 256GB, AI Camera",
                        Fiyat = 22000,
                        StokMiktari = 18,
                        UrunTipi = UrunTipi.Telefon,
                        ResimUrl = "/images/telefon3.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "OnePlus 12",
                        Aciklama = "Snapdragon 8 Gen 3, 256GB, 100W Charging",
                        Fiyat = 18000,
                        StokMiktari = 12,
                        UrunTipi = UrunTipi.Telefon,
                        ResimUrl = "/images/telefon4.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    },
                    new Urun
                    {
                        Ad = "Xiaomi 14 Ultra",
                        Aciklama = "Snapdragon 8 Gen 3, 512GB, Leica Camera",
                        Fiyat = 25000,
                        StokMiktari = 15,
                        UrunTipi = UrunTipi.Telefon,
                        ResimUrl = "/images/telefon5.jpg",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now
                    }
                };

                context.Urunler.AddRange(urunler);
                context.SaveChanges();
            }
        }
    }
} 