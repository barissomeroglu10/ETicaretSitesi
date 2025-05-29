using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETicaretSitesi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResimUrlFromGorselUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mevcut ürünlerin ResimUrl alanlarını GorselUrl'den kopyala
            migrationBuilder.Sql("UPDATE Urunler SET ResimUrl = GorselUrl WHERE ResimUrl = '' OR ResimUrl IS NULL");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "KayitTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(3135));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2821));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2854));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 3,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2858));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 4,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2859));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 5,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2861));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 6,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2863));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 7,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2864));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 8,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2866));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 9,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2867));

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "Id",
                keyValue: 10,
                column: "OlusturmaTarihi",
                value: new DateTime(2025, 5, 27, 14, 12, 56, 529, DateTimeKind.Local).AddTicks(2873));
        }
    }
}
