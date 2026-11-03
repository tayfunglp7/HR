using Microsoft.Data.SqlClient;
using HR.Models;

namespace HR.Data;

public class PersonelRepository
{
    private readonly string _baglantiMetni;

    public PersonelRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("HRDb")!;
    }

    private Personel SatiriNesneyeCevir(SqlDataReader okuyucu)
    {
        Personel p = new Personel();

        p.PersonelId = okuyucu.GetInt64(okuyucu.GetOrdinal("personel_id"));
        p.DepartmanId = okuyucu.GetInt64(okuyucu.GetOrdinal("departman_id"));

        p.PersonelAd = okuyucu.GetString(okuyucu.GetOrdinal("ad"));
        p.PersonelSoyad = okuyucu.GetString(okuyucu.GetOrdinal("soyad"));

        p.PersonelTc = okuyucu.GetString(okuyucu.GetOrdinal("tc"));

        p.PersonelDogumTarihi =
            okuyucu.GetDateTime(okuyucu.GetOrdinal("dogum_tarihi"));

        p.PersonelCinsiyet =
            okuyucu.GetString(okuyucu.GetOrdinal("cinsiyet"));

        p.PersonelTelefon =
            okuyucu.GetString(okuyucu.GetOrdinal("telefon"));

        p.PersonelEposta =
            okuyucu.GetString(okuyucu.GetOrdinal("e_posta"));

        p.PersonelPozisyon =
            okuyucu.GetString(okuyucu.GetOrdinal("pozisyon"));

        p.PersonelIseGirisTarihi =
            okuyucu.GetDateTime(okuyucu.GetOrdinal("ise_giris_tarihi"));

        p.YillikIzinHakki =
            okuyucu.GetInt32(okuyucu.GetOrdinal("yillik_izin_hakki"));

        p.CreatedDate =
            okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));

        p.AktifMi =
            okuyucu.GetBoolean(okuyucu.GetOrdinal("aktif_mi"));

        int sutunNo = okuyucu.GetOrdinal("updated_date");

        p.UpdatedDate = okuyucu.IsDBNull(sutunNo)
            ? null
            : okuyucu.GetDateTime(sutunNo);

        return p;
    }

    public List<Personel> TumunuGetir()
    {
        List<Personel> liste = new List<Personel>();

        string sql = @"SELECT p.personel_id, p.departman_id, p.ad, p.soyad, p.tc, p.dogum_tarihi, p.cinsiyet,
             p.telefon, p.e_posta, p.pozisyon, p.ise_giris_tarihi, p.yillik_izin_hakki, 
             p.created_date, p.updated_date, p.aktif_mi, d.departman_ad
             FROM personel p
            INNER JOIN departman d
                ON p.departman_id = d.departman_id
            WHERE p.aktif_mi = 1
            ORDER BY p.ad, p.soyad";   
                
        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                while (okuyucu.Read())
                {
                    Personel p = SatiriNesneyeCevir(okuyucu);

                    p.DepartmanAdi =
                        okuyucu.GetString(okuyucu.GetOrdinal("departman_ad"));

                    liste.Add(p);
                }
            }
        }

        return liste;
    }

    public int PersonelSayisiGetir(string arama)
{
    string sql = @"
        SELECT COUNT(*) FROM personel p
        INNER JOIN departman d ON p.departman_id = d.departman_id
        WHERE p.aktif_mi = 1";

    if (!string.IsNullOrWhiteSpace(arama))
    {
        sql += " AND (p.ad LIKE @arama OR p.soyad LIKE @arama OR p.tc LIKE @arama OR d.departman_ad LIKE @arama)";
    }

    using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
    using (SqlCommand komut = new SqlCommand(sql, baglanti))
    {
        if (!string.IsNullOrWhiteSpace(arama))
        {
            komut.Parameters.AddWithValue("@arama", "%" + arama + "%");
        }
        
        baglanti.Open();
        return Convert.ToInt32(komut.ExecuteScalar());
    }
}

