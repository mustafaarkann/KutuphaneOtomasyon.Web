// Dosya yolu: KutuphaneOtomasyonMVC/ViewModels/DashboardViewModel.cs (veya Models klasöründeyse ona göre)
namespace KutuphaneOtomasyon.Web.Models // Projenizin namespace'ine göre düzenleyin
{
    public class DashboardViewModel
    {
        public int ToplamKitapSayisi { get; set; }
        public int ToplamUyeSayisi { get; set; }
        public int AktifOduncSayisi { get; set; }

        // Son etkinlikler için de bir yer ayırıyoruz, bunu sonra dolduracağız.
        public List<string> SonEtkinlikler { get; set; } = new List<string>();
    }
}