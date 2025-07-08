using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Eğer [ValidateNever] kullanmayacaksanız kaldırılabilir

namespace KutuphaneOtomasyon.Web.Models;

public class Kategori
{
    [Key]
    [Column("kategoriid")]
    public int KategoriID { get; set; }

    [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
    [StringLength(100, ErrorMessage = "Kategori adı 100 karakterden uzun olamaz.")]
    [Column("ad")]
    public string Ad { get; set; } = string.Empty; // <-- Ad property'si başlatıldı

    // [ValidateNever] // Eğer başka özel bir ihtiyacınız yoksa kaldırılabilir
    public ICollection<Kitap> Kitaplar { get; set; } // <-- Koleksiyon başlatılacak

    public Kategori() // <-- Constructor 
    {
        Kitaplar = new HashSet<Kitap>(); // Koleksiyon başlatıldı
    }
}