using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; // <-- Burayı ekledik!

namespace KutuphaneOtomasyon.Web.Models
{
    public class Personel
    {
        [Key] // Genellikle ID'ler Key olarak işaretlenir. Eğer primary key ise ekleyin.
        [Column("personelid")]
        public int PersonelID { get; set; }

        [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")] // <-- Ekledik
        [Column("ad")]
        public string Ad { get; set; } = string.Empty; // <-- Başlattık

        [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")] // <-- Ekledik
        [Column("soyad")]
        public string Soyad { get; set; } = string.Empty; // <-- Başlattık

        [Required(ErrorMessage = "Kullanıcı Adı alanı boş bırakılamaz.")] // <-- Ekledik
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.")] // Örnek uzunluk kısıtlaması
        [Column("kullaniciadi")]
        public string KullaniciAdi { get; set; } = string.Empty; // <-- Başlattık

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")] // <-- Ekledik
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")] // Minimum uzunluk eklenebilir, hash olduğu için uzun olabilir
        [Column("sifrehash")]
        public string SifreHash { get; set; } = string.Empty; // <-- Başlattık

        [Required(ErrorMessage = "Rol alanı boş bırakılamaz.")] // <-- Ekledik
        [StringLength(50, ErrorMessage = "Rol en fazla 50 karakter olabilir.")]
        [Column("rol")]
        public string Rol { get; set; } = string.Empty; // <-- Başlattık
    }
}