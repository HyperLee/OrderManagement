using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderLunchWeb.Models;

namespace OrderLunchWeb.Controllers;

/// <summary>
/// 首頁控制器
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// 初始化 HomeController
    /// </summary>
    /// <param name="logger">日誌記錄器</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// GET: /Home/Index - 顯示首頁
    /// </summary>
    /// <returns>首頁視圖</returns>
    public IActionResult Index()
    {
        _logger.LogInformation("顯示首頁");
        
        // 傳遞伺服器時間到視圖
        ViewBag.ServerTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        return View();
    }

    /// <summary>
    /// GET: /Home/Privacy - 顯示隱私權頁面
    /// </summary>
    /// <returns>隱私權頁面視圖</returns>
    public IActionResult Privacy()
    {
        _logger.LogInformation("顯示隱私權頁面");
        return View();
    }

    /// <summary>
    /// GET: /Home/Error - 顯示錯誤頁面
    /// </summary>
    /// <returns>錯誤頁面視圖</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
