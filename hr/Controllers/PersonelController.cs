using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using HR.Data;
using HR.Models;

public class PersonelController : Controller
{
    private readonly PersonelRepository _personelRepo;
    private readonly DepartmanRepository _departmanRepo;

    public PersonelController(
        PersonelRepository personelRepo,
        DepartmanRepository departmanRepo)
    {
        _personelRepo = personelRepo;
        _departmanRepo = departmanRepo;
    }

    private void DepartmanListesiniHazirla(long? secili = null)
    {
        var departmanlar = _departmanRepo.TumunuGetir();

        var liste = departmanlar.Select(d => new
        {
            Id = d.DepartmanId,
            Ad = d.DepartmanAd
        }).ToList();

        ViewBag.Departmanlar = new SelectList(
            liste,
            "Id",
            "Ad",
            secili
        );
    }

    public IActionResult Index(string aramaKelimesi, int sayfa = 1)
    {
        int sayfaBoyutu = 5;

        int toplamKayit = _personelRepo.PersonelSayisiGetir(aramaKelimesi);
        int toplamSayfa = (int)Math.Ceiling((double)toplamKayit / sayfaBoyutu);

        List<Personel> personeller = _personelRepo.AramaVeSayfalamaIleGetir(aramaKelimesi, sayfa, sayfaBoyutu);

        ViewBag.AramaKelimesi = aramaKelimesi;
        ViewBag.MevcutSayfa = sayfa;
        ViewBag.ToplamSayfa = toplamSayfa;

        return View(personeller);
    }

    public IActionResult Create()
    {
        DepartmanListesiniHazirla();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Personel personel)
    {
        if (!ModelState.IsValid)
        {
            DepartmanListesiniHazirla(personel.DepartmanId);

            return View(personel);
        }

        try
        {
            _personelRepo.Ekle(personel);

            TempData["Basarili"] = $"{personel.TamAd} kaydedildi.";

            return RedirectToAction("Index");
        }
        catch (SqlException ex)
        {
            BenzersizlikHatasiniIsle(ex);

            DepartmanListesiniHazirla(personel.DepartmanId);

            return View(personel);
        }
    }

    public IActionResult Edit(long id)
    {
        Personel? personel = _personelRepo.IdIleGetir(id);

        if (personel == null)
        {
            return NotFound();
        }

        DepartmanListesiniHazirla(personel.DepartmanId);

        return View(personel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Personel personel)
    {
        if (!ModelState.IsValid)
        {
            DepartmanListesiniHazirla(personel.DepartmanId);

            return View(personel);
        }

        try
        {
            _personelRepo.Guncelle(personel);

            TempData["Basarili"] = $"{personel.TamAd} güncellendi.";

            return RedirectToAction("Index");
        }
        catch (SqlException ex)
        {
            BenzersizlikHatasiniIsle(ex);

            DepartmanListesiniHazirla(personel.DepartmanId);

            return View(personel);
        }
    }

    public IActionResult Delete(long id)
    {
        Personel? personel = _personelRepo.IdIleGetir(id);

        if (personel == null)
        {
            return NotFound();
        }

        return View(personel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(long id)
    {
        _personelRepo.PasifYap(id);

        TempData["Basarili"] = "Personel silindi.";

        return RedirectToAction("Index");
    }

    public IActionResult Details(long id)
    {
        Personel? personel = _personelRepo.IdIleGetir(id);

        if (personel == null)
            return NotFound();

        personel.KullanilanYillikIzin =
            _personelRepo.KullanilanYillikIzinGunSayisi(id);

        return View(personel);
    }

    private void BenzersizlikHatasiniIsle(SqlException ex)
    {
        if ((ex.Number == 2601 || ex.Number == 2627) &&
            ex.Message.Contains("eposta") ||
            ex.Message.Contains("e_posta"))
        {
            ModelState.AddModelError(
                "PersonelEposta",
                "Bu e-posta adresi başka bir personelde kayıtlı."
            );
        }
        else if ((ex.Number == 2601 || ex.Number == 2627) &&
                 ex.Message.Contains("tc"))
        {
            ModelState.AddModelError(
                "PersonelTc",
                "Bu TC kimlik numarası başka bir personelde kayıtlı."
            );
        }
        else
        {
            ModelState.AddModelError(
                "",
                "Kayıt sırasında bir sorun oluştu. Lütfen tekrar deneyin."
            );
        }
    }
}