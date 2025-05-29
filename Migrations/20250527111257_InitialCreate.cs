using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ETicaretSitesi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdminMi = table.Column<bool>(type: "bit", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Fiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StokMiktari = table.Column<int>(type: "int", nullable: false),
                    UrunTipi = table.Column<int>(type: "int", nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResimUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Siparisler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    SiparisNumarasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiparisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OdemeDurumu = table.Column<int>(type: "int", nullable: false),
                    SiparisDurumu = table.Column<int>(type: "int", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparisler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siparisler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SepetUrunleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Adet = table.Column<int>(type: "int", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SepetUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SepetUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiparisUrunleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiparisId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Adet = table.Column<int>(type: "int", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisUrunleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisUrunleri_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiparisUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Kullanicilar",
                columns: new[] { "Id", "Ad", "AdminMi", "Adres", "AktifMi", "Email", "KayitTarihi", "KullaniciAdi", "Rol", "Sifre", "SonGirisTarihi", "Soyad", "Telefon" },
                values: new object[] { 1, "Admin", true, "Admin Adresi", true, "admin@example.com", new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(3135), "admin", "Admin", "PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=", null, "User", "5551234567" });

            migrationBuilder.InsertData(
                table: "Urunler",
                columns: new[] { "Id", "Aciklama", "Ad", "AktifMi", "Fiyat", "GorselUrl", "GuncellemeTarihi", "OlusturmaTarihi", "ResimUrl", "StokMiktari", "UrunTipi" },
                values: new object[,]
                {
                    { 1, "Apple'ın en güçlü M2 çipi ile donatılmış MacBook Pro, 13.3 inç Retina ekranı, 8GB birleşik bellek ve 256GB SSD depolama alanı ile profesyonel kullanım için mükemmel performans sunar. Touch Bar ve Touch ID özelliği ile güvenlik ve kullanım kolaylığı sağlar. 10 saate kadar pil ömrü ile mobil çalışma için ideal.", "MacBook Pro M2", true, 42999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2821), "/images/laptop1.jpg", 10, 1 },
                    { 2, "Intel Core i7 işlemci ile güçlendirilmiş Dell XPS 13, 13.4 inç 4K Ultra HD InfinityEdge ekranı ile kristal netliğinde görüntü sunar. 16GB LPDDR4x RAM ve 512GB PCIe SSD ile hızlı performans. Premium alüminyum kasası ve karbon fiber iç yüzeyi ile şık tasarım. Windows 11 Pro ile gelir.", "Dell XPS 13", true, 34999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2854), "/images/laptop2.jpg", 8, 1 },
                    { 3, "Intel Core i5, 14 inç FHD ekran, 16GB RAM, 512GB SSD", "Lenovo ThinkPad X1", true, 27999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2858), "/images/laptop3.jpg", 12, 1 },
                    { 4, "Intel Core i7, 13.5 inç OLED ekran, 16GB RAM, 1TB SSD", "HP Spectre x360", true, 31999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2859), "/images/laptop4.jpg", 6, 1 },
                    { 5, "AMD Ryzen 9, 15.6 inç 165Hz ekran, 32GB RAM, 1TB SSD", "Asus ROG Zephyrus", true, 45999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2861), "/images/laptop5.jpg", 4, 1 },
                    { 6, "A17 Pro çip, 6.1 inç Super Retina XDR ekran, 128GB", "iPhone 15 Pro", true, 49999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2863), "/images/telefon1.jpg", 15, 0 },
                    { 7, "Samsung'un en gelişmiş akıllı telefonu Galaxy S24 Ultra, Snapdragon 8 Gen 3 işlemci ile üstün performans sunar. 6.8 inç Dynamic AMOLED 2X ekranı, 120Hz yenileme hızı ve S Pen desteği ile profesyonel kullanım için ideal. 200MP ana kamera, 12GB RAM ve 256GB depolama alanı. 5000mAh pil ile uzun kullanım süresi.", "Samsung Galaxy S24 Ultra", true, 44999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2864), "/images/telefon2.jpg", 12, 0 },
                    { 8, "Google Tensor G3, 6.7 inç LTPO OLED ekran, 256GB", "Google Pixel 8 Pro", true, 39999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2866), "/images/telefon3.jpg", 10, 0 },
                    { 9, "Snapdragon 8 Gen 3, 6.73 inç AMOLED ekran, 512GB", "Xiaomi 14 Pro", true, 34999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2867), "/images/telefon4.jpg", 8, 0 },
                    { 10, "Snapdragon 8 Gen 3, 6.82 inç AMOLED ekran, 256GB", "OnePlus 12", true, 29999.99m, "", null, new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2873), "/images/telefon5.jpg", 14, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_KullaniciId",
                table: "SepetUrunleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_UrunId",
                table: "SepetUrunleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_KullaniciId",
                table: "Siparisler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisUrunleri_SiparisId",
                table: "SiparisUrunleri",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisUrunleri_UrunId",
                table: "SiparisUrunleri",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SepetUrunleri");

            migrationBuilder.DropTable(
                name: "SiparisUrunleri");

            migrationBuilder.DropTable(
                name: "Siparisler");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
