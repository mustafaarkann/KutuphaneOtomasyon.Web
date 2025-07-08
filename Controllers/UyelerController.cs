using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // DbContext işlemleri için
using KutuphaneOtomasyon.Web.Models; // Uye modelinizin namespace'i
using System.Linq;
using System.Threading.Tasks;

namespace KutuphaneOtomasyon.Web.Controllers
{
    public class UyelerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Dependency Injection ile DbContext'i alıyoruz
        public UyelerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Uyeler (Üye Listeleme Sayfası)
        public async Task<IActionResult> Index()
        {
            // Tüm üyeleri veritabanından al ve View'a gönder
            return View(await _context.Uyeler.ToListAsync());
        }

        // GET: Uyeler/Details/5 (Üye Detay Sayfası)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID yoksa 404 hatası
            }

            // ID'ye göre üyeyi bul
            var uye = await _context.Uyeler.FirstOrDefaultAsync(m => m.UyeID == id);
            if (uye == null)
            {
                return NotFound(); // Üye bulunamazsa 404 hatası
            }

            return View(uye);
        }

        // GET: Uyeler/Create (Yeni Üye Ekleme Formu Sayfası)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Uyeler/Create (Yeni Üye Ekleme Formu Gönderimi)
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı güvenlik
        public async Task<IActionResult> Create([Bind("UyeID,Ad,Soyad,Eposta,Telefon")] Uye uye)
        {
            // Model doğrulama başarılıysa
            if (ModelState.IsValid)
            {
                _context.Add(uye); // Üyeyi DbContext'e ekle
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
                TempData["SuccessMessage"] = "Üye başarıyla eklendi."; // Başarı mesajı
                return RedirectToAction(nameof(Index)); // Üye listesi sayfasına yönlendir
            }
            // Model doğrulama başarısızsa, aynı View'ı hata mesajlarıyla geri gönder
            return View(uye);
        }

        // GET: Uyeler/Edit/5 (Üye Düzenleme Formu Sayfası)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler.FindAsync(id);
            if (uye == null)
            {
                return NotFound();
            }
            return View(uye);
        }

        // POST: Uyeler/Edit/5 (Üye Düzenleme Formu Gönderimi)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UyeID,Ad,Soyad,Eposta,Telefon")] Uye uye)
        {
            // ID eşleşmiyorsa
            if (id != uye.UyeID)
            {
                return NotFound();
            }

            // Model doğrulama başarılıysa
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uye); // Üyeyi güncelle
                    await _context.SaveChangesAsync(); // Değişiklikleri kaydet
                    TempData["SuccessMessage"] = "Üye başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException) // Eşzamanlılık hatası
                {
                    if (!UyeExists(uye.UyeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Doğrulama başarısızsa
            return View(uye);
        }

        // Üye var mı kontrolü (Edit metodu için yardımcı)
        private bool UyeExists(int id)
        {
            return _context.Uyeler.Any(e => e.UyeID == id);
        }

        // GET: Uyeler/Delete/5 (Üye Silme Onay Sayfası)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uye = await _context.Uyeler
                .FirstOrDefaultAsync(m => m.UyeID == id);
            if (uye == null)
            {
                return NotFound();
            }

            return View(uye);
        }

        // POST: Uyeler/Delete/5 (Üye Silme İşlemi)
        [HttpPost, ActionName("Delete")] // POST isteği için Delete aksiyonu
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uye = await _context.Uyeler.FindAsync(id);
            if (uye != null)
            {
                _context.Uyeler.Remove(uye); // Üyeyi kaldır
                await _context.SaveChangesAsync(); // Kaydet
                TempData["SuccessMessage"] = "Üye başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}