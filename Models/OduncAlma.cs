using System;
using System.ComponentModel.DataAnnotations; // Doğrulama (Validation) attribute'ları için eklendi
using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneOtomasyon.Web.Models
{
    public class OduncAlma
    {
        [Column("oduncalmaid")]
        public int OduncAlmaID { get; set; }

        [Required(ErrorMessage = "Kitap seçimi zorunludur.")]
        [Column("kitapid")]
        public int KitapID { get; set; }

        [Required(ErrorMessage = "Üye seçimi zorunludur.")]
        [Column("uyeid")]
        public int UyeID { get; set; }

        [Required(ErrorMessage = "Ödünç Alma Tarihi boş bırakılamaz.")]
        [Column("oduncalmatarihi")]
        [DataType(DataType.Date)] // Sadece tarih kısmını almasını sağlar, saat olmadan
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OduncAlmaTarihi { get; set; }

        [Column("bekleneniadetarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        // Custom validation: Beklenen İade Tarihi, Ödünç Alma Tarihinden sonra olmalı
        [BeklenenIadeTarihiGecerli(ErrorMessage = "Beklenen İade Tarihi, Ödünç Alma Tarihinden sonra olmalıdır.")]
        public DateTime? BeklenenIadeTarihi { get; set; }

        [Column("gercekiadetarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        // Custom validation: Gerçek İade Tarihi, Ödünç Alma Tarihinden sonra olmalı (ve Beklenenden sonra olabilir)
        [GercekIadeTarihiGecerli(ErrorMessage = "Gerçek İade Tarihi, Ödünç Alma Tarihinden sonra olmalıdır.")]
        public DateTime? GercekIadeTarihi { get; set; }

        [Required(ErrorMessage = "Durum alanı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Durum en fazla 50 karakter olabilir.")]
        [Column("durum")]
        public string? Durum { get; set; } // Örneğin: "Ödünç Verildi", "İade Edildi", "Gecikti"

        // Navigasyon Property'leri
        public Kitap? Kitap { get; set; }
        public Uye? Uye { get; set; }
    }

    // --- Custom Validation Attribute'ları ---
    // Bu attribute'lar, modelde daha karmaşık doğrulama kuralları tanımlamamızı sağlar.

    public class BeklenenIadeTarihiGecerliAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var oduncAlma = (OduncAlma)validationContext.ObjectInstance;
            var beklenenIadeTarihi = value as DateTime?;

            // !!! BU KONTROLÜ KALDIRIN VEYA YORUM SATIRI YAPIN !!!
            // if (oduncAlma.OduncAlmaTarihi == default(DateTime))
            // {
            //     return new ValidationResult("Ödünç Alma Tarihi geçerli bir değer içermiyor.");
            // }

            // Ödünç Alma Tarihi'nin geçerli olup olmadığını kontrol etmek yerine,
            // sadece null olmadığından emin olun veya doğrudan kullanın.
            // Eğer OduncAlmaTarihi [Required] ve controller'da atanıyorsa, burada null/default olması beklenmez.
            if (!beklenenIadeTarihi.HasValue) // Eğer Beklenen İade Tarihi girilmediyse
            {
                // Eğer bu alanın boş bırakılmasına izin veriliyorsa (ki nullable), burada bir hata dönmeyiz.
                // Eğer zorunluysa, [Required] attribute'ını eklemelisiniz.
                return ValidationResult.Success; // Boş bırakılabiliyorsa geçerli say.
            }

            // Tarih karşılaştırması
            if (beklenenIadeTarihi.Value.Date < oduncAlma.OduncAlmaTarihi.Date)
            {
                return new ValidationResult(ErrorMessage ?? "Beklenen İade Tarihi, Ödünç Alma Tarihinden sonra olmalıdır.");
            }
            return ValidationResult.Success;
        }
    }

    public class GercekIadeTarihiGecerliAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var oduncAlma = (OduncAlma)validationContext.ObjectInstance;
            var gercekIadeTarihi = value as DateTime?;

            // !!! BU KONTROLÜ KALDIRIN VEYA YORUM SATIRI YAPIN !!!
            if (oduncAlma.OduncAlmaTarihi == default(DateTime))
            {
                return new ValidationResult("Ödünç Alma Tarihi geçerli bir değer içermiyor.");
            }

            // Eğer gerçek iade tarihi girilmişse
            if (gercekIadeTarihi.HasValue)
            {
                if (gercekIadeTarihi.Value.Date < oduncAlma.OduncAlmaTarihi.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Gerçek İade Tarihi, Ödünç Alma Tarihinden sonra olmalıdır.");
                }
            }
            return ValidationResult.Success; // Eğer null ise veya geçerliyse başarılı say.
        }
    }
}