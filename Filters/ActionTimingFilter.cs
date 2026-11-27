using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodHub.Filters;

// Session 3: Action Filter = كود بيتنفّذ قبل وبعد أي Action من غير ما نكرره في كل Controller
// ترتيب الـ Filters: Authorization → Resource → [Model Binding] → Action → Exception → Result
// IAsyncActionFilter: method واحدة فيها "قبل" و "بعد" مع بعض (بدل OnActionExecuting + OnActionExecuted)
public class ActionTimingFilter : IAsyncActionFilter
{
    private readonly ILogger<ActionTimingFilter> _logger;

    // الـ Filter بيتعمل من الـ DI → ينفع نحقن فيه أي Service (هنا الـ Logger)
    public ActionTimingFilter(ILogger<ActionTimingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // ===== قبل الـ Action (OnActionExecuting) =====
        var sw = Stopwatch.StartNew();

        var executed = await next();          // ← هنا الـ Action نفسه بيتنفّذ

        // ===== بعد الـ Action (OnActionExecuted) =====
        sw.Stop();
        var action = context.ActionDescriptor.DisplayName;   // مثلاً: FoodHub.Controllers.RestaurantsController.Index (FoodHub)

        // Header في الـ Response → شوفه من F12 → Network → Headers
        context.HttpContext.Response.Headers["X-Action-Ms"] = sw.ElapsedMilliseconds.ToString();

        // Structured logging: {Action} و {Ms} بيتسجلوا كـ fields مش نص بس
        if (executed.Exception is not null && !executed.ExceptionHandled)
            _logger.LogWarning("{Action} threw after {Ms} ms", action, sw.ElapsedMilliseconds);
        else if (sw.ElapsedMilliseconds > 500)
            _logger.LogWarning("SLOW action {Action} took {Ms} ms", action, sw.ElapsedMilliseconds);
        else
            _logger.LogInformation("{Action} took {Ms} ms", action, sw.ElapsedMilliseconds);
    }
}
