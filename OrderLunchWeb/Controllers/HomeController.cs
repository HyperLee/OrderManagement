using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderLunchWeb.Models;

namespace OrderLunchWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // 傳遞伺服器時間到視圖
        ViewBag.ServerTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
