using FoodHub.Data;
using FoodHub.Mappers;
using FoodHub.Models;
using FoodHub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Controllers;

// Session 3: CRUD كامل (Create / Read / Update / Delete) بالفورمز + PRG + ViewBag/ViewData/TempData
// الـ DI Container (IControllerActivator) هو اللي بيعمل الـ Controller ويحقن الـ AppDbContext في الـ constructor
// (ده البديل الحديث للـ Controller Factory اللي في منهج MVC 5)
public class RestaurantsController : Controller
{
    private readonly AppDbContext _db;

    public RestaurantsController(AppDbContext db)
    {
        _db = db;
    }

    // ===================== READ =====================

    // GET /Restaurants?category=pizza&q=corner
    // category و q جايين من الـ Query String بالـ Model Binding (اختياريين → string?)
    public async Task<IActionResult> Index(string? category, string? q)
    {
        // AsNoTracking: صفحة عرض بس → مش محتاجين EF يتابع التغييرات (أسرع وأقل ميموري)
        // AsQueryable: لسه مفيش query اتنفذ — بنبني الـ SQL خطوة بخطوة
        var query = _db.Restaurants
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.Reviews)         // محتاجينها عشان AverageRating وعدد التقييمات في الكارت
            .AsQueryable();

        // فلتر بالنوع لو اتبعت: ?category=pizza
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(r => r.Category!.Slug == category);

        // بحث بالاسم أو الوصف: ?q=corner → بيتحول لـ LIKE '%corner%' في SQL
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(r => r.Name.Contains(q) || (r.Description != null && r.Description.Contains(q)));

        // ToListAsync → هنا بس الـ query بيتبعت للداتابيز
        var restaurants = await query
            .OrderByDescending(r => r.IsOpen)    // المفتوح الأول
            .ThenBy(r => r.Name)
            .ToListAsync();

        // Session 3: نبعت بيانات إضافية للـ View غير الـ Model
        ViewBag.Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();  // ViewBag = dynamic
        ViewData["CurrentCategory"] = category;                                                          // ViewData = Dictionary
        ViewData["Search"] = q;
        // ملحوظة: ViewBag و ViewData نفس الـ Dictionary من جوه — ViewBag.X هو نفسه ViewData["X"]

