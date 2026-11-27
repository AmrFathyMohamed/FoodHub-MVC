using FoodHub.Models;
using FoodHub.ViewModels;

namespace FoodHub.Mappers;

// Session 3: Mapper = تحويل بين الـ Entity والـ ViewModel (نفس فكرة StockMappers في FinShark)
// Extension methods (this) → بنكتب restaurant.ToFormViewModel() كأنها method جوه الكلاس
public static class RestaurantMappers
{
    // Entity → ViewModel: بنستخدمها في GET Edit عشان نملا الفورم بالقيم الحالية
    public static RestaurantFormViewModel ToFormViewModel(this Restaurant r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Slug = r.Slug,
        Description = r.Description,
        Address = r.Address,
        Phone = r.Phone,
        DeliveryFee = r.DeliveryFee,
        MinOrder = r.MinOrder,
        DeliveryMinutes = r.DeliveryMinutes,
        IsOpen = r.IsOpen,
        CategoryId = r.CategoryId
    };

    // ViewModel → Entity: بننسخ الحقول المسموح بيها بس — Id و CreatedAt مش من الفورم (حماية من Over-Posting)
    // بتعدّل Entity موجود (مش بترجّع جديد) → في Edit الـ Entity بيكون Tracked فـ EF يعمل UPDATE للي اتغير بس
    public static void ApplyTo(this RestaurantFormViewModel vm, Restaurant r)
    {
        r.Name = vm.Name.Trim();                                  // نشيل المسافات الزيادة
        r.Slug = vm.Slug.Trim().ToLowerInvariant();               // Slug دايماً small letters
        r.Description = vm.Description?.Trim();                  // ?. عشان ممكن تكون null
        r.Address = vm.Address.Trim();
        r.Phone = vm.Phone.Replace(" ", "").Replace("-", "");     // نخزن الرقم أرقام بس: 01012345678
        r.DeliveryFee = vm.DeliveryFee;
        r.MinOrder = vm.MinOrder;
        r.DeliveryMinutes = vm.DeliveryMinutes;
        r.IsOpen = vm.IsOpen;
        r.CategoryId = vm.CategoryId;
    }
}
