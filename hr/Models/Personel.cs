using System.ComponentModel.DataAnnotations;

namespace HR.Models;

public class Personel
{
    public long PersonelId { get; set; }

    [Required(ErrorMessage = "Departman seçmelisiniz.")]
    [Display(Name = "Departman")]
    public long DepartmanId { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Ad")]
    public string PersonelAd { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Soyad")]
    public string PersonelSoyad { get; set; } = "";

    [Required(ErrorMessage = "TC kimlik numarası zorunludur.")]
    [RegularExpression(@"^[1-9][0-9]{10}$",
        ErrorMessage = "TC kimlik numarası 11 haneli olmalı ve 0 ile başlamamalıdır.")]
    [Display(Name = "TC kimlik no")]
    public string PersonelTc { get; set; } = "";

    [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum tarihi")]
    public DateTime PersonelDogumTarihi { get; set; }

    [Required(ErrorMessage = "Cinsiyet seçmelisiniz.")]
    [Display(Name = "Cinsiyet")]
    public string PersonelCinsiyet { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string PersonelTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string PersonelEposta { get; set; } = "";

    [Required(ErrorMessage = "Pozisyon zorunludur.")]
    [Display(Name = "Pozisyon")]
    public string PersonelPozisyon { get; set; } = "";

    [Required(ErrorMessage = "İşe Giriş tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "İşe Giriş Tarihi")]
    public DateTime PersonelIseGirisTarihi { get; set; }

    [Required(ErrorMessage = "Yıllık izin hakkı zorunludur.")]
    [Range(0, 14, ErrorMessage = "Yıllık izin hakkı 0 ile 14 arasında olmalıdır.")]
    [Display(Name = "Yıllık İzin Hakkı")]
    public int YillikIzinHakki { get; set; } = 14;

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool AktifMi { get; set; } = true;

    [Display(Name = "Departman Adı")]
    public string DepartmanAdi { get; set; } = "";

    public string TamAd => PersonelAd + " " + PersonelSoyad;

    public int KullanilanYillikIzin { get; set; }

    public int KalanYillikIzin =>
        Math.Max(0, YillikIzinHakki - KullanilanYillikIzin);
}