using Microsoft.AspNetCore.Mvc;
using HR.Data;

namespace HR.Controllers;

public class HomeController : Controller
{
    private readonly DashboardRepository _dashboardRepo;

    public HomeController(
        DashboardRepository dashboardRepo)
    {
        _dashboardRepo = dashboardRepo;
    }

    public IActionResult Index()
    {
        var model = new HR.Models.DashboardViewModel();

        model.ToplamPersonelSayisi =
            _dashboardRepo.ToplamPersonelSayisi();

        model.BekleyenIzinSayisi =
            _dashboardRepo.BekleyenIzinSayisi();

        model.BuAyIzindeOlanPersonelSayisi =
            _dashboardRepo.BuAyIzindeOlanPersonelSayisi();

        model.DepartmanDagilimi =
            _dashboardRepo.DepartmanDagilimi();

        model.BekleyenIzinler =
            _dashboardRepo.SonBesBekleyenIzin();

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}