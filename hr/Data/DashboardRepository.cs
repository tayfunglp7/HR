using Microsoft.Data.SqlClient;
using HR.Models;

namespace HR.Data;

public class DashboardRepository
{
    private readonly string _baglantiMetni;

    public DashboardRepository(IConfiguration configuration)
    {
        _baglantiMetni =
            configuration.GetConnectionString("HRDb")!;
    }

    public int ToplamPersonelSayisi()
    {
        string sql = @"
            SELECT COUNT(*)
            FROM personel
            WHERE aktif_mi = 1";

        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            return Convert.ToInt32(
                komut.ExecuteScalar()
            );
        }
    }

    public int BekleyenIzinSayisi()
    {
        string sql = @"
            SELECT COUNT(*)
            FROM izin
            WHERE aktif_mi = 1
              AND durum = 'Beklemede'";

        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            return Convert.ToInt32(
                komut.ExecuteScalar()
            );
        }
    }

    public int BuAyIzindeOlanPersonelSayisi()
    {
        string sql = @"
            SELECT COUNT(DISTINCT personel_id)
            FROM izin
            WHERE aktif_mi = 1
              AND durum = 'Onaylandı'
              AND baslangic_tarihi <= EOMONTH(GETDATE())
              AND bitis_tarihi >=
                  DATEFROMPARTS(
                      YEAR(GETDATE()),
                      MONTH(GETDATE()),
                      1
                  )";

        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            return Convert.ToInt32(
                komut.ExecuteScalar()
            );
        }
    }

    public List<DashboardDepartman> DepartmanDagilimi()
    {
        List<DashboardDepartman> liste =
            new List<DashboardDepartman>();

        string sql = @"
            SELECT
                d.departman_ad,
                COUNT(p.personel_id) AS personel_sayisi
            FROM departman d

            LEFT JOIN personel p
                ON d.departman_id = p.departman_id
                AND p.aktif_mi = 1

            WHERE d.aktif_mi = 1

            GROUP BY
                d.departman_id,
                d.departman_ad

            ORDER BY
                COUNT(p.personel_id) DESC";

        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            using (SqlDataReader okuyucu =
                   komut.ExecuteReader())
            {
                while (okuyucu.Read())
                {
                    DashboardDepartman departman =
                        new DashboardDepartman();

                    departman.DepartmanAdi =
                        okuyucu.GetString(
                            okuyucu.GetOrdinal("departman_ad")
                        );

                    departman.PersonelSayisi =
                        okuyucu.GetInt32(
                            okuyucu.GetOrdinal("personel_sayisi")
                        );

                    liste.Add(departman);
                }
            }
        }

        return liste;
    }

    public List<DashboardBekleyenIzin> SonBesBekleyenIzin()
    {
        List<DashboardBekleyenIzin> liste =
            new List<DashboardBekleyenIzin>();

        string sql = @"
            SELECT TOP 5

                i.izin_id,

                p.ad + ' ' + p.soyad
                    AS personel_ad_soyad,

                d.departman_ad,

                i.izin_tipi,

                i.baslangic_tarihi,

                i.bitis_tarihi,

                i.gun_sayisi,

                i.created_date

            FROM izin i

            INNER JOIN personel p
                ON i.personel_id = p.personel_id

            INNER JOIN departman d
                ON p.departman_id = d.departman_id

            WHERE i.aktif_mi = 1
              AND i.durum = 'Beklemede'

            ORDER BY
                i.created_date DESC";

        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            using (SqlDataReader okuyucu =
                   komut.ExecuteReader())
            {
                while (okuyucu.Read())
                {
                    DashboardBekleyenIzin izin =
                        new DashboardBekleyenIzin();

                    izin.IzinId =
                        okuyucu.GetInt64(
                            okuyucu.GetOrdinal("izin_id")
                        );

                    izin.PersonelAdSoyad =
                        okuyucu.GetString(
                            okuyucu.GetOrdinal(
                                "personel_ad_soyad")
                        );

                    izin.DepartmanAdi =
                        okuyucu.GetString(
                            okuyucu.GetOrdinal(
                                "departman_ad")
                        );

                    izin.IzinTipi =
                        okuyucu.GetString(
                            okuyucu.GetOrdinal(
                                "izin_tipi")
                        );

                    izin.BaslangicTarihi =
                        okuyucu.GetDateTime(
                            okuyucu.GetOrdinal(
                                "baslangic_tarihi")
                        );

                    izin.BitisTarihi =
                        okuyucu.GetDateTime(
                            okuyucu.GetOrdinal(
                                "bitis_tarihi")
                        );

                    izin.GunSayisi =
                        okuyucu.GetInt32(
                            okuyucu.GetOrdinal(
                                "gun_sayisi")
                        );

                    izin.CreatedDate =
                        okuyucu.GetDateTime(
                            okuyucu.GetOrdinal(
                                "created_date")
                        );

                    liste.Add(izin);
                }
            }
        }

        return liste;
    }
}