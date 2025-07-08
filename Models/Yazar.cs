using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Eğer kullanmıyorsanız kaldırılabilir

namespace KutuphaneOtomasyon.Web.Models
{
    public class Yazar
    {
        [Key]
        [Column("yazarid")]
        public int YazarID { get; set; }

        [Required(ErrorMessage = "Yazar adı boş bırakılamaz.")]
        [StringLength(100)]
        [Column("ad")]
        public string Ad { get; set; } = string.Empty; // <-- Değişiklik burada

        [Required(ErrorMessage = "Yazar soyadı boş bırakılamaz.")]
        [StringLength(100)]
        [Column("soyad")]
        public string Soyad { get; set; } = string.Empty; // <-- Değişiklik burada

        [NotMapped]
        public string TamAd => $"{Ad} {Soyad}";

        public ICollection<Kitap> Kitaplar { get; set; } = new List<Kitap>();
    }
}