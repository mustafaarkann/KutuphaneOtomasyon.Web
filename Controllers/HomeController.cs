using KutuphaneOtomasyon.Web.Models; // DashboardViewModel'inizin bulunduðu namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // DbContext için gerekli
using System.Collections.Generic; // List<string> için
using System.Diagnostics;
using System.Linq; // OrderByDescending, Take gibi LINQ metotlarý için

namespace KutuphaneOtomasyon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Veritabaný baðlamýnýzý (DbContext) tanýmlýyoruz

        // Constructor: ILogger ve ApplicationDbContext'i baðýmlýlýk enjeksiyonu ile alýyoruz
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Enjekte edilen DbContext'i atýyoruz
        }

        public async Task<IActionResult> Index() // Metodu asenkron (async) yapýyoruz
        {
            var viewModel = new DashboardViewModel
            {
                // Veritabanýndan toplam sayýlarý çekiyoruz
                ToplamKitapSayisi = await _context.Kitaplar.CountAsync(),
                ToplamUyeSayisi = await _context.Uyeler.CountAsync(),
                // GercekIadeTarihi null olanlar aktif ödünçlerdir
                AktifOduncSayisi = await _context.OduncAlmalar.CountAsync(o => o.GercekIadeTarihi == null)
            };

            // Son Etkinlikler için verileri çekme
            var activities = new List<string>();

            // Ödünç Alma ve Ýade Etkinlikleri (OduncAlmaID'ye göre sýrala)
            var recentLoans = await _context.OduncAlmalar
                .Include(o => o.Kitap) // Kitap bilgilerini de yükle
                .Include(o => o.Uye)   // Üye bilgilerini de yükle
                .OrderByDescending(o => o.OduncAlmaID) // ID'ye göre azalan sýrada sýrala
                .Take(5) // Son 5 kaydý al
                .ToListAsync();

            foreach (var loan in recentLoans)
            {
                if (loan.GercekIadeTarihi.HasValue)
                {
                    activities.Add($"Kitap \"{loan.Kitap?.Baslik}\" (ISBN: {loan.Kitap?.ISBN}) iade edildi. {CalculateTimeAgo(loan.GercekIadeTarihi.Value)} önce.");
                }
                else
                {
                    activities.Add($"Kitap \"{loan.Kitap?.Baslik}\" (ISBN: {loan.Kitap?.ISBN}) ödünç verildi. {CalculateTimeAgo(loan.OduncAlmaTarihi)} önce.");
                }
            }

            // Yeni Üye Etkinlikleri (UyeID'ye göre sýrala)
            var recentMembers = await _context.Uyeler
                .OrderByDescending(u => u.UyeID) // ID'ye göre azalan sýrada sýrala
                .Take(5) // Son 5 kaydý al
                .ToListAsync();

            foreach (var member in recentMembers)
            {
                // Eðer Uye modelinizde KayitTarihi alaný yoksa, aktivite zamanýný belirtmek için þimdiki zamaný kullanýyoruz.
                activities.Add($"Yeni üye \"{member.Ad} {member.Soyad}\" kaydedildi. {CalculateTimeAgo(DateTime.Now)} önce.");
            }

            // Yeni Kitap Ekleme Etkinlikleri (KitapID'ye göre sýrala)
            var recentBooks = await _context.Kitaplar
                .OrderByDescending(k => k.KitapID) // ID'ye göre azalan sýrada sýrala
                .Take(5) // Son 5 kaydý al
                .ToListAsync();

            foreach (var book in recentBooks)
            {
                // Eðer Kitap modelinizde EklemeTarihi alaný yoksa, aktivite zamanýný belirtmek için þimdiki zamaný kullanýyoruz.
                activities.Add($"Yeni kitap \"{book.Baslik}\" (ISBN: {book.ISBN}) eklendi. {CalculateTimeAgo(DateTime.Now)} önce.");
            }

            // Tüm etkinlikleri ViewModel'e atýyoruz. Bu basit bir birleþtirmedir.
            // Daha geliþmiþ bir kronolojik sýralama için her bir etkinliðin zaman damgasýný tutan bir yapýya ihtiyaç duyulur.
            viewModel.SonEtkinlikler = activities.Take(10).ToList(); // Ýlk 10 etkinliði alýyoruz

            return View(viewModel); // ViewModel'i View'a gönderiyoruz
        }

        // Yardýmcý metot: Zaman farkýný hesaplar ve "X dakika önce" gibi bir string döndürür
        private string CalculateTimeAgo(DateTime dateTime)
        {
            TimeSpan timeAgo = DateTime.Now - dateTime;

            if (timeAgo.TotalMinutes < 1)
                return "þimdi";
            if (timeAgo.TotalMinutes < 60)
                return $"{(int)timeAgo.TotalMinutes} dakika";
            if (timeAgo.TotalHours < 24)
                return $"{(int)timeAgo.TotalHours} saat";
            if (timeAgo.TotalDays < 7)
                return $"{(int)timeAgo.TotalDays} gün";
            if (timeAgo.TotalDays < 30)
                return $"{(int)(timeAgo.TotalDays / 7)} hafta";
            if (timeAgo.TotalDays < 365)
                return $"{(int)(timeAgo.TotalDays / 30)} ay";
            return $"{(int)(timeAgo.TotalDays / 365)} yýl";
        }


        // Diðer metotlar (Privacy, Error) olduðu gibi kalýr.
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