using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using HR.Models;

namespace HR.Data;

public class KullaniciRepository
{
    private readonly string _baglantiMetni;

    public KullaniciRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("HRDb")!;
    }

    public static string SifreyiHashle(string sifre)
    {
        byte[] bayt = Encoding.UTF8.GetBytes(sifre);   
        byte[] hash = SHA256.HashData(bayt);           
        return Convert.ToHexString(hash);              
    }

    public Kullanici? Dogrula(string kullaniciAdi, string sifre)
    {
        string hash = SifreyiHashle(sifre);

        string sql = @"SELECT kullanici_id, kullanici_adi, ad_soyad,
                              created_date, aktif_mi
                       FROM kullanici
                       WHERE kullanici_adi = @ad
                         AND sifre_hash    = @hash
                         AND aktif_mi      = 1";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            
            komut.Parameters.AddWithValue("@ad", kullaniciAdi);
            komut.Parameters.AddWithValue("@hash", hash);

            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                if (okuyucu.Read())
                {
                    return new Kullanici
                    {
                        KullaniciId = okuyucu.GetInt64(okuyucu.GetOrdinal("kullanici_id")),
                        KullaniciAdi = okuyucu.GetString(okuyucu.GetOrdinal("kullanici_adi")),
                        AdSoyad = okuyucu.GetString(okuyucu.GetOrdinal("ad_soyad")),
                        CreatedDate = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date")),

                        AktifMi = okuyucu.GetBoolean(okuyucu.GetOrdinal("aktif_mi"))
                    };
                }
            }
        }

        return null;  
    }
}