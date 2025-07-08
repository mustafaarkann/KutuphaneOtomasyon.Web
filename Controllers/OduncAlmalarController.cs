using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Web.Models; // Namespace for your models
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics; // For Debug.WriteLine

namespace KutuphaneOtomasyon.Web.Controllers
{
    public class OduncAlmalarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OduncAlmalarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OduncAlmalar
        public async Task<IActionResult> Index()
        {
            var oduncAlmalar = _context.OduncAlmalar
                                       .Include(o => o.Kitap)
                                       .Include(o => o.Uye);
            return View(await oduncAlmalar.ToListAsync());
        }

        // GET: OduncAlmalar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var oduncAlma = await _context.OduncAlmalar.Include(o => o.Kitap).Include(o => o.Uye).FirstOrDefaultAsync(m => m.OduncAlmaID == id);
            if (oduncAlma == null) return NotFound();
            return View(oduncAlma);
        }

        // GET: OduncAlmalar/Create
        public async Task<IActionResult> Create()
        {
            ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik");
            ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd");
            return View(new OduncAlma());
        }

        // POST: OduncAlmalar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OduncAlmaID,KitapID,UyeID,OduncAlmaTarihi,BeklenenIadeTarihi,GercekIadeTarihi")] OduncAlma oduncAlma)
        {
            // YENİ EKLENECEK KISIM: ModelState.IsValid kontrolü
            if (!ModelState.IsValid)
            {
                // ModelState geçerli değilse, hatalar zaten View'a geri gönderilir.
                // Dropdown'ları tekrar doldurmak gerekecek.
                ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik", oduncAlma.KitapID);
                ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd", oduncAlma.UyeID);
                TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doğru şekilde doldurunuz."; // Genel bir bilgi mesajı ekleyebilirsiniz
                return View(oduncAlma);
            }

            // Eğer ModelState.IsValid ise, varsayılan değer atamaları buraya taşınabilir
            // veya OduncAlma modelinin yapıcı metodunda (constructor) yapılabilir.
            // Mevcut haliyle de çalışır ancak validasyondan sonra olması daha mantıklı.
            if (string.IsNullOrEmpty(oduncAlma.Durum))
            {
                oduncAlma.Durum = "Ödünç Verildi";
            }
            if (oduncAlma.OduncAlmaTarihi == DateTime.MinValue)
            {
                oduncAlma.OduncAlmaTarihi = DateTime.Today;
            }

            // Tüm işlemleri tek bir try-catch bloğuna alıyoruz,
            // çünkü validasyon hataları artık burada yakalanacak.
            try
            {
                // Kitabın mevcut adedini kontrol et
                var kitap = await _context.Kitaplar.FindAsync(oduncAlma.KitapID);
                if (kitap == null || kitap.MevcutAdet <= 0)
                {
                    // Kitap stoğu yetersizse burada hata mesajı gösterip geri dönebiliriz.
                    // Bu, iş mantığı validasyonudur, ModelState.IsValid ile ilgili değildir.
                    TempData["ErrorMessage"] = "Seçilen kitap şu anda mevcut değil veya stoğu tükenmiştir.";
                    ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik", oduncAlma.KitapID);
                    ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd", oduncAlma.UyeID);
                    return View(oduncAlma);
                }
                else
                {
                    kitap.MevcutAdet--; // Kitap stoğunu azalt
                    _context.Update(kitap);

                    _context.Add(oduncAlma);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ödünç alma işlemi başarıyla kaydedildi.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                // Veritabanı hatalarını (örn. NOT NULL kısıtlaması) burada yakalıyoruz.
                // Modeldeki [Required] attribute'ları hala çalışır, ancak bunlar sadece
                // modelin doğru bağlandığını gösterir, veritabanı düzeyindeki hataları engellemez.
                var postgresEx = ex.InnerException as Npgsql.PostgresException;
                if (postgresEx != null)
                {
                    // Hatanın detayını Debug Output'a yazdır
                    Debug.WriteLine($"DbUpdateException (POST Create): {postgresEx.MessageText}");
                    // Kullanıcıya daha genel bir hata mesajı göster
                    TempData["ErrorMessage"] = $"Veritabanı Hatası: {postgresEx.MessageText}";
                }
                else
                {
                    Debug.WriteLine($"DbUpdateException (POST Create): {ex.Message}");
                    TempData["ErrorMessage"] = "Kayıt sırasında bir veritabanı hatası oluştu. Lütfen tüm alanların doğru doldurulduğundan emin olun.";
                }

                // Hata durumunda dropdown'ları tekrar doldur ve View'ı geri döndür.
                ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik", oduncAlma.KitapID);
                ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd", oduncAlma.UyeID);
                return View(oduncAlma);
            }
            catch (Exception ex)
            {
                // Diğer genel hataları yakala
                Debug.WriteLine($"Genel Hata (POST Create): {ex.Message}");
                TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.";
                ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik", oduncAlma.KitapID);
                ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd", oduncAlma.UyeID);
                return View(oduncAlma);
            }
        }

        // GET: OduncAlmalar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var oduncAlma = await _context.OduncAlmalar.FindAsync(id);
            if (oduncAlma == null) return NotFound();

            ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik", oduncAlma.KitapID);
            ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd", oduncAlma.UyeID);
            return View(oduncAlma);
        }

        // POST: OduncAlmalar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OduncAlmaID,KitapID,UyeID,OduncAlmaTarihi,BeklenenIadeTarihi,GercekIadeTarihi,Durum")] OduncAlma oduncAlma)
        {
            if (id != oduncAlma.OduncAlmaID) return NotFound();

            var originalOduncAlma = await _context.OduncAlmalar.AsNoTracking().FirstOrDefaultAsync(o => o.OduncAlmaID == id);
            if (originalOduncAlma == null) return NotFound();

            try
            {
                // Stok güncelleme mantığı
                if (originalOduncAlma.GercekIadeTarihi == null && oduncAlma.GercekIadeTarihi.HasValue && oduncAlma.Durum == "İade Edildi")
                {
                    var kitap = await _context.Kitaplar.FindAsync(oduncAlma.KitapID);
                    if (kitap != null)
                    {
                        kitap.MevcutAdet++;
                        _context.Update(kitap);
                    }
                }

                _context.Update(oduncAlma);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ödünç alma kaydı başarıyla güncellendi.";
                return RedirectToAction(nameof(Index)); // Başarılıysa Index'e yönlendir
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OduncAlmaExists(oduncAlma.OduncAlmaID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                var postgresEx = ex.InnerException as Npgsql.PostgresException;
                if (postgresEx != null)
                {
                    Debug.WriteLine($"DbUpdateException (POST Edit): {postgresEx.MessageText}");
                    TempData["ErrorMessage"] = $"Veritabanı Hatası: {postgresEx.MessageText}";
                }
                else
                {
                    Debug.WriteLine($"DbUpdateException (POST Edit): {ex.Message}");
                    TempData["ErrorMessage"] = "Güncelleme sırasında bir veritabanı hatası oluştu. Lütfen tüm alanların doğru doldurulduğundan emin olun.";
                }
                ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik", oduncAlma.KitapID);
                ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd", oduncAlma.UyeID);
                return View(oduncAlma); // Hata durumunda View'ı geri döndür
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Genel Hata (POST Edit): {ex.Message}");
                TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.";
                ViewData["KitapID"] = new SelectList(await _context.Kitaplar.ToListAsync(), "KitapID", "Baslik", oduncAlma.KitapID);
                ViewData["UyeID"] = new SelectList(await _context.Uyeler.ToListAsync(), "UyeID", "TamAd", oduncAlma.UyeID);
                return View(oduncAlma);
            }
        }

        // GET: OduncAlmalar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var oduncAlma = await _context.OduncAlmalar.Include(o => o.Kitap).Include(o => o.Uye).FirstOrDefaultAsync(m => m.OduncAlmaID == id);
            if (oduncAlma == null) return NotFound();
            return View(oduncAlma);
        }

        // POST: OduncAlmalar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var oduncAlma = await _context.OduncAlmalar.FindAsync(id);
                if (oduncAlma != null)
                {
                    if (oduncAlma.Durum == "Ödünç Verildi")
                    {
                        var kitap = await _context.Kitaplar.FindAsync(oduncAlma.KitapID);
                        if (kitap != null)
                        {
                            kitap.MevcutAdet++;
                            _context.Update(kitap);
                        }
                    }
                    _context.OduncAlmalar.Remove(oduncAlma);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ödünç alma kaydı başarıyla silindi.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var postgresEx = ex.InnerException as Npgsql.PostgresException;
                if (postgresEx != null)
                {
                    Debug.WriteLine($"DbUpdateException (POST Delete): {postgresEx.MessageText}");
                    TempData["ErrorMessage"] = $"Silme sırasında veritabanı hatası: {postgresEx.MessageText}";
                }
                else
                {
                    Debug.WriteLine($"DbUpdateException (POST Delete): {ex.Message}");
                    TempData["ErrorMessage"] = "Silme sırasında bir veritabanı hatası oluştu.";
                }
                return RedirectToAction(nameof(Index)); // Hata olsa bile index'e yönlendirip mesaj göster
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Genel Hata (POST Delete): {ex.Message}");
                TempData["ErrorMessage"] = "Silme sırasında beklenmeyen bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool OduncAlmaExists(int id)
        {
            return _context.OduncAlmalar.Any(e => e.OduncAlmaID == id);
        }
    }
}