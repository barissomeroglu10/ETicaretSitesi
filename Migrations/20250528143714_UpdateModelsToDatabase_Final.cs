using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETicaretSitesi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsToDatabase_Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SiparisUrunleri_Urunler_UrunId",
                table: "SiparisUrunleri");

            migrationBuilder.AddColumn<int>(
                name: "SepetId",
                table: "SepetUrunleri",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sepetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sepetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sepetler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "KayitTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(813));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(589));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(644));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 3,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(647));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 4,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(648));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 5,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(650));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 6,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(652));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 7,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(654));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 8,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(656));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 9,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(660));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 10,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 17, 37, 13, 621, DateTimeKind.Local).AddTicks(661));

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_SepetId",
                table: "SepetUrunleri",
                column: "SepetId");

            migrationBuilder.CreateIndex(
                name: "IX_Sepetler_KullaniciId",
                table: "Sepetler",
                column: "KullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_SepetUrunleri_Sepetler_SepetId",
                table: "SepetUrunleri",
                column: "SepetId",
                principalTable: "Sepetler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SiparisUrunleri_Urunler_UrunId",
                table: "SiparisUrunleri",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SepetUrunleri_Sepetler_SepetId",
                table: "SepetUrunleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SiparisUrunleri_Urunler_UrunId",
                table: "SiparisUrunleri");

            migrationBuilder.DropTable(
                name: "Sepetler");

            migrationBuilder.DropIndex(
                name: "IX_SepetUrunleri_SepetId",
                table: "SepetUrunleri");

            migrationBuilder.DropColumn(
                name: "SepetId",
                table: "SepetUrunleri");

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "KayitTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5746));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5363));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5396));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 3,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5399));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 4,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5450));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 5,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5451));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 6,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5453));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 7,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5454));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 8,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5457));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 9,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5460));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 10,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 28, 16, 26, 27, 633, DateTimeKind.Local).AddTicks(5462));

            migrationBuilder.AddForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiparisUrunleri_Urunler_UrunId",
                table: "SiparisUrunleri",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
