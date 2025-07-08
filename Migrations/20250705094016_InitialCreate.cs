using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KutuphaneOtomasyon.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kategoriler",
                columns: table => new
                {
                    KategoriID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kategoriler", x => x.KategoriID);
                });

            migrationBuilder.CreateTable(
                name: "personel",
                columns: table => new
                {
                    PersonelID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Soyad = table.Column<string>(type: "text", nullable: false),
                    KullaniciAdi = table.Column<string>(type: "text", nullable: false),
                    SifreHash = table.Column<string>(type: "text", nullable: false),
                    Rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personel", x => x.PersonelID);
                });

            migrationBuilder.CreateTable(
                name: "uyeler",
                columns: table => new
                {
                    UyeID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Soyad = table.Column<string>(type: "text", nullable: false),
                    Eposta = table.Column<string>(type: "text", nullable: false),
                    Telefon = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uyeler", x => x.UyeID);
                });

            migrationBuilder.CreateTable(
                name: "yayinevleri",
                columns: table => new
                {
                    YayineviID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yayinevleri", x => x.YayineviID);
                });

            migrationBuilder.CreateTable(
                name: "yazarlar",
                columns: table => new
                {
                    YazarID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Soyad = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yazarlar", x => x.YazarID);
                });

            migrationBuilder.CreateTable(
                name: "kitaplar",
                columns: table => new
                {
                    KitapID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "text", nullable: false),
                    YayinYili = table.Column<int>(type: "integer", nullable: true),
                    ISBN = table.Column<string>(type: "text", nullable: false),
                    MevcutAdet = table.Column<int>(type: "integer", nullable: false),
                    YazarID = table.Column<int>(type: "integer", nullable: false),
                    YayineviID = table.Column<int>(type: "integer", nullable: false),
                    KategoriID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kitaplar", x => x.KitapID);
                    table.ForeignKey(
                        name: "FK_kitaplar_kategoriler_KategoriID",
                        column: x => x.KategoriID,
                        principalTable: "kategoriler",
                        principalColumn: "KategoriID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kitaplar_yayinevleri_YayineviID",
                        column: x => x.YayineviID,
                        principalTable: "yayinevleri",
                        principalColumn: "YayineviID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kitaplar_yazarlar_YazarID",
                        column: x => x.YazarID,
                        principalTable: "yazarlar",
                        principalColumn: "YazarID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "odunc_almalar",
                columns: table => new
                {
                    OduncAlmaID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KitapID = table.Column<int>(type: "integer", nullable: false),
                    UyeID = table.Column<int>(type: "integer", nullable: false),
                    OduncAlmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BeklenenIadeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GercekIadeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Durum = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_odunc_almalar", x => x.OduncAlmaID);
                    table.ForeignKey(
                        name: "FK_odunc_almalar_kitaplar_KitapID",
                        column: x => x.KitapID,
                        principalTable: "kitaplar",
                        principalColumn: "KitapID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_odunc_almalar_uyeler_UyeID",
                        column: x => x.UyeID,
                        principalTable: "uyeler",
                        principalColumn: "UyeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kitaplar_KategoriID",
                table: "kitaplar",
                column: "KategoriID");

            migrationBuilder.CreateIndex(
                name: "IX_kitaplar_YayineviID",
                table: "kitaplar",
                column: "YayineviID");

            migrationBuilder.CreateIndex(
                name: "IX_kitaplar_YazarID",
                table: "kitaplar",
                column: "YazarID");

            migrationBuilder.CreateIndex(
                name: "IX_odunc_almalar_KitapID",
                table: "odunc_almalar",
                column: "KitapID");

            migrationBuilder.CreateIndex(
                name: "IX_odunc_almalar_UyeID",
                table: "odunc_almalar",
                column: "UyeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "odunc_almalar");

            migrationBuilder.DropTable(
                name: "personel");

            migrationBuilder.DropTable(
                name: "kitaplar");

            migrationBuilder.DropTable(
                name: "uyeler");

            migrationBuilder.DropTable(
                name: "kategoriler");

            migrationBuilder.DropTable(
                name: "yayinevleri");

            migrationBuilder.DropTable(
                name: "yazarlar");
        }
    }
}
