using System;
using System.ComponentModel.DataAnnotations;

namespace ETicaretSitesi.Models
{
    public enum UrunTipi
    {
        Telefon,
        Laptop
    }

    public class Urun
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Ürün Adı")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün açıklaması zorunludur.")]
        [StringLength(1000, ErrorMessage = "Ürün açıklaması en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        [Display(Name = "Fiyat")]
        public decimal Fiyat { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı 0'dan küçük olamaz.")]
        [Display(Name = "Stok Miktarı")]
        public int StokMiktari { get; set; }

        [Required(ErrorMessage = "Ürün tipi zorunludur.")]
        [Display(Name = "Ürün Tipi")]
        public UrunTipi UrunTipi { get; set; }

        [Display(Name = "Görsel URL")]
        public string GorselUrl { get; set; } = string.Empty;

        [Display(Name = "Resim URL")]
        public string ResimUrl { get; set; } = string.Empty;

        [Display(Name = "Oluşturma Tarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Aktif Mi?")]
        public bool AktifMi { get; set; } = true;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellemeTarihi { get; set; }

        // Navigation Properties
        public virtual ICollection<SiparisUrun> SiparisUrunleri { get; set; } = new List<SiparisUrun>();
        public virtual ICollection<SepetUrun> SepetUrunleri { get; set; } = new List<SepetUrun>();
    }
} 