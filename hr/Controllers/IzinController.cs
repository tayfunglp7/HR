using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HR.Data;
using HR.Models;
using System.Text;

namespace HR.Controllers;

public class IzinController : Controller
{
    private readonly IzinRepository _izinRepo;
    private readonly PersonelRepository _personelRepo;

    public IzinController(
        IzinRepository izinRepo,
        PersonelRepository personelRepo)
    {
        _izinRepo = izinRepo;
        _personelRepo = personelRepo;
    }

    private void PersonelListesiniHazirla(long? secili = null)
    {
        var personeller = _personelRepo.TumunuGetir();

        ViewBag.Personeller = new SelectList(
            personeller,
            "PersonelId",
            "TamAd",
            secili
        );
    }

    public IActionResult Index()
    {
        var liste = _izinRepo.TumunuGetir();

        return View(liste);
    }

    public IActionResult Create()
    {
        PersonelListesiniHazirla();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Izin izin)
    {
        if (izin.BitisTarihi < izin.BaslangicTarihi)
        {
            ModelState.AddModelError(
                "BitisTarihi",
                "Bitiş tarihi başlangıç tarihinden önce olamaz."
            );
        }

        if (!ModelState.IsValid)
        {
            PersonelListesiniHazirla(izin.PersonelId);

            return View(izin);
        }

        int hesaplananGun = 0;
        for (DateTime tarih = izin.BaslangicTarihi.Date; tarih <= izin.BitisTarihi.Date; tarih = tarih.AddDays(1))
        {
            if (tarih.DayOfWeek != DayOfWeek.Saturday && tarih.DayOfWeek != DayOfWeek.Sunday)
            {
                hesaplananGun++;
            }
        }
        izin.GunSayisi = hesaplananGun;

        if (izin.GunSayisi <= 0)
        {
            ModelState.AddModelError("BitisTarihi", "Seçilen tarih aralığında geçerli bir iş günü bulunmuyor.");
            PersonelListesiniHazirla(izin.PersonelId);
            return View(izin);
        }

        izin.Durum = "Beklemede";

        _izinRepo.Ekle(izin);

        TempData["Basarili"] =
            $"\"{izin.IzinTipi}\" izni eklendi.";

        return RedirectToAction("Index");
    }

    public IActionResult Edit(long id)
    {
        Izin? izin = _izinRepo.IdIleGetir(id);

        if (izin == null)
            return NotFound();


        PersonelListesiniHazirla(izin.PersonelId);

        return View(izin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Izin izin)
    {
        if (izin.BitisTarihi < izin.BaslangicTarihi)
        {
            ModelState.AddModelError(
                "BitisTarihi",
                "Bitiş tarihi başlangıç tarihinden önce olamaz."
            );
        }


        if (!ModelState.IsValid)
        {
            PersonelListesiniHazirla(izin.PersonelId);

            return View(izin);
        }

        int hesaplananGun = 0;
        for (DateTime tarih = izin.BaslangicTarihi.Date; tarih <= izin.BitisTarihi.Date; tarih = tarih.AddDays(1))
        {
            if (tarih.DayOfWeek != DayOfWeek.Saturday && tarih.DayOfWeek != DayOfWeek.Sunday)
            {
                hesaplananGun++;
            }
        }
        izin.GunSayisi = hesaplananGun;

        if (izin.GunSayisi <= 0)
        {
            ModelState.AddModelError("BitisTarihi", "Seçilen tarih aralığında geçerli bir iş günü bulunmuyor.");
            PersonelListesiniHazirla(izin.PersonelId);
            return View(izin);
        }


        _izinRepo.Guncelle(izin);

        TempData["Basarili"] = "İzin güncellendi.";

        return RedirectToAction("Index");
    }

    public IActionResult Details(long id)
    {
        Izin? izin = _izinRepo.IdIleGetir(id);

        if (izin == null)
            return NotFound();

        return View(izin);
    }

    public IActionResult Delete(long id)
    {
        Izin? izin = _izinRepo.IdIleGetir(id);

        if (izin == null)
            return NotFound();

        return View(izin);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(long id)
    {
        _izinRepo.PasifYap(id);

        TempData["Basarili"] = "İzin silindi.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Onayla(long id)
    {
        _izinRepo.Onayla(id);

        TempData["Basarili"] = "İzin onaylandı.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reddet(long id)
    {
        _izinRepo.Reddet(id);

        TempData["Basarili"] = "İzin reddedildi.";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult IzinleriIndir()
    {
        var izinler = _izinRepo.TumunuGetir();

        StringBuilder sb = new StringBuilder();

        sb.Append('\uFEFF');

        sb.AppendLine("Personel;Departman;İzin Tipi;Başlangıç Tarihi;Bitiş Tarihi;Gün Sayısı;Durum;Kayıt Tarihi");

        foreach (var izin in izinler)
        {
            sb.AppendLine($"{izin.PersonelAdSoyad};{izin.DepartmanAdi};{izin.IzinTipi};{izin.BaslangicTarihi:dd.MM.yyyy};{izin.BitisTarihi:dd.MM.yyyy};{izin.GunSayisi};{izin.Durum};{izin.CreatedDate:dd.MM.yyyy HH:mm}");
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "izin_listesi.csv");
    }
}