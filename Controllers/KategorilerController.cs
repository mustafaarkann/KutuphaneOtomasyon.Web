using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KutuphaneOtomasyon.Web.Controllers
{
    public class KategorilerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategorilerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kategoriler
        // Tüm kategorileri listeleme
        public async Task<IActionResult> Index()
        {
            return View(await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync());
        }

        // GET: Kategoriler/Details/5
        // Belirli bir kategorinin detaylarını gösterir
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler
                                         .FirstOrDefaultAsync(m => m.KategoriID == id);
            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        // GET: Kategoriler/Create
        // Yeni kategori ekleme formunu gösterir
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kategoriler/Create
        // Yeni kategori verilerini veritabanına kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KategoriID,Ad")] Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kategori);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kategori başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(kategori);
        }

        // GET: Kategoriler/Edit/5
        // Belirli bir kategoriyi düzenleme formunu gösterir
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }
            return View(kategori);
        }

        // POST: Kategoriler/Edit/5
        // Düzenleme formundan gelen verileri işler ve veritabanına kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KategoriID,Ad")] Kategori kategori)
        {
            if (id != kategori.KategoriID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategori);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Kategoriler.Any(e => e.KategoriID == kategori.KategoriID))
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
            return View(kategori);
        }

        // GET: Kategoriler/Delete/5
        // Belirli bir kategoriyi silme onay sayfasını gösterir
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler
                                         .FirstOrDefaultAsync(m => m.KategoriID == id);
            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        // POST: Kategoriler/Delete/5
        // Silme onayından sonra veritabanından kaydı siler
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori != null)
            {
                _context.Kategoriler.Remove(kategori);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Silinecek kategori bulunamadı.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}