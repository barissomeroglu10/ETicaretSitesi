using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretSitesi.Models
{
    public class Sepet
    {
        public int Id { get; set; }

        [Required]
        public int KullaniciId { get; set; }

        [Required]
        [Display(Name = "Oluşturma Tarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellemeTarihi { get; set; }

        // Navigation Properties
        [ForeignKey("KullaniciId")]
        public virtual Kullanici Kullanici { get; set; } = null!;
        public virtual ICollection<SepetUrun> SepetUrunleri { get; set; } = new List<SepetUrun>();
    }
} 