using System.ComponentModel.DataAnnotations;

namespace HR.Models;

public class Kullanici
{
    public long KullaniciId { get; set; }
    public string KullaniciAdi { get; set; } = "";
    public string SifreHash { get; set; } = "";
    public string AdSoyad { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    public bool AktifMi { get; set; } = true;
}

public class GirisViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı gerekli.")]
    [Display(Name = "Kullanıcı adı")]
    public string KullaniciAdi { get; set; } = "";

    [Required(ErrorMessage = "Şifre gerekli.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Sifre { get; set; } = "";

    [Display(Name = "Beni hatırla")]
    public bool BeniHatirla { get; set; }
}