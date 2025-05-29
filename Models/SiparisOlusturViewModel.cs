using System.ComponentModel.DataAnnotations;

namespace ETicaretSitesi.Models
{
    public class SiparisOlusturViewModel
    {
        [Required(ErrorMessage = "Adres alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        [Display(Name = "Teslimat Adresi")]
        public string Adres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; } = string.Empty;

        public ICollection<SepetUrun> SepetUrunleri { get; set; } = new List<SepetUrun>();
        public decimal ToplamTutar { get; set; }
    }
} 