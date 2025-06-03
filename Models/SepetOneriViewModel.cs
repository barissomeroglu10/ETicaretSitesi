using System.ComponentModel.DataAnnotations;

namespace ETicaretSitesi.Models
{
    public class SepetOneriViewModel
    {
        public Urun EklenenUrun { get; set; } = new Urun();
        public List<Urun> OnerilenUrunler { get; set; } = new List<Urun>();
        public int SepetUrunSayisi { get; set; }
    }
} 