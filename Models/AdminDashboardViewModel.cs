using System.Collections.Generic;

namespace ETicaretSitesi.Models
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<Siparis> Siparisler { get; set; } = new List<Siparis>();
        public IEnumerable<Urun> Urunler { get; set; } = new List<Urun>();
        public IEnumerable<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
        public int ToplamUrun { get; set; }
        public int ToplamSiparis { get; set; }
        public int ToplamKullanici { get; set; }
        public decimal ToplamGelir { get; set; }
        public IEnumerable<Siparis> SonSiparisler { get; set; } = new List<Siparis>();
    }
} 