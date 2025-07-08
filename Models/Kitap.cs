using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Gerekli

namespace KutuphaneOtomasyon.Web.Models
{
    public class Kitap
    {
        [Key]
        [Column("kitapid")]
        public int KitapID { get; set; }

        [Required(ErrorMessage = "Kitap başlığı boş bırakılamaz.")]
        [StringLength(200, ErrorMessage = "Kitap başlığı 200 karakterden uzun olamaz.")]
        [Column("baslik")]
        public string Baslik { get; set; } = string.Empty; // <-- Değişiklik burada: string? -> string ve başlatma

        [Display(Name = "Yayın Yılı")]
        [Column("yayinyili")]
        [Range(1, 9999, ErrorMessage = "Yayın Yılı 1 ile 9999 arasında bir değer olmalıdır.")]
        public int? YayinYili { get; set; }

        [Required(ErrorMessage = "ISBN alanı boş bırakılamaz.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN 10 veya 13 karakter olmalıdır.")]
        [Column("isbn")]
        public string ISBN { get; set; } = string.Empty; // <-- Değişiklik burada: string? -> string ve başlatma

        [Display(Name = "Mevcut Adet")]
        [Column("mevcutadet")]
        [Range(0, int.MaxValue, ErrorMessage = "Mevcut Adet negatif olamaz.")]
        public int? MevcutAdet { get; set; } // int? olarak kalabilir, sorun yok

        // İlişkiler için Yabancı Anahtar (Foreign Key) özellikleri
        [Required(ErrorMessage = "Yazar seçimi zorunludur.")]
        [Column("yazarid")]
        [ForeignKey("Yazar")]
        public int YazarID { get; set; }

        [Required(ErrorMessage = "Yayınevi seçimi zorunludur.")]
        [Column("yayineviid")]
        [ForeignKey("Yayinevi")]
        public int YayineviID { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Column("kategoriid")]
        [ForeignKey("Kategori")]
        public int KategoriID { get; set; }

        // Navigasyon Özellikleri (Bu kitap hangi yazar/yayınevi/kategoriye ait?)
        public Yazar? Yazar { get; set; }
        public Yayinevi? Yayinevi { get; set; }
        public Kategori? Kategori { get; set; }

        [ValidateNever]
        public ICollection<OduncAlma> OduncAlmalar { get; set; } = new List<OduncAlma>(); // Bu zaten doğru
    }
}