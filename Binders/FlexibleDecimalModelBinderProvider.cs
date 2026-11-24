using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodHub.Binders
{
    // الـ Provider بيقول للـ MVC: "أي property نوعها decimal أو decimal? → استخدم الـ Binder بتاعي"
    public class FlexibleDecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var type = context.Metadata.UnderlyingOrModelType;
            return type == typeof(decimal) ? new FlexibleDecimalModelBinder() : null;
        }
    }
}