public List<Personel> AramaVeSayfalamaIleGetir(string arama, int sayfa, int sayfaBoyutu)
{
    List<Personel> liste = new List<Personel>();
    string sql = @"
        SELECT p.personel_id, p.departman_id, p.ad, p.soyad, p.tc, p.dogum_tarihi, p.cinsiyet,
               p.telefon, p.e_posta, p.pozisyon, p.ise_giris_tarihi, p.yillik_izin_hakki, 
               p.created_date, p.updated_date, p.aktif_mi, d.departman_ad
        FROM personel p
        INNER JOIN departman d ON p.departman_id = d.departman_id
        WHERE p.aktif_mi = 1";

    if (!string.IsNullOrWhiteSpace(arama))
    {
        sql += " AND (p.ad LIKE @arama OR p.soyad LIKE @arama OR p.tc LIKE @arama OR d.departman_ad LIKE @arama)";
    }

    sql += @" ORDER BY p.ad, p.soyad
              OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";

    using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
    using (SqlCommand komut = new SqlCommand(sql, baglanti))
    {
        if (!string.IsNullOrWhiteSpace(arama))
        {
            komut.Parameters.AddWithValue("@arama", "%" + arama + "%");
        }
        
        komut.Parameters.AddWithValue("@offset", (sayfa - 1) * sayfaBoyutu);
        komut.Parameters.AddWithValue("@limit", sayfaBoyutu);

        baglanti.Open();
        using (SqlDataReader okuyucu = komut.ExecuteReader())
        {
            while (okuyucu.Read())
            {
                Personel p = SatiriNesneyeCevir(okuyucu);
                p.DepartmanAdi = okuyucu.GetString(okuyucu.GetOrdinal("departman_ad"));
                liste.Add(p);
            }
        }
    }
    return liste;
}

    public Personel? IdIleGetir(long id)
    {
        Personel? sonuc = null;

        string sql = @"SELECT p.personel_id, p.departman_id, p.ad, p.soyad, p.tc, p.dogum_tarihi, p.cinsiyet,
                    p.telefon, p.e_posta, p.pozisyon, p.ise_giris_tarihi, p.yillik_izin_hakki, 
                    p.created_date, p.updated_date, p.aktif_mi, d.departman_ad
                    FROM personel p
                    INNER JOIN departman d
                    ON p.departman_id = d.departman_id
                    WHERE p.personel_id = @id
                    AND p.aktif_mi = 1";
                
        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@id", id);

            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                if (okuyucu.Read())
                {
                    sonuc = SatiriNesneyeCevir(okuyucu);

                    sonuc.DepartmanAdi =
                        okuyucu.GetString(
                            okuyucu.GetOrdinal("departman_ad"));
                }
            }
        }

        return sonuc;
    }

    public void Ekle(Personel personel)
    {
        string sql = @"
            INSERT INTO personel
            (
                departman_id,
                ad,
                soyad,
                tc,
                dogum_tarihi,
                cinsiyet,
                telefon,
                e_posta,
                pozisyon,
                ise_giris_tarihi,
                yillik_izin_hakki,
                created_date,
                updated_date,
                aktif_mi
            )
            VALUES
            (
                @departmanId,
                @ad,
                @soyad,
                @tc,
                @dogumTarihi,
                @cinsiyet,
                @telefon,
                @eposta,
                @pozisyon,
                @iseGirisTarihi,
                @yillikIzinHakki,
                @createdDate,
                NULL,
                1
            )";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@departmanId", personel.DepartmanId);

            komut.Parameters.AddWithValue(
                "@ad", personel.PersonelAd);

            komut.Parameters.AddWithValue(
                "@soyad", personel.PersonelSoyad);

            komut.Parameters.AddWithValue(
                "@tc", personel.PersonelTc);

            komut.Parameters.AddWithValue(
                "@dogumTarihi", personel.PersonelDogumTarihi);

            komut.Parameters.AddWithValue(
                "@cinsiyet", personel.PersonelCinsiyet);

            komut.Parameters.AddWithValue(
                "@telefon", personel.PersonelTelefon);

            komut.Parameters.AddWithValue(
                "@eposta", personel.PersonelEposta);

            komut.Parameters.AddWithValue(
                "@pozisyon", personel.PersonelPozisyon);

            komut.Parameters.AddWithValue(
                "@iseGirisTarihi", personel.PersonelIseGirisTarihi);

            komut.Parameters.AddWithValue(
                "@yillikIzinHakki", personel.YillikIzinHakki);

            komut.Parameters.AddWithValue(
                "@createdDate", DateTime.Now);

            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }

    public void Guncelle(Personel personel)
    {
        string sql = @"
            UPDATE personel
            SET
                departman_id = @departmanId,
                ad = @ad,
                soyad = @soyad,
                tc = @tc,
                dogum_tarihi = @dogumTarihi,
                cinsiyet = @cinsiyet,
                telefon = @telefon,
                e_posta = @eposta,
                pozisyon = @pozisyon,
                ise_giris_tarihi = @iseGirisTarihi,
                yillik_izin_hakki = @yillikIzinHakki,
                updated_date = @updatedDate
            WHERE personel_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@departmanId", personel.DepartmanId);

            komut.Parameters.AddWithValue(
                "@ad", personel.PersonelAd);

            komut.Parameters.AddWithValue(
                "@soyad", personel.PersonelSoyad);

            komut.Parameters.AddWithValue(
                "@tc", personel.PersonelTc);

            komut.Parameters.AddWithValue(
                "@dogumTarihi", personel.PersonelDogumTarihi);

            komut.Parameters.AddWithValue(
                "@cinsiyet", personel.PersonelCinsiyet);

            komut.Parameters.AddWithValue(
                "@telefon", personel.PersonelTelefon);

            komut.Parameters.AddWithValue(
                "@eposta", personel.PersonelEposta);

            komut.Parameters.AddWithValue(
                "@pozisyon", personel.PersonelPozisyon);

            komut.Parameters.AddWithValue(
                "@iseGirisTarihi", personel.PersonelIseGirisTarihi);

            komut.Parameters.AddWithValue(
                "@yillikIzinHakki", personel.YillikIzinHakki);

            komut.Parameters.AddWithValue(
                "@updatedDate", DateTime.Now);

            komut.Parameters.AddWithValue(
                "@id", personel.PersonelId);

            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }

    public void PasifYap(long id)
    {
        string sql = @"
            UPDATE personel
            SET
                aktif_mi = 0,
                updated_date = @updatedDate
            WHERE personel_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@updatedDate", DateTime.Now);

            komut.Parameters.AddWithValue(
                "@id", id);

            baglanti.Open();

            komut.ExecuteNonQuery();
        }
    }

    public int KullanilanYillikIzinGunSayisi(long personelId)
    {
        string sql = @"
        SELECT ISNULL(SUM(gun_sayisi), 0)
        FROM izin
        WHERE personel_id = @personelId
          AND izin_tipi = 'Yıllık'
          AND durum = 'Onaylandı'
          AND aktif_mi = 1
          AND YEAR(baslangic_tarihi) = YEAR(GETDATE())";

        using (SqlConnection baglanti =
               new SqlConnection(_baglantiMetni))

        using (SqlCommand komut =
               new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue(
                "@personelId",
                personelId);

            baglanti.Open();

            return Convert.ToInt32(
                komut.ExecuteScalar());
        }
    }
}