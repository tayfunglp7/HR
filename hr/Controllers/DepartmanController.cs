using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using HR.Data;
using HR.Models;

namespace HR.Controllers;

public class DepartmanController : Controller
{
    private readonly DepartmanRepository _repo;

    public DepartmanController(DepartmanRepository repo)
    {
        _repo = repo;
    }

    private void BenzersizlikHatasiniIsle(SqlException ex)
    {
        if (ex.Number == 2627 || ex.Number == 2601)
        {
            if (ex.Message.Contains("departman_ad"))
            {
                ModelState.AddModelError("DepartmanAd",
                    "Bu departman adı başka bir departmanda kayıtlı.");
            }
        }
        else
        {
            ModelState.AddModelError("",
                "Kayıt sırasında bir sorun oluştu. Lütfen tekrar deneyin.");
        }
    }

    public IActionResult Index()
    {
        return View(_repo.TumunuGetir());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Departman departman)
    {
        if (!ModelState.IsValid)
            return View(departman);

        try
        {
            _repo.Ekle(departman);

            TempData["Basarili"] = $"\"{departman.DepartmanAd}\" departmanı eklendi.";

            return RedirectToAction("Index");
        }
        catch (SqlException ex)
        {
            BenzersizlikHatasiniIsle(ex);

            return View(departman);
        }
    }

    public IActionResult Edit(long id)
    {
        var departman = _repo.IdIleGetir(id);
        if (departman == null) return NotFound();
        return View(departman);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Departman departman)
    {
        if (!ModelState.IsValid)
            return View(departman);

        try
        {
            _repo.Guncelle(departman);

            TempData["Basarili"] = $"\"{departman.DepartmanAd}\" departmanı güncellendi.";

            return RedirectToAction("Index");
        }
        catch (SqlException ex)
        {
            BenzersizlikHatasiniIsle(ex);

            return View(departman);
        }
    }

    public IActionResult Delete(long id)
    {
        var departman = _repo.IdIleGetir(id);

        if (departman == null)
            return NotFound();

        ViewBag.PersonelSayisi = _repo.AktifPersonelSayisi(id);

        return View(departman);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(long id)
    {
        int personelSayisi = _repo.AktifPersonelSayisi(id);

        if (personelSayisi > 0)
        {
            TempData["Uyari"] = $"Bu departmanda {personelSayisi} personel var. " +
                                 "Önce personelleri silmeli veya başka departmana taşımalısınız.";
            return RedirectToAction("Index");
        }

        _repo.PasifYap(id);
        TempData["Basarili"] = "Departman silindi.";
        return RedirectToAction("Index");
    }

    public IActionResult Details(long id)
    {
        var departman = _repo.IdIleGetir(id);

        if (departman == null)
            return NotFound();

        departman.PersonelSayisi = _repo.AktifPersonelSayisi(id);

        return View(departman);
    }
}