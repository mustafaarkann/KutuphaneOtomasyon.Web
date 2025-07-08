using KutuphaneOtomasyon.Web.Models; // DashboardViewModel'inizin bulundu�u namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // DbContext i�in gerekli
using System.Collections.Generic; // List<string> i�in
using System.Diagnostics;
using System.Linq; // OrderByDescending, Take gibi LINQ metotlar� i�in

namespace KutuphaneOtomasyon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Veritaban� ba�lam�n�z� (DbContext) tan�ml�yoruz

        // Constructor: ILogger ve ApplicationDbContext'i ba��ml�l�k enjeksiyonu ile al�yoruz
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Enjekte edilen DbContext'i at�yoruz
        }

        public async Task<IActionResult> Index() // Metodu asenkron (async) yap�yoruz
        {
            var viewModel = new DashboardViewModel
            {
                // Veritaban�ndan toplam say�lar� �ekiyoruz
                ToplamKitapSayisi = await _context.Kitaplar.CountAsync(),
                ToplamUyeSayisi = await _context.Uyeler.CountAsync(),
                // GercekIadeTarihi null olanlar aktif �d�n�lerdir
                AktifOduncSayisi = await _context.OduncAlmalar.CountAsync(o => o.GercekIadeTarihi == null)
            };

            // Son Etkinlikler i�in verileri �ekme
            var activities = new List<string>();

            // �d�n� Alma ve �ade Etkinlikleri (OduncAlmaID'ye g�re s�rala)
            var recentLoans = await _context.OduncAlmalar
                .Include(o => o.Kitap) // Kitap bilgilerini de y�kle
                .Include(o => o.Uye)   // �ye bilgilerini de y�kle
                .OrderByDescending(o => o.OduncAlmaID) // ID'ye g�re azalan s�rada s�rala
                .Take(5) // Son 5 kayd� al
                .ToListAsync();

            foreach (var loan in recentLoans)
            {
                if (loan.GercekIadeTarihi.HasValue)
                {
                    activities.Add($"Kitap \"{loan.Kitap?.Baslik}\" (ISBN: {loan.Kitap?.ISBN}) iade edildi. {CalculateTimeAgo(loan.GercekIadeTarihi.Value)} �nce.");
                }
                else
                {
                    activities.Add($"Kitap \"{loan.Kitap?.Baslik}\" (ISBN: {loan.Kitap?.ISBN}) �d�n� verildi. {CalculateTimeAgo(loan.OduncAlmaTarihi)} �nce.");
                }
            }

            // Yeni �ye Etkinlikleri (UyeID'ye g�re s�rala)
            var recentMembers = await _context.Uyeler
                .OrderByDescending(u => u.UyeID) // ID'ye g�re azalan s�rada s�rala
                .Take(5) // Son 5 kayd� al
                .ToListAsync();

            foreach (var member in recentMembers)
            {
                // E�er Uye modelinizde KayitTarihi alan� yoksa, aktivite zaman�n� belirtmek i�in �imdiki zaman� kullan�yoruz.
                activities.Add($"Yeni �ye \"{member.Ad} {member.Soyad}\" kaydedildi. {CalculateTimeAgo(DateTime.Now)} �nce.");
            }

            // Yeni Kitap Ekleme Etkinlikleri (KitapID'ye g�re s�rala)
            var recentBooks = await _context.Kitaplar
                .OrderByDescending(k => k.KitapID) // ID'ye g�re azalan s�rada s�rala
                .Take(5) // Son 5 kayd� al
                .ToListAsync();

            foreach (var book in recentBooks)
            {
                // E�er Kitap modelinizde EklemeTarihi alan� yoksa, aktivite zaman�n� belirtmek i�in �imdiki zaman� kullan�yoruz.
                activities.Add($"Yeni kitap \"{book.Baslik}\" (ISBN: {book.ISBN}) eklendi. {CalculateTimeAgo(DateTime.Now)} �nce.");
            }

            // T�m etkinlikleri ViewModel'e at�yoruz. Bu basit bir birle�tirmedir.
            // Daha geli�mi� bir kronolojik s�ralama i�in her bir etkinli�in zaman damgas�n� tutan bir yap�ya ihtiya� duyulur.
            viewModel.SonEtkinlikler = activities.Take(10).ToList(); // �lk 10 etkinli�i al�yoruz

            return View(viewModel); // ViewModel'i View'a g�nderiyoruz
        }

        // Yard�mc� metot: Zaman fark�n� hesaplar ve "X dakika �nce" gibi bir string d�nd�r�r
        private string CalculateTimeAgo(DateTime dateTime)
        {
            TimeSpan timeAgo = DateTime.Now - dateTime;

            if (timeAgo.TotalMinutes < 1)
                return "�imdi";
            if (timeAgo.TotalMinutes < 60)
                return $"{(int)timeAgo.TotalMinutes} dakika";
            if (timeAgo.TotalHours < 24)
                return $"{(int)timeAgo.TotalHours} saat";
            if (timeAgo.TotalDays < 7)
                return $"{(int)timeAgo.TotalDays} g�n";
            if (timeAgo.TotalDays < 30)
                return $"{(int)(timeAgo.TotalDays / 7)} hafta";
            if (timeAgo.TotalDays < 365)
                return $"{(int)(timeAgo.TotalDays / 30)} ay";
            return $"{(int)(timeAgo.TotalDays / 365)} y�l";
        }


        // Di�er metotlar (Privacy, Error) oldu�u gibi kal�r.
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}