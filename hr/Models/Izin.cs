using System.ComponentModel.DataAnnotations;

namespace HR.Models;

public class Izin
{
    public long IzinId { get; set; }

    [Required(ErrorMessage = "Personel seçmelisiniz.")]
    [Display(Name = "Personel")]
    public long PersonelId { get; set; }

    [Required(ErrorMessage = "İzin tipi seçmelisiniz.")]
    [StringLength(30)]
    [Display(Name = "İzin Tipi")]
    public string IzinTipi { get; set; } = "";

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Başlangıç Tarihi")]
    public DateTime BaslangicTarihi { get; set; }

    [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Bitiş Tarihi")]
    public DateTime BitisTarihi { get; set; }

    [Display(Name = "Gün Sayısı")]
    public int GunSayisi { get; set; }

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Required(ErrorMessage = "Durum seçmelisiniz.")]
    [StringLength(20)]
    [Display(Name = "Durum")]
    public string Durum { get; set; } = "Beklemede";

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool AktifMi { get; set; } = true;

    [Display(Name = "Personel")]
    public string PersonelAdSoyad { get; set; } = "";

    [Display(Name = "Departman")]
    public string DepartmanAdi { get; set; } = "";
}