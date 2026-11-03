using System.ComponentModel.DataAnnotations;

namespace HR.Models;

public class Departman
{
    public long DepartmanId { get; set; }

    [Required(ErrorMessage = "Departman adı zorunludur.")]
    [StringLength(100, ErrorMessage = "En fazla 100 karakter olabilir.")]
    [Display(Name = "Departman adı")]
    public string DepartmanAd { get; set; } = "";

    [StringLength(500, ErrorMessage = "En fazla 500 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public bool AktifMi { get; set; } = true;

    [Display(Name = "Personel sayısı")]
    public int PersonelSayisi { get; set; }
}