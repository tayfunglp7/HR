namespace HR.Models;

public class DashboardViewModel
{
    public int ToplamPersonelSayisi { get; set; }

    public int BekleyenIzinSayisi { get; set; }

    public int BuAyIzindeOlanPersonelSayisi { get; set; }

    public List<DashboardDepartman> DepartmanDagilimi { get; set; }
        = new List<DashboardDepartman>();

    public List<DashboardBekleyenIzin> BekleyenIzinler { get; set; }
        = new List<DashboardBekleyenIzin>();
}

public class DashboardDepartman
{
    public string DepartmanAdi { get; set; } = "";

    public int PersonelSayisi { get; set; }
}

public class DashboardBekleyenIzin
{
    public long IzinId { get; set; }

    public string PersonelAdSoyad { get; set; } = "";

    public string DepartmanAdi { get; set; } = "";

    public string IzinTipi { get; set; } = "";

    public DateTime BaslangicTarihi { get; set; }

    public DateTime BitisTarihi { get; set; }

    public int GunSayisi { get; set; }

    public DateTime CreatedDate { get; set; }
}