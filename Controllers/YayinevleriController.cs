using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // DbContext ve ToListAsync için gerekli
using KutuphaneOtomasyon.Web.Models; // Yayinevi modeliniz için gerekli
using System.Linq; // Any() ve OrderBy() için gerekli
using System.Threading.Tasks; // Asenkron metodlar için gerekli
using System.ComponentModel.DataAnnotations.Schema; // [NotMapped] için gerekli olabilir, modelde kullanılıyorsa

namespace KutuphaneOtomasyon.Web.Controllers
{
    public class YayinevleriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public YayinevleriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Yayinevleri
        // Tüm yayınevlerini listeleme
        public async Task<IActionResult> Index()
        {
            return View(await _context.Yayinevleri.OrderBy(y => y.Ad).ToListAsync());
        }

        // GET: Yayinevleri/Details/5
        // Belirli bir yayınevinin detaylarını gösterir
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yayinevi = await _context.Yayinevleri
                                         .FirstOrDefaultAsync(m => m.YayineviID == id);
            if (yayinevi == null)
            {
                return NotFound();
            }

            return View(yayinevi);
        }

        // GET: Yayinevleri/Create
        // Yeni yayınevi ekleme formunu gösterir
        public IActionResult Create()
        {
            return View();
        }

        // POST: Yayinevleri/Create
        // Yeni yayınevi verilerini veritabanına kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("YayineviID,Ad")] Yayinevi yayinevi)
        {
            // *** Kategori'de yaşadığımız sorunu burada yaşamamak için: ***
            // Yayinevi.cs modelinizde 'Ad' alanı için [Required(ErrorMessage = "...")] olduğundan emin olun.
            // Eğer hata alırsanız, buradaki ModelState.IsValid kontrolü 'false' dönecektir.
            if (ModelState.IsValid)
            {
                _context.Add(yayinevi);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Yayınevi başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(yayinevi);
        }

        // GET: Yayinevleri/Edit/5
        // Belirli bir yayınevini düzenleme formunu gösterir
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yayinevi = await _context.Yayinevleri.FindAsync(id);
            if (yayinevi == null)
            {
                return NotFound();
            }
            return View(yayinevi);
        }

        // POST: Yayinevleri/Edit/5
        // Düzenleme formundan gelen verileri işler ve veritabanına kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("YayineviID,Ad")] Yayinevi yayinevi)
        {
            if (id != yayinevi.YayineviID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yayinevi);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Yayınevi başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Yayinevleri.Any(e => e.YayineviID == yayinevi.YayineviID))
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
            return View(yayinevi);
        }

        // GET: Yayinevleri/Delete/5
        // Belirli bir yayınevini silme onay sayfasını gösterir
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yayinevi = await _context.Yayinevleri
                                         .FirstOrDefaultAsync(m => m.YayineviID == id);
            if (yayinevi == null)
            {
                return NotFound();
            }

            return View(yayinevi);
        }

        // POST: Yayinevleri/Delete/5
        // Silme onayından sonra veritabanından kaydı siler
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yayinevi = await _context.Yayinevleri.FindAsync(id);
            if (yayinevi != null)
            {
                _context.Yayinevleri.Remove(yayinevi);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Yayınevi başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Silinecek yayınevi bulunamadı.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}