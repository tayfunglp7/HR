using Microsoft.Data.SqlClient;
using HR.Models;

namespace HR.Data;

public class DepartmanRepository
{
    private readonly string _baglantiMetni;

    public DepartmanRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("HRDb")!;
    }

    private Departman SatiriNesneyeCevir(SqlDataReader okuyucu)
    {
        Departman d = new Departman();

        d.DepartmanId = okuyucu.GetInt64(okuyucu.GetOrdinal("departman_id"));
        d.DepartmanAd = okuyucu.GetString(okuyucu.GetOrdinal("departman_ad"));
        d.CreatedDate = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));

        d.AktifMi = okuyucu.GetBoolean(okuyucu.GetOrdinal("aktif_mi"));

        int aciklamaSutun = okuyucu.GetOrdinal("aciklama");
        d.Aciklama = okuyucu.IsDBNull(aciklamaSutun)
            ? null
            : okuyucu.GetString(aciklamaSutun);

        int guncellemeSutun = okuyucu.GetOrdinal("updated_date");
        d.UpdatedDate = okuyucu.IsDBNull(guncellemeSutun)
            ? null
            : okuyucu.GetDateTime(guncellemeSutun);

        return d;
    }

    public List<Departman> TumunuGetir()
    {
        List<Departman> liste = new List<Departman>();

        string sql = @"
        SELECT
            d.departman_id,
            d.departman_ad,
            d.aciklama,
            d.created_date,
            d.updated_date,
            d.aktif_mi,
            COUNT(p.personel_id) AS personel_sayisi
        FROM departman d
        INNER JOIN personel p
            ON d.departman_id = p.departman_id
            AND p.aktif_mi = 1
        WHERE d.aktif_mi = 1
        GROUP BY
            d.departman_id,
            d.departman_ad,
            d.aciklama,
            d.created_date,
            d.updated_date,
            d.aktif_mi
        ORDER BY d.departman_ad";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                while (okuyucu.Read())
                {
                    Departman d = SatiriNesneyeCevir(okuyucu);

                    d.PersonelSayisi =
                        okuyucu.GetInt32(
                            okuyucu.GetOrdinal("personel_sayisi")
                        );

                    liste.Add(d);
                }
            }
        }

        return liste;
    }

    public Departman? IdIleGetir(long id)
    {
        Departman? sonuc = null;

        string sql = @"SELECT departman_id, departman_ad, aciklama, created_date, updated_date, aktif_mi
                       FROM departman
                       WHERE departman_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@id", id);
            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                if (okuyucu.Read())
                    sonuc = SatiriNesneyeCevir(okuyucu);
            }
        }

        return sonuc;
    }

    public void Ekle(Departman departman)
    {
        string sql = @"INSERT INTO departman
                          (departman_ad, aciklama, created_date, updated_date, aktif_mi)
                       VALUES
                          (@ad, @aciklama, @createdDate, NULL, 1)";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@ad", departman.DepartmanAd);
            komut.Parameters.AddWithValue("@aciklama",
                (object?)departman.Aciklama ?? DBNull.Value);
            komut.Parameters.AddWithValue("@createdDate", DateTime.Now);

            baglanti.Open();
            komut.ExecuteNonQuery();
        }
    }

    public void Guncelle(Departman departman)
    {
        string sql = @"UPDATE departman
                       SET departman_ad  = @ad,
                           aciklama     = @aciklama,
                           updated_date = @updatedDate
                       WHERE departman_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@ad", departman.DepartmanAd);
            komut.Parameters.AddWithValue(
    "@aciklama",
    (object?)departman.Aciklama ?? DBNull.Value
);
            komut.Parameters.AddWithValue("@updatedDate", DateTime.Now);
            komut.Parameters.AddWithValue("@id", departman.DepartmanId);

            baglanti.Open();
            komut.ExecuteNonQuery();
        }
    }

    public void PasifYap(long id)
    {
        string sql = @"UPDATE departman
                       SET aktif_mi = 0, updated_date = @updatedDate
                       WHERE departman_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@updatedDate", DateTime.Now);
            komut.Parameters.AddWithValue("@id", id);

            baglanti.Open();
            komut.ExecuteNonQuery();
        }
    }

    public int AktifPersonelSayisi(long departmanId)
    {
        string sql = @"SELECT COUNT(*) FROM personel
                       WHERE departman_id = @id AND aktif_mi = 1";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@id", departmanId);
            baglanti.Open();
            return Convert.ToInt32(komut.ExecuteScalar());
        }
    }

}