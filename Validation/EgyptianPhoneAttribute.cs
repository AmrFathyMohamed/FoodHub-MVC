using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FoodHub.Validation;

// Custom Validation Data Annotation:
// رقم موبايل مصري = 11 رقم بيبدأ بـ 010 / 011 / 012 / 015
// Session 3: + IClientModelValidator → نفس القاعدة تشتغل في المتصفح (jQuery Validation)
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public partial class EgyptianPhoneAttribute : ValidationAttribute, IClientModelValidator
{
    // GeneratedRegex: الـ Regex بيتعمل وقت الـ Compile (أسرع من new Regex وقت التشغيل)
    [GeneratedRegex(@"^01[0125]\d{8}$")]
    private static partial Regex PhoneRegex();

    // {0} بيتبدل بالـ Display Name بتاع الحقل ("Phone number")
    public EgyptianPhoneAttribute()
        : base("{0} must be a valid Egyptian mobile number (e.g. 01012345678).") { }

    // ===== Server-side: بيشتغل وقت الـ Model Validation بعد الـ Model Binding =====
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // القيمة الفاضية مسؤولية [Required] — مش مسؤوليتنا
        if (value is not string phone || string.IsNullOrWhiteSpace(phone))
            return ValidationResult.Success;

        var digits = phone.Replace(" ", "").Replace("-", "");
        return PhoneRegex().IsMatch(digits)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName),
                                   [validationContext.MemberName!]);   // نربط الخطأ بالحقل نفسه
    }

    // ===== Client-side: بيشتغل وقت ما الـ Tag Helper بيرسم الـ <input asp-for="Phone"> =====
    // بيضيف data-val-egphone="..." على الـ input → الـ JS adapter في wwwroot/js/validation.js بيقراه
    public void AddValidation(ClientModelValidationContext context)
    {
        Merge(context.Attributes, "data-val", "true");                // فعّل الـ Unobtrusive Validation على الحقل ده
        Merge(context.Attributes, "data-val-egphone", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));  // اسم الـ rule + رسالة الخطأ
    }

    // بنضيف الـ attribute لو مش موجود بس (عشان مانكررش data-val لو [Required] حطه قبلنا)
    private static void Merge(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
            attributes.Add(key, value);
    }
}