        return View(restaurants);   // الـ Model الأساسي (Strongly typed) → Views/Restaurants/Index.cshtml
    }

    // GET /Restaurants/Details/2   ← الـ 2 جاية من {id?} في الـ route
    public async Task<IActionResult> Details(int id)
    {
        // Filtered Include: نجيب الأصناف المتاحة بس ومترتبة (الـ Popular الأول)
        var restaurant = await _db.Restaurants
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.MenuItems.Where(m => m.IsAvailable).OrderByDescending(m => m.IsPopular).ThenBy(m => m.Name))
            .Include(r => r.Reviews.OrderByDescending(rv => rv.CreatedAt))
            .FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant is null)
            return NotFound();      // 404

        return View(restaurant);
    }

    // ===================== CREATE =====================

    // GET /Restaurants/Create → فورم فاضي
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new RestaurantFormViewModel();
        await FillCategoriesAsync(vm);   // نملا الـ dropdown
        return View(vm);
    }

    // POST /Restaurants/Create → الفورم اتبعت
    // [ValidateAntiForgeryToken]: بيتأكد إن الـ POST جاي من الفورم بتاعنا (حماية CSRF — تفاصيل في Session 6)
    // الـ token بيتحط في الفورم تلقائي لما نستخدم <form asp-action>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RestaurantFormViewModel vm)
    {
        // Validation يدوي محتاج داتابيز (الـ Annotations متقدرش تعمله): الـ Slug لازم يبقى unique
        if (await _db.Restaurants.AnyAsync(r => r.Slug == vm.Slug))
            ModelState.AddModelError(nameof(vm.Slug), "This slug is already used by another restaurant.");

        if (!ModelState.IsValid)
        {
            await FillCategoriesAsync(vm);     // الـ dropdown مبيرجعش مع الفورم — لازم نملاه تاني
            return View(vm);                    // نفس الفورم + رسايل الأخطاء + القيم اللي كتبها
        }

        var restaurant = new Restaurant();
        vm.ApplyTo(restaurant);                 // ViewModel → Entity (الحقول المسموحة بس)
        _db.Restaurants.Add(restaurant);
        await _db.SaveChangesAsync();           // INSERT — وبعدها restaurant.Id بيتملا من الداتابيز

        // TempData: بيعيش لحد ما يتقري مرة واحدة → بيعدّي الـ Redirect ويظهر في الصفحة الجاية
        TempData["Success"] = $"Restaurant '{restaurant.Name}' was created.";
        return RedirectToAction(nameof(Details), new { id = restaurant.Id });   // PRG: Post → Redirect → Get
    }

    // ===================== UPDATE =====================

    // GET /Restaurants/Edit/2 → الفورم مليان بالقيم الحالية
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // FindAsync: بيدوّر بالـ Primary Key (وبيشوف الـ Change Tracker الأول قبل الداتابيز)
        var restaurant = await _db.Restaurants.FindAsync(id);
        if (restaurant is null)
            return NotFound();

        var vm = restaurant.ToFormViewModel();   // Entity → ViewModel
        await FillCategoriesAsync(vm);
        return View(vm);
    }

    // POST /Restaurants/Edit/2
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RestaurantFormViewModel vm)
    {
        // id جاي من الـ URL و vm.Id جاي من الـ hidden field — لو مختلفين يبقى في حد بيلعب في الفورم
        if (id != vm.Id)
            return BadRequest();    // 400

        // نفس الـ unique check بس نستثني المطعم نفسه (r.Id != id)
        if (await _db.Restaurants.AnyAsync(r => r.Slug == vm.Slug && r.Id != id))
            ModelState.AddModelError(nameof(vm.Slug), "This slug is already used by another restaurant.");

        if (!ModelState.IsValid)
        {
            await FillCategoriesAsync(vm);
            return View(vm);
        }

        var restaurant = await _db.Restaurants.FindAsync(id);
        if (restaurant is null)
            return NotFound();

        vm.ApplyTo(restaurant);               // Tracked entity → EF هيعمل UPDATE للأعمدة اللي اتغيرت بس
        await _db.SaveChangesAsync();

        TempData["Success"] = "Changes saved.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ===================== DELETE =====================

    // GET /Restaurants/Delete/2 → صفحة تأكيد (عمرنا ما نمسح بـ GET — لينك ممكن يتفتح بالغلط أو من bot)
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var restaurant = await _db.Restaurants
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.MenuItems)       // عشان نقول للمستخدم كام صنف هيتمسح معاه
            .FirstOrDefaultAsync(r => r.Id == id);

        return restaurant is null ? NotFound() : View(restaurant);
    }

    // POST /Restaurants/Delete/2 — نفس الـ URL بس اسم الـ method مختلف
    // C# مش بيسمح بـ method تانية بنفس الاسم ونفس الـ parameters (int id)
    // فبنسمّيها DeleteConfirmed و [ActionName("Delete")] بتقول للـ MVC إن اسمها في الـ Routing هو Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var restaurant = await _db.Restaurants.FindAsync(id);
        if (restaurant is null)
            return NotFound();

        _db.Restaurants.Remove(restaurant);   // الـ MenuItems والـ Reviews بيتمسحوا معاه (Cascade Delete)
        await _db.SaveChangesAsync();

        TempData["Success"] = $"'{restaurant.Name}' was deleted.";
        return RedirectToAction(nameof(Index));
    }

    // ===================== Helpers =====================

    // private → مش Action (الـ Routing مش بيشوف الـ private methods)
    // SelectList(items, valueField, textField, selectedValue) → <option value="Id">Name</option>
    private async Task FillCategoriesAsync(RestaurantFormViewModel vm)
    {
        var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        vm.Categories = new SelectList(categories, nameof(Category.Id), nameof(Category.Name), vm.CategoryId);
    }
}
