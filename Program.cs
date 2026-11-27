using FoodHub.Binders;
using FoodHub.Data;
using FoodHub.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Session 2: الـ Custom Model Binder في أول القائمة عشان ياخد الأولوية
    options.ModelBinderProviders.Insert(0, new FlexibleDecimalModelBinderProvider());

    // Session 3: Global Action Filter — بيشتغل على كل Action في المشروع
    // Add<T>() → الـ Filter بيتعمل من الـ DI (عشان الـ ILogger يتحقن فيه)
    // البدائل: [ServiceFilter(typeof(ActionTimingFilter))] على Controller/Action معين
    //          أو نورّث من ActionFilterAttribute ونحطه [MyFilter]
    options.Filters.Add<ActionTimingFilter>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
