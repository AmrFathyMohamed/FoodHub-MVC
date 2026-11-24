using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodHub.Binders
{
    // Custom Model Binder:
    // الـ Admin ممكن يكتب السعر "١٢٠" أو "120 EGP" أو "١٢٠٫٥ ج.م"
    // الـ Binder الافتراضي هيرفضهم — إحنا بنحوّلهم لـ decimal عادي
    public class FlexibleDecimalModelBinder : IModelBinder
    {
        private static readonly string[] Suffixes = ["EGP", "LE", "ج.م", "جنيه"];

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == ValueProviderResult.None)
                return Task.CompletedTask;               // مفيش قيمة خالص في الـ request

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
            var raw = valueResult.FirstValue;

            if (string.IsNullOrWhiteSpace(raw))
            {
                if (bindingContext.ModelMetadata.IsReferenceOrNullableType)
                    bindingContext.Result = ModelBindingResult.Success(null);   // decimal? فاضي = null
                else
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                        $"{bindingContext.ModelMetadata.GetDisplayName()} is required.");
                return Task.CompletedTask;
            }

            if (decimal.TryParse(Normalize(raw), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                bindingContext.Result = ModelBindingResult.Success(value);
            else
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"'{raw}' is not a valid amount.");

            return Task.CompletedTask;
        }

        public static string Normalize(string raw)
        {
            var text = raw.Trim();
            foreach (var s in Suffixes)
                text = text.Replace(s, "", StringComparison.OrdinalIgnoreCase);

            var sb = new StringBuilder(text.Length);
            foreach (var ch in text)
            {
                if (ch is >= '٠' and <= '٩') sb.Append((char)('0' + (ch - '٠')));       // ٠-٩
                else if (ch is >= '۰' and <= '۹') sb.Append((char)('0' + (ch - '۰')));  // ۰-۹
                else if (ch == '٫') sb.Append('.');                                      // ٫ فاصلة عشرية عربي
                else if (ch is >= '0' and <= '9' or '.' or '-') sb.Append(ch);
                // , و ٬ (فاصل الآلاف) والمسافات بنتجاهلهم: "1,500" = 1500
            }
            return sb.ToString();
        }
    }
}
