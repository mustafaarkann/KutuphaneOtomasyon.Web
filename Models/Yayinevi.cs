using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneOtomasyon.Web.Models
{
    public class Yayinevi
    {
        [Key]
        [Column("yayineviid")]
        public int YayineviID { get; set; }

        [Required(ErrorMessage = "Yayınevi adı alanı zorunludur.")]
        [Column("ad")]
        public string Ad { get; set; } = string.Empty; // <-- Değişiklik burada

        public ICollection<Kitap> Kitaplar { get; set; } = new List<Kitap>();
    }
}