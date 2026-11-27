using FoodHub.Areas.Admin.Models;

namespace FoodHub.Areas.Admin.Data;

// داتا وهمية للـ Dashboard — مؤقتة لحد ما نعمل Order و OrderItem في Session 6
// static → مفيش داعي نعمل منها object، بنستخدمها على طول: FakeDashboardData.RecentOrders()
public static class FakeDashboardData
{
    public const decimal RevenueToday = 4_850m;
    public const int OrdersToday = 37;

    // عدد الأوردرات في آخر 7 أيام (للـ bars في الـ Dashboard)
    public static List<DailyOrders> Week() =>
    [
        new("Sat", 42),
        new("Sun", 35),
        new("Mon", 28),
        new("Tue", 31),
        new("Wed", 46),
        new("Thu", 58),
        new("Fri", 37)
    ];

    // آخر الأوردرات — الوقت محسوب من دلوقتي عشان يبان "من كام دقيقة"
    public static List<RecentOrder> RecentOrders()
    {
        var now = DateTime.Now;
        return
        [
            new(1042, "Mona Ali",      "Koshary El Tahrir", 135m, "Pending",    now.AddMinutes(-4)),
            new(1041, "Karim Hassan",  "Pizza Corner",      270m, "Preparing",  now.AddMinutes(-11)),
            new(1040, "Sara Mahmoud",  "Sweet Tooth",        90m, "On the way", now.AddMinutes(-23)),
            new(1039, "Omar Khaled",   "Grill House",       520m, "Delivered",  now.AddMinutes(-48)),
            new(1038, "Nada Youssef",  "Pizza Corner",      150m, "Delivered",  now.AddHours(-1).AddMinutes(-5)),
            new(1037, "Youssef Adel",  "Koshary El Tahrir",  65m, "Cancelled",  now.AddHours(-2))
        ];
    }
}
