using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneOtomasyon.Web.Models
{
    public class Uye
    {
        [Column("uyeid")]
        public int UyeID { get; set; }

        [Required(ErrorMessage = "Üye Adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Üye Adı en fazla 100 karakter olabilir.")]
        [Column("ad")]
        public string Ad { get; set; } = string.Empty; // <-- Başlattık

        [Required(ErrorMessage = "Üye Soyadı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Üye Soyadı en fazla 100 karakter olabilir.")]
        [Column("soyad")]
        public string Soyad { get; set; } = string.Empty; // <-- Başlattık

        [Required(ErrorMessage = "E-posta adresi boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(200, ErrorMessage = "E-posta adresi en fazla 200 karakter olabilir.")]
        [Column("eposta")]
        public string Eposta { get; set; } = string.Empty; // <-- Başlattık

        [Required(ErrorMessage = "Telefon numarası boş bırakılamaz.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir.")]
        [Column("telefon")]
        public string Telefon { get; set; } = string.Empty; // <-- Başlattık

        // Koleksiyonu constructor'da başlattığımız için '?' işaretine gerek yok,
        // çünkü her zaman boş bir koleksiyonla da olsa başlatılacak.
        public ICollection<OduncAlma> OduncAlmalar { get; set; }

        [NotMapped]
        public string TamAd => $"{Ad} {Soyad}";

        public Uye()
        {
            // Koleksiyonu boş bir HashSet ile başlatıyoruz
            OduncAlmalar = new HashSet<OduncAlma>();
        }
    }
}