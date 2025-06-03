using System.ComponentModel.DataAnnotations;

namespace ETicaretSitesi.Models
{
    public class SepetIndexViewModel
    {
        public List<SepetUrun> SepetUrunleri { get; set; } = new List<SepetUrun>();
        public List<Urun> OnerilenUrunler { get; set; } = new List<Urun>();
        public decimal ToplamTutar { get; set; }
        public int ToplamUrunSayisi { get; set; }
    }
} 