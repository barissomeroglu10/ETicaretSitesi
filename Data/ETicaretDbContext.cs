using Microsoft.EntityFrameworkCore;
using ETicaretSitesi.Models;

namespace ETicaretSitesi.Data
{
    public class ETicaretDbContext : DbContext
    {
        public ETicaretDbContext(DbContextOptions<ETicaretDbContext> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisUrun> SiparisUrunleri { get; set; }
        public DbSet<SepetUrun> SepetUrunleri { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kullanıcı konfigürasyonu
            modelBuilder.Entity<Kullanici>()
                .HasMany(k => k.Siparisler)
                .WithOne(s => s.Kullanici)
                .HasForeignKey(s => s.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Kullanici>()
                .HasMany(k => k.SepetUrunleri)
                .WithOne(su => su.Kullanici)
                .HasForeignKey(su => su.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sipariş konfigürasyonu
            modelBuilder.Entity<Siparis>()
                .HasMany(s => s.SiparisUrunleri)
                .WithOne(su => su.Siparis)
                .HasForeignKey(su => su.SiparisId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ürün konfigürasyonu
            modelBuilder.Entity<Urun>()
                .Property(u => u.Fiyat)
                .HasColumnType("decimal(18,2)");

            // Örnek veriler
            modelBuilder.Entity<Urun>().HasData(
                // Laptoplar
                new Urun
                {
                    Id = 1,
                    Ad = "MacBook Pro M2",
                    Aciklama = "Apple'ın en güçlü M2 çipi ile donatılmış MacBook Pro, 13.3 inç Retina ekranı, 8GB birleşik bellek ve 256GB SSD depolama alanı ile profesyonel kullanım için mükemmel performans sunar. Touch Bar ve Touch ID özelliği ile güvenlik ve kullanım kolaylığı sağlar. 10 saate kadar pil ömrü ile mobil çalışma için ideal.",
                    Fiyat = 42999.99m,
                    StokMiktari = 10,
                    UrunTipi = UrunTipi.Laptop,
                    ResimUrl = "/images/laptop1.jpg"
                },
                new Urun
                {
                    Id = 2,
                    Ad = "Dell XPS 13",
                    Aciklama = "Intel Core i7 işlemci ile güçlendirilmiş Dell XPS 13, 13.4 inç 4K Ultra HD InfinityEdge ekranı ile kristal netliğinde görüntü sunar. 16GB LPDDR4x RAM ve 512GB PCIe SSD ile hızlı performans. Premium alüminyum kasası ve karbon fiber iç yüzeyi ile şık tasarım. Windows 11 Pro ile gelir.",
                    Fiyat = 34999.99m,
                    StokMiktari = 8,
                    UrunTipi = UrunTipi.Laptop,
                    ResimUrl = "/images/laptop2.jpg"
                },
                new Urun
                {
                    Id = 3,
                    Ad = "Lenovo ThinkPad X1",
                    Aciklama = "Intel Core i5, 14 inç FHD ekran, 16GB RAM, 512GB SSD",
                    Fiyat = 27999.99m,
                    StokMiktari = 12,
                    UrunTipi = UrunTipi.Laptop,
                    ResimUrl = "/images/laptop3.jpg"
                },
                new Urun
                {
                    Id = 4,
                    Ad = "HP Spectre x360",
                    Aciklama = "Intel Core i7, 13.5 inç OLED ekran, 16GB RAM, 1TB SSD",
                    Fiyat = 31999.99m,
                    StokMiktari = 6,
                    UrunTipi = UrunTipi.Laptop,
                    ResimUrl = "/images/laptop4.jpg"
                },
                new Urun
                {
                    Id = 5,
                    Ad = "Asus ROG Zephyrus",
                    Aciklama = "AMD Ryzen 9, 15.6 inç 165Hz ekran, 32GB RAM, 1TB SSD",
                    Fiyat = 45999.99m,
                    StokMiktari = 4,
                    UrunTipi = UrunTipi.Laptop,
                    ResimUrl = "/images/laptop5.jpg"
                },
                // Telefonlar
                new Urun
                {
                    Id = 6,
                    Ad = "iPhone 15 Pro",
                    Aciklama = "A17 Pro çip, 6.1 inç Super Retina XDR ekran, 128GB",
                    Fiyat = 49999.99m,
                    StokMiktari = 15,
                    UrunTipi = UrunTipi.Telefon,
                    ResimUrl = "/images/telefon1.jpg"
                },
                new Urun
                {
                    Id = 7,
                    Ad = "Samsung Galaxy S24 Ultra",
                    Aciklama = "Samsung'un en gelişmiş akıllı telefonu Galaxy S24 Ultra, Snapdragon 8 Gen 3 işlemci ile üstün performans sunar. 6.8 inç Dynamic AMOLED 2X ekranı, 120Hz yenileme hızı ve S Pen desteği ile profesyonel kullanım için ideal. 200MP ana kamera, 12GB RAM ve 256GB depolama alanı. 5000mAh pil ile uzun kullanım süresi.",
                    Fiyat = 44999.99m,
                    StokMiktari = 12,
                    UrunTipi = UrunTipi.Telefon,
                    ResimUrl = "/images/telefon2.jpg"
                },
                new Urun
                {
                    Id = 8,
                    Ad = "Google Pixel 8 Pro",
                    Aciklama = "Google Tensor G3, 6.7 inç LTPO OLED ekran, 256GB",
                    Fiyat = 39999.99m,
                    StokMiktari = 10,
                    UrunTipi = UrunTipi.Telefon,
                    ResimUrl = "/images/telefon3.jpg"
                },
                new Urun
                {
                    Id = 9,
                    Ad = "Xiaomi 14 Pro",
                    Aciklama = "Snapdragon 8 Gen 3, 6.73 inç AMOLED ekran, 512GB",
                    Fiyat = 34999.99m,
                    StokMiktari = 8,
                    UrunTipi = UrunTipi.Telefon,
                    ResimUrl = "/images/telefon4.jpg"
                },
                new Urun
                {
                    Id = 10,
                    Ad = "OnePlus 12",
                    Aciklama = "Snapdragon 8 Gen 3, 6.82 inç AMOLED ekran, 256GB",
                    Fiyat = 29999.99m,
                    StokMiktari = 14,
                    UrunTipi = UrunTipi.Telefon,
                    ResimUrl = "/images/telefon5.jpg"
                }
            );

            // Admin kullanıcısı
            modelBuilder.Entity<Kullanici>().HasData(
                new Kullanici
                {
                    Id = 1,
                    Ad = "Admin",
                    Soyad = "User",
                    Email = "admin@example.com",
                    Sifre = "Admin123!",
                    Telefon = "5551234567",
                    Adres = "Admin Adresi",
                    AdminMi = true
                }
            );

            // Sepet konfigürasyonu
            modelBuilder.Entity<Sepet>()
                .HasOne(s => s.Kullanici)
                .WithMany()
                .HasForeignKey(s => s.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sepet>()
                .HasMany(s => s.SepetUrunleri)
                .WithOne(su => su.Sepet)
                .HasForeignKey(su => su.SepetId)
                .OnDelete(DeleteBehavior.NoAction);

            // SepetUrun konfigürasyonu
            modelBuilder.Entity<SepetUrun>()
                .HasOne(su => su.Urun)
                .WithMany(u => u.SepetUrunleri)
                .HasForeignKey(su => su.UrunId)
                .OnDelete(DeleteBehavior.NoAction);

            // SiparisUrun konfigürasyonu
            modelBuilder.Entity<SiparisUrun>()
                .HasOne(su => su.Urun)
                .WithMany(u => u.SiparisUrunleri)
                .HasForeignKey(su => su.UrunId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
} 