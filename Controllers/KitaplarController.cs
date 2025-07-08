using KutuphaneOtomasyon.Web.Models; // Model sınıflarınızın bulunduğu namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // DbContext işlemleri için
using System.Linq; // OrderByDescending, ToListAsync için
using System.Threading.Tasks; // Async metodlar için
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList sınıfı için gereklidir


namespace KutuphaneOtomasyon.Web.Controllers
{
    public class KitaplarController : Controller
    {
        private readonly ApplicationDbContext _context; // DbContext'i tanımla

        // Constructor ile ApplicationDbContext'i enjekte et
        public KitaplarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kitaplar
        // Tüm kitapları listeleme Action'ı
        public async Task<IActionResult> Index()
        {
            // Kitapları Yazar, Kategori ve Yayınevi bilgileriyle birlikte çekiyoruz
            // Bu, Kitap modelinizdeki navigasyon property'lerini kullanarak ilişkili veriyi yükler.
            var kitaplar = await _context.Kitaplar
                                         .Include(k => k.Yazar)     // Yazar bilgisini dahil et
                                         .Include(k => k.Kategori)  // Kategori bilgisini dahil et
                                         .Include(k => k.Yayinevi)  // Yayınevi bilgisini dahil et
                                         .OrderByDescending(k => k.KitapID) // Kitapları ID'ye göre tersten sırala (veya başka bir kritere göre)
                                         .ToListAsync(); // Tüm kitapları liste olarak çek

            return View(kitaplar); // Çekilen kitap listesini View'a gönder
        }

        // GET: Kitaplar/Create
        // Yeni kitap ekleme formunu göstermek için kullanılır.
        public async Task<IActionResult> Create()
        {




            // Dropdown listeler için gerekli verileri veritabanından alıyoruz
            // SelectList nesneleri, HTML <select> elemanları için veri hazırlar.
            // "YazarID", "TamAd" (YazarID value, TamAd metin olarak görünür)
            ViewData["YazarID"] = new SelectList(await _context.Yazarlar.OrderBy(y => y.Ad).ThenBy(y => y.Soyad).ToListAsync(), "YazarID", "TamAd");
            ViewData["KategoriID"] = new SelectList(await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync(), "KategoriID", "Ad");
            ViewData["YayineviID"] = new SelectList(await _context.Yayinevleri.OrderBy(y => y.Ad).ToListAsync(), "YayineviID", "Ad");

            return View(); // Boş bir form gönderecek
        }

        // POST: Kitaplar/Create
        // Yeni kitap formundan gelen verileri işler ve veritabanına kaydeder.
        [HttpPost] // Bu Action'ın sadece POST isteklerini kabul ettiğini belirtir
        [ValidateAntiForgeryToken] // CSRF (Cross-Site Request Forgery) saldırılarına karşı koruma sağlar
        public async Task<IActionResult> Create([Bind("KitapID,Baslik,YayinYili,ISBN,MevcutAdet,YazarID,YayineviID,KategoriID")] Kitap kitap)
        {

            

            // Model doğrulama (data annotations'lar ile belirlenen kurallar kontrol edilir)
            if (ModelState.IsValid)
            {
                _context.Add(kitap); // Kitabı veritabanı bağlamına ekle (henüz veritabanına gitmez)
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
                TempData["SuccessMessage"] = "Kitap başarıyla eklendi."; // Başarı mesajı ayarla
                return RedirectToAction(nameof(Index)); // İşlem sonrası Index sayfasına yönlendir
            }

            // Model doğrulama başarısız olursa, aynı formu hatalarla birlikte göster.
            // Bu sefer, kullanıcının girdiği değerleri (kitap.YazarID vb.) seçili olarak göstermek için
            // SelectList'e dördüncü parametre olarak mevcut değeri iletiyoruz.
            ViewData["YazarID"] = new SelectList(await _context.Yazarlar.OrderBy(y => y.Ad).ThenBy(y => y.Soyad).ToListAsync(), "YazarID", "TamAd", kitap.YazarID);
            ViewData["KategoriID"] = new SelectList(await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync(), "KategoriID", "Ad", kitap.KategoriID);
            ViewData["YayineviID"] = new SelectList(await _context.Yayinevleri.OrderBy(y => y.Ad).ToListAsync(), "YayineviID", "Ad", kitap.YayineviID);
            return View(kitap); // Hatalı model ile birlikte View'ı geri gönder
        }

        // GET: Kitaplar/Details/5
        // Belirli bir kitabın detaylarını gösterir
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID sağlanmadıysa 404 hatası döndür
            }

            // Kitabı ID'ye göre bul ve ilgili yazar, kategori, yayınevi bilgilerini dahil et
            var kitap = await _context.Kitaplar
                                      .Include(k => k.Yazar)
                                      .Include(k => k.Kategori)
                                      .Include(k => k.Yayinevi)
                                      .FirstOrDefaultAsync(m => m.KitapID == id);

            if (kitap == null)
            {
                return NotFound(); // Kitap bulunamadıysa 404 hatası döndür
            }

            return View(kitap); // Kitap detaylarını View'a gönder
        }

        // GET: Kitaplar/Edit/5
        // Belirli bir kitabı düzenleme formunu gösterir
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID sağlanmadıysa 404 hatası döndür
            }

            // Kitabı ID'ye göre bul
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap == null)
            {
                return NotFound(); // Kitap bulunamadıysa 404 hatası döndür
            }

            // Düzenleme formu için dropdown listeleri hazırla (seçili değeri de belirt)
            ViewData["YazarID"] = new SelectList(await _context.Yazarlar.OrderBy(y => y.Ad).ThenBy(y => y.Soyad).ToListAsync(), "YazarID", "TamAd", kitap.YazarID);
            ViewData["KategoriID"] = new SelectList(await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync(), "KategoriID", "Ad", kitap.KategoriID);
            ViewData["YayineviID"] = new SelectList(await _context.Yayinevleri.OrderBy(y => y.Ad).ToListAsync(), "YayineviID", "Ad", kitap.YayineviID);

            return View(kitap); // Kitap bilgilerini form için View'a gönder
        }

        // POST: Kitaplar/Edit/5
        // Düzenleme formundan gelen verileri işler ve veritabanına kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KitapID,Baslik,YayinYili,ISBN,MevcutAdet,YazarID,YayineviID,KategoriID")] Kitap kitap)
        {
            // URL'deki ID ile formdan gelen KitapID'nin eşleştiğini kontrol et
            if (id != kitap.KitapID)
            {
                return NotFound();
            }

            // Model doğrulama başarılı ise
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kitap); // Kitabı veritabanında güncelle
                    await _context.SaveChangesAsync(); // Değişiklikleri kaydet
                    TempData["SuccessMessage"] = "Kitap başarıyla güncellendi."; // Başarı mesajı
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Eşzamanlılık hatası kontrolü (başka biri aynı anda değiştirdiyse)
                    if (!_context.Kitaplar.Any(e => e.KitapID == kitap.KitapID))
                    {
                        return NotFound(); // Kitap silinmişse 404
                    }
                    else
                    {
                        throw; // Diğer eşzamanlılık hatalarını fırlat
                    }
                }
                return RedirectToAction(nameof(Index)); // Index sayfasına yönlendir
            }

            // Model doğrulama başarısız olursa, formu hatalarla birlikte tekrar göster
            ViewData["YazarID"] = new SelectList(await _context.Yazarlar.OrderBy(y => y.Ad).ThenBy(y => y.Soyad).ToListAsync(), "YazarID", "TamAd", kitap.YazarID);
            ViewData["KategoriID"] = new SelectList(await _context.Kategoriler.OrderBy(k => k.Ad).ToListAsync(), "KategoriID", "Ad", kitap.KategoriID);
            ViewData["YayineviID"] = new SelectList(await _context.Yayinevleri.OrderBy(y => y.Ad).ToListAsync(), "YayineviID", "Ad", kitap.YayineviID);
            return View(kitap);
        }

        // GET: Kitaplar/Delete/5
        // Belirli bir kitabı silme onay sayfasını gösterir
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID sağlanmadıysa 404 hatası döndür
            }

            // Kitabı ID'ye göre bul ve ilişkili verileri dahil et (onay sayfasında göstermek için)
            var kitap = await _context.Kitaplar
                                      .Include(k => k.Yazar)
                                      .Include(k => k.Kategori)
                                      .Include(k => k.Yayinevi)
                                      .FirstOrDefaultAsync(m => m.KitapID == id);

            if (kitap == null)
            {
                return NotFound(); // Kitap bulunamadıysa 404 hatası döndür
            }

            return View(kitap); // Kitap detaylarını onay View'ına gönder
        }

        // POST: Kitaplar/Delete/5
        // Silme onayından sonra veritabanından kaydı siler
        [HttpPost, ActionName("Delete")] // URL'de Delete olarak görünür, Action ismi DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Kitabı ID'ye göre bul
            var kitap = await _context.Kitaplar.FindAsync(id);

            if (kitap != null)
            {
                _context.Kitaplar.Remove(kitap); // Kitabı veritabanı bağlamından kaldır
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet (silme işlemi gerçekleşir)
                TempData["SuccessMessage"] = "Kitap başarıyla silindi."; // Başarı mesajı
            }
            else
            {
                // Kitap bulunamadıysa uyarı mesajı
                TempData["ErrorMessage"] = "Silinecek kitap bulunamadı.";
            }

            return RedirectToAction(nameof(Index)); // Index sayfasına yönlendir
        }


    }
}