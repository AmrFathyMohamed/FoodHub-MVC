using System.ComponentModel.DataAnnotations;
using FoodHub.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodHub.ViewModels;

// Session 3: ViewModel = شكل الفورم بالظبط (زي الـ DTO في FinShark بس للـ View)
// ليه مش بنربط الفورم بالـ Restaurant Entity مباشرة؟
//   (1) الفورم محتاج حاجات مش في الـ Entity (قايمة الـ Categories للـ dropdown)
//   (2) الـ Entity فيه حاجات ممنوع الفورم يلمسها (CreatedAt, Reviews, MenuItems)
//   (3) Over-Posting: حد يضيف <input name="CreatedAt"> بإيده → الـ ViewModel مفيهوش الحقل ده أصلاً
public class RestaurantFormViewModel
{
    // Hidden field في الفورم — بنستخدمه في Edit بس عشان نتأكد إن الـ id في الـ URL = الـ id في الفورم
    public int Id { get; set; }

    // [Required] + [StringLength] → Validation في السيرفر + data-val-* في المتصفح
    // [Display] → النص اللي بيظهر في <label asp-for>
    [Required, StringLength(100, MinimumLength = 2)]
    [Display(Name = "Restaurant name")]
    public string Name { get; set; } = string.Empty;

    // Slug = الاسم في الـ URL (pizza-corner) → حروف صغيرة وأرقام وشرطة بس
    // Prompt → بيتحط كـ placeholder في الـ input تلقائي
    [Required, StringLength(120)]
    [RegularExpression("^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "Slug: lowercase letters, numbers and dashes only")]
    [Display(Name = "URL slug", Prompt = "pizza-corner")]
    public string Slug { get; set; } = string.Empty;

    // MultilineText → لو استخدمنا EditorFor هيطلع textarea
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required, StringLength(200)]
    public string Address { get; set; } = string.Empty;

    // [EgyptianPhone] = Custom Validation بتاعتنا — من Session 3 بقت شغالة في المتصفح كمان (IClientModelValidator)
    [Required, EgyptianPhone]
    [Display(Name = "Phone number", Prompt = "01012345678")]
    public string Phone { get; set; } = string.Empty;

    // decimal → بيعدّي على FlexibleDecimalModelBinder (بيقبل "١٥" و "15 EGP")
    [Range(0, 200)]
    [Display(Name = "Delivery fee (EGP)")]
    public decimal DeliveryFee { get; set; }

    [Range(0, 1000)]
    [Display(Name = "Minimum order (EGP)")]
    public decimal MinOrder { get; set; }

    // القيمة الافتراضية 30 → بتظهر في فورم Create الفاضي
    [Range(10, 120)]
    [Display(Name = "Delivery time (min)")]
    public int DeliveryMinutes { get; set; } = 30;

    // bool + asp-for → checkbox تلقائي
    [Display(Name = "Open now")]
    public bool IsOpen { get; set; } = true;

    // الـ dropdown بيبعت 0 لو المستخدم مختارش → Range(1, ...) بترفض الـ 0
    [Range(1, int.MaxValue, ErrorMessage = "Choose a cuisine")]
    [Display(Name = "Cuisine")]
    public int CategoryId { get; set; }

    // للـ dropdown بس — مش جاية من الفورم، فبنقول للـ Validation يتجاهلها
    // من غير [ValidateNever] الـ ModelState هيعتبرها Required (Nullable reference types) والفورم عمره ما هيتحفظ
    [ValidateNever]
    public IEnumerable<SelectListItem> Categories { get; set; } = [];
}
