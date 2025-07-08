using Microsoft.AspNetCore.Mvc;
using KutuphaneOtomasyon.Web.Models;
using Microsoft.EntityFrameworkCore; // ToListAsync() için bu using'i ekliyoruz
namespace KutuphaneOtomasyon.Web.Controllers
{
    public class YazarlarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public YazarlarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Yazarlar (Yazar Listesini Göster)
        public async Task<IActionResult> Index() // async ve Task<IActionResult> olarak değiştiriyoruz
        {
            // Veritabanından tüm yazarları al ve görünüme gönder
            return View(await _context.Yazarlar.ToListAsync());
        }




       

        // GET: Yazarlar/Create (Yeni yazar ekleme formunu göster)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Yazarlar/Create (Yeni yazar verisini kaydet)
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı koruma sağlar
        public async Task<IActionResult> Create([Bind("YazarID,Ad,Soyad")] Yazar yazar)
        {
            // Model doğrulamasını kontrol et (örneğin boş alanlar)
            if (ModelState.IsValid)
            {
                _context.Add(yazar); // Yazarı belleğe ekle
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
                return RedirectToAction(nameof(Index)); // Kaydedince Index sayfasına yönlendir
            }
           return View(yazar); // Doğrulama hatası varsa aynı formu tekrar göster
        }







        // GET: Yazarlar/Details/5 (Belirli bir yazarın detaylarını göster)

        public async Task<IActionResult> Details(int? id) // id parametresi nullable olabilir
        {
            if (id == null)
            {
                return NotFound(); // ID yoksa 404 Not Found döndür
            }

            // Veritabanından belirtilen ID'ye sahip yazarı bul
            var yazar = await _context.Yazarlar
                .FirstOrDefaultAsync(m => m.YazarID == id); // FirstOrDefaultAsync ile asenkron sorgulama

            if (yazar == null)
            {
                return NotFound(); // Yazar bulunamazsa 404 Not Found döndür
            }

            return View(yazar); // Yazarı görünüme gönder
        }




        // GET: Yazarlar/Edit/5 (Mevcut yazar düzenleme formunu göster)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Veritabanından düzenlenecek yazarı bul
            var yazar = await _context.Yazarlar.FindAsync(id); // FindAsync doğrudan ID ile arar

            if (yazar == null)
            {
                return NotFound();
            }
            return View(yazar); // Yazarı görünüme gönder
        }

        // POST: Yazarlar/Edit/5 (Yazar verisini güncelle)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("YazarID,Ad,Soyad")] Yazar yazar)
        {
            if (id != yazar.YazarID) // URL'deki ID ile formdaki ID eşleşiyor mu?
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yazar); // Yazarı güncelleme için belleğe işaretle
                    await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Yazarın varlığını kontrol et, yoksa hata döndür
                    if (!YazarExists(yazar.YazarID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Başka bir hata varsa fırlat
                    }
                }
                return RedirectToAction(nameof(Index)); // Başarılıysa Index sayfasına yönlendir
            }
            return View(yazar); // Doğrulama hatası varsa formu tekrar göster
        }

        // Helper metot: Yazarın var olup olmadığını kontrol eder
        private bool YazarExists(int id)
        {
            return _context.Yazarlar.Any(e => e.YazarID == id);
        }



        // GET: Yazarlar/Delete/5 (Yazar silme onay sayfasını göster)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Veritabanından silinecek yazarı bul
            var yazar = await _context.Yazarlar
                .FirstOrDefaultAsync(m => m.YazarID == id);

            if (yazar == null)
            {
                return NotFound();
            }

            return View(yazar); // Yazarı görünüme gönder
        }

        // POST: Yazarlar/Delete/5 (Yazar silme işlemini gerçekleştir)
        [HttpPost, ActionName("Delete")] // URL'de Delete, ama metod adı DeleteConfirmed olabilir
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // Metot adı DeleteConfirmed
        {
            var yazar = await _context.Yazarlar.FindAsync(id); // Silinecek yazarı ID'ye göre bul

            if (yazar != null)
            {
                _context.Yazarlar.Remove(yazar); // Yazarı silinmek üzere işaretle
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
            }

            return RedirectToAction(nameof(Index)); // İşlem başarılıysa Index sayfasına yönlendir
        }
    }
}