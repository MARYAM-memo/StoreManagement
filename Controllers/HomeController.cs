using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models;
using StoreManagement.ViewModels;

namespace StoreManagement.Controllers;

public class HomeController(IDashboardService dbSrv, ILogger<HomeController> logger) : Controller
{
    readonly IDashboardService dbService = dbSrv;
    readonly ILogger<HomeController> _logger = logger;
    public async Task<IActionResult> Index()
    {
        try
        {
            var stats = await dbService.GetDashboardStatsAsync();
            return View(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard statistics");
            return View(new DashboardStatsVM());
        }

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
