using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretSitesi.Models
{
    public class SepetUrun
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SepetId { get; set; }

        [Required]
        public int KullaniciId { get; set; }

        [Required]
        public int UrunId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Adet 1'den küçük olamaz.")]
        [Display(Name = "Adet")]
        public int Adet { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Birim Fiyat")]
        public decimal BirimFiyat { get; set; }

        [Required]
        [Display(Name = "Eklenme Tarihi")]
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("SepetId")]
        public virtual Sepet? Sepet { get; set; }

        [ForeignKey("KullaniciId")]
        public virtual Kullanici? Kullanici { get; set; }

        [ForeignKey("UrunId")]
        public virtual Urun? Urun { get; set; }
    }
} 