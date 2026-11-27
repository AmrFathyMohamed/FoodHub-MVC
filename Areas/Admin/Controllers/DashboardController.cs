using FoodHub.Areas.Admin.Data;
using FoodHub.Areas.Admin.Models;
using FoodHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Areas.Admin.Controllers;

// [Area("Admin")] → URL الصفحة: /Admin/Dashboard
// والـ Views بتتدوّر في Areas/Admin/Views/Dashboard الأول
[Area("Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    // GET /Admin/Dashboard
    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            // أرقام حقيقية: CountAsync بيتحول لـ SELECT COUNT(*) في SQL (مش بيجيب الصفوف نفسها)
            Restaurants = await _db.Restaurants.CountAsync(),
            OpenNow = await _db.Restaurants.CountAsync(r => r.IsOpen),
            MenuItems = await _db.MenuItems.CountAsync(),
            Reviews = await _db.Reviews.CountAsync(),

            // داتا وهمية لحد Session 6
            RevenueToday = FakeDashboardData.RevenueToday,
            OrdersToday = FakeDashboardData.OrdersToday,
            Week = FakeDashboardData.Week(),
            RecentOrders = FakeDashboardData.RecentOrders()
        };

        return View(vm);
    }
}
