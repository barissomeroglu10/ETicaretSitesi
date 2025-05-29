using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretSitesi.Models
{
    public class Siparis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int KullaniciId { get; set; }

        [Required]
        [Display(Name = "Sipariş Numarası")]
        public string SiparisNumarasi { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Sipariş Tarihi")]
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Toplam Tutar")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamTutar { get; set; }

        [Required]
        [Display(Name = "Adres")]
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        public string Adres { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Telefon")]
        [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
        public string Telefon { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ödeme Durumu")]
        public OdemeDurumu OdemeDurumu { get; set; } = OdemeDurumu.Beklemede;

        [Required]
        [Display(Name = "Sipariş Durumu")]
        public SiparisDurumu SiparisDurumu { get; set; } = SiparisDurumu.Beklemede;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellemeTarihi { get; set; }

        // Navigation Properties
        [ForeignKey("KullaniciId")]
        public virtual Kullanici? Kullanici { get; set; }

        public ICollection<SiparisUrun> SiparisUrunleri { get; set; } = new List<SiparisUrun>();
    }

    public class SiparisDetay
    {
        [Key]
        public int Id { get; set; }

        public int SiparisId { get; set; }
        public Siparis? Siparis { get; set; }

        public int UrunId { get; set; }
        public required Urun Urun { get; set; }

        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
    }
} 