namespace FoodHub.Areas.Admin.Models;

// ViewModel للـ Dashboard: كل الداتا اللي الصفحة محتاجاها في Object واحد Strongly typed
// (أحسن من ViewData["..."] — فيه IntelliSense ومفيش cast)
public class DashboardViewModel
{
    // ===== أرقام حقيقية من الداتابيز =====
    public int Restaurants { get; set; }
    public int OpenNow { get; set; }
    public int MenuItems { get; set; }
    public int Reviews { get; set; }

    // ===== داتا وهمية (Fake) لحد ما نعمل جدول Orders في Session 6 =====
    public decimal RevenueToday { get; set; }
    public int OrdersToday { get; set; }
    public List<DailyOrders> Week { get; set; } = [];
    public List<RecentOrder> RecentOrders { get; set; } = [];
}

// record = كلاس صغير للداتا بس (Immutable) — سطر واحد بدل كلاس كامل بـ properties
public record DailyOrders(string Day, int Orders);

public record RecentOrder(int Id, string Customer, string Restaurant, decimal Total, string Status, DateTime PlacedAt);
