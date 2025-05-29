using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretSitesi.Models
{
    public class SiparisUrun
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SiparisId { get; set; }

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

        // Navigation Properties
        [ForeignKey("SiparisId")]
        public virtual Siparis? Siparis { get; set; }

        [ForeignKey("UrunId")]
        public virtual Urun? Urun { get; set; }
    }
} 