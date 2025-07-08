using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Web.Models;
using System.Collections.Generic;

namespace KutuphaneOtomasyon.Web.Models // Sizin DbContext'inizin namespace'i
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Yazar> Yazarlar { get; set; }
        public DbSet<Yayinevi> Yayinevleri { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Uye> Uyeler { get; set; }
        public DbSet<OduncAlma> OduncAlmalar { get; set; }
        public DbSet<Personel> Personel { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Yazar>().ToTable("yazarlar");
            modelBuilder.Entity<Yayinevi>().ToTable("yayinevleri");
            modelBuilder.Entity<Kategori>().ToTable("kategoriler");
            modelBuilder.Entity<Kitap>().ToTable("kitaplar");
            modelBuilder.Entity<Uye>().ToTable("uyeler");
            modelBuilder.Entity<OduncAlma>().ToTable("odunc_almalar");
            modelBuilder.Entity<Personel>().ToTable("personel");

            // Kitap modeli için Foreign Key ilişkilerini açıkça belirtme
            modelBuilder.Entity<Kitap>()
                .HasOne(k => k.Yazar)
                .WithMany(y => y.Kitaplar) // <-- BURAYI YENİDEN DÜZELTİYORUZ!
                .HasForeignKey(k => k.YazarID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kitap>()
                .HasOne(k => k.Kategori)
                .WithMany(c => c.Kitaplar) // <-- BURAYI DEĞİŞTİRDİK! Kategori modelindeki Kitaplar koleksiyonunu belirttik.
                .HasForeignKey(k => k.KategoriID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kitap>()
                .HasOne(k => k.Yayinevi)
               .WithMany(y => y.Kitaplar) // <-- BURAYI YENİDEN DÜZELTİYORUZ!
                .HasForeignKey(k => k.YayineviID)
                .OnDelete(DeleteBehavior.Restrict);

            // OduncAlma ilişkileri
            modelBuilder.Entity<OduncAlma>()
                .HasOne(o => o.Kitap)
                .WithMany(k => k.OduncAlmalar)
                .HasForeignKey(o => o.KitapID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OduncAlma>()
                .HasOne(o => o.Uye)
                .WithMany(u => u.OduncAlmalar)
                .HasForeignKey(o => o.UyeID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}