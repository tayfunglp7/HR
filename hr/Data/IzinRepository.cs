using Microsoft.Data.SqlClient;
using HR.Models;

namespace HR.Data;

public class IzinRepository
{
    private readonly string _baglantiMetni;

    public IzinRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("HRDb")!;
    }

    private Izin SatiriNesneyeCevir(SqlDataReader okuyucu)
    {
        Izin i = new Izin();

        i.IzinId = okuyucu.GetInt64(
            okuyucu.GetOrdinal("izin_id"));

        i.PersonelId = okuyucu.GetInt64(
            okuyucu.GetOrdinal("personel_id"));

        i.IzinTipi = okuyucu.GetString(
            okuyucu.GetOrdinal("izin_tipi"));

        i.BaslangicTarihi = okuyucu.GetDateTime(
            okuyucu.GetOrdinal("baslangic_tarihi"));

        i.BitisTarihi = okuyucu.GetDateTime(
            okuyucu.GetOrdinal("bitis_tarihi"));

        i.GunSayisi = okuyucu.GetInt32(
            okuyucu.GetOrdinal("gun_sayisi"));

        i.Durum = okuyucu.GetString(
            okuyucu.GetOrdinal("durum"));

        i.CreatedDate = okuyucu.GetDateTime(
            okuyucu.GetOrdinal("created_date"));

        int aciklamaSutun = okuyucu.GetOrdinal("aciklama");

        i.Aciklama = okuyucu.IsDBNull(aciklamaSutun)
            ? null
            : okuyucu.GetString(aciklamaSutun);

        int updatedDateSutun = okuyucu.GetOrdinal("updated_date");

        i.UpdatedDate = okuyucu.IsDBNull(updatedDateSutun)
            ? null
            : okuyucu.GetDateTime(updatedDateSutun);

        i.AktifMi = okuyucu.GetBoolean(
            okuyucu.GetOrdinal("aktif_mi"));


        int personelAdSoyadSutun =
            okuyucu.GetOrdinal("personel_ad_soyad");

        i.PersonelAdSoyad =
            okuyucu.GetString(personelAdSoyadSutun);


        int departmanAdiSutun =
            okuyucu.GetOrdinal("departman_ad");

        i.DepartmanAdi =
            okuyucu.GetString(departmanAdiSutun);


        return i;
    }

    public List<Izin> TumunuGetir()
    {
        List<Izin> liste = new List<Izin>();

        string sql = @"
            SELECT
                i.izin_id,
                i.personel_id,
                i.izin_tipi,
                i.baslangic_tarihi,
                i.bitis_tarihi,
                i.gun_sayisi,
                i.aciklama,
                i.durum,
                i.created_date,
                i.updated_date,
                i.aktif_mi,

                p.ad + ' ' + p.soyad AS personel_ad_soyad,
                d.departman_ad

            FROM izin i

            INNER JOIN personel p
                ON i.personel_id = p.personel_id

            INNER JOIN departman d
                ON p.departman_id = d.departman_id

            WHERE i.aktif_mi = 1

            ORDER BY i.created_date DESC";


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
                    Izin i = SatiriNesneyeCevir(okuyucu);

                    liste.Add(i);
                }
            }
        }

        return liste;
    }

    public Izin? IdIleGetir(long id)
    {
        Izin? sonuc = null;

        string sql = @"
            SELECT
                i.izin_id,
                i.personel_id,
                i.izin_tipi,
                i.baslangic_tarihi,
                i.bitis_tarihi,
                i.gun_sayisi,
                i.aciklama,
                i.durum,
                i.created_date,
                i.updated_date,
                i.aktif_mi,

                p.ad + ' ' + p.soyad AS personel_ad_soyad,
                d.departman_ad

            FROM izin i

            INNER JOIN personel p
                ON i.personel_id = p.personel_id

            INNER JOIN departman d
                ON p.departman_id = d.departman_id

            WHERE i.izin_id = @id
              AND i.aktif_mi = 1";


        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@id", id);

            baglanti.Open();

            using (SqlDataReader okuyucu =
                   komut.ExecuteReader())
            {
                if (okuyucu.Read())
                {
                    sonuc = SatiriNesneyeCevir(okuyucu);
                }
            }
        }

        return sonuc;
    }

    public void Ekle(Izin izin)
    {
        string sql = @"
            INSERT INTO izin
            (
                personel_id,
                izin_tipi,
                baslangic_tarihi,
                bitis_tarihi,
                gun_sayisi,
                aciklama,
                durum,
                created_date,
                updated_date,
                aktif_mi
            )
            VALUES
            (
                @personelId,
                @izinTipi,
                @baslangicTarihi,
                @bitisTarihi,
                @gunSayisi,
                @aciklama,
                @durum,
                @createdDate,
                NULL,
                1
            )";


        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@personelId",
                izin.PersonelId);

            komut.Parameters.AddWithValue(
                "@izinTipi",
                izin.IzinTipi);

            komut.Parameters.AddWithValue(
                "@baslangicTarihi",
                izin.BaslangicTarihi);

            komut.Parameters.AddWithValue(
                "@bitisTarihi",
                izin.BitisTarihi);

            komut.Parameters.AddWithValue(
                "@gunSayisi",
                izin.GunSayisi);

            komut.Parameters.AddWithValue(
                "@aciklama",
                (object?)izin.Aciklama ?? DBNull.Value);

            komut.Parameters.AddWithValue(
                "@durum",
                "Beklemede");

            komut.Parameters.AddWithValue(
                "@createdDate",
                DateTime.Now);


            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }

    public void Guncelle(Izin izin)
    {
        string sql = @"
            UPDATE izin
            SET
                personel_id = @personelId,
                izin_tipi = @izinTipi,
                baslangic_tarihi = @baslangicTarihi,
                bitis_tarihi = @bitisTarihi,
                gun_sayisi = @gunSayisi,
                aciklama = @aciklama,
                updated_date = @updatedDate

            WHERE izin_id = @id";


        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@personelId",
                izin.PersonelId);

            komut.Parameters.AddWithValue(
                "@izinTipi",
                izin.IzinTipi);

            komut.Parameters.AddWithValue(
                "@baslangicTarihi",
                izin.BaslangicTarihi);

            komut.Parameters.AddWithValue(
                "@bitisTarihi",
                izin.BitisTarihi);

            komut.Parameters.AddWithValue(
                "@gunSayisi",
                izin.GunSayisi);

            komut.Parameters.AddWithValue(
                "@aciklama",
                (object?)izin.Aciklama ?? DBNull.Value);

            komut.Parameters.AddWithValue(
                "@updatedDate",
                DateTime.Now);

            komut.Parameters.AddWithValue(
                "@id",
                izin.IzinId);


            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }

    public void PasifYap(long id)
    {
        string sql = @"
            UPDATE izin
            SET
                aktif_mi = 0,
                updated_date = @updatedDate

            WHERE izin_id = @id";


        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@updatedDate",
                DateTime.Now);

            komut.Parameters.AddWithValue(
                "@id",
                id);


            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }


    public void Onayla(long id)
    {
        string sql = @"
            UPDATE izin
            SET
                durum = 'Onaylandı',
                updated_date = @updatedDate

            WHERE izin_id = @id
              AND aktif_mi = 1";


        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@updatedDate",
                DateTime.Now);

            komut.Parameters.AddWithValue(
                "@id",
                id);


            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }

    public void Reddet(long id)
    {
        string sql = @"
            UPDATE izin
            SET
                durum = 'Reddedildi',
                updated_date = @updatedDate

            WHERE izin_id = @id
              AND aktif_mi = 1";


        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@updatedDate",
                DateTime.Now);

            komut.Parameters.AddWithValue(
                "@id",
                id);


            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }
}