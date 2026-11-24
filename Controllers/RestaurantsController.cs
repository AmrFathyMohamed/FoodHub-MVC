using FoodHub.Data;
using FoodHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace FoodHub.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly AppDbContext _context;
        // داتا في الميموري لحد ما نوصّل EF Core — static عشان اللي بيتضاف من Create يفضل موجود
        //private static readonly List<Category> categories = new List<Category>()
        //{
        //    new Category(){Id=1, Name="Egyptian", Slug="egyptian", Icon="🥙"},
        //    new Category(){Id=2, Name="Pizza",    Slug="pizza",    Icon="🍕"},
        //    new Category(){Id=3, Name="Grill",    Slug="grill",    Icon="🍖"},
        //    new Category(){Id=4, Name="Desserts", Slug="desserts", Icon="🍰"},
        //};

        //private static readonly List<Restaurant> restaurants = new List<Restaurant>()
        //{
        //    new Restaurant(){Id=1, Name="Koshary El Tahrir", Slug="koshary-el-tahrir", CategoryId=1, Category=categories[0],
        //        Address="Gomhoreya St, Assiut", Phone="01012345678", DeliveryFee=15, MinOrder=50, DeliveryMinutes=25, IsOpen=true,
        //        MenuItems = { new MenuItem(){Id=1, RestaurantId=1, Name="Koshary Large", Price=45, IsPopular=true}, new MenuItem(){Id=2, RestaurantId=1, Name="Rice Pudding", Price=20} },
        //        Reviews = { new Review(){Id=1, RestaurantId=1, Rating=5, CustomerName="Mona"}, new Review(){Id=2, RestaurantId=1, Rating=4, CustomerName="Karim"} }},
        //    new Restaurant(){Id=2, Name="Pizza Corner", Slug="pizza-corner", CategoryId=2, Category=categories[1],
        //        Address="University St, Assiut", Phone="01112345678", DeliveryFee=20, MinOrder=100, DeliveryMinutes=35, IsOpen=true,
        //        MenuItems = { new MenuItem(){Id=3, RestaurantId=2, Name="Margherita", Price=120, IsPopular=true}, new MenuItem(){Id=4, RestaurantId=2, Name="Pepperoni", Price=150} },
        //        Reviews = { new Review(){Id=3, RestaurantId=2, Rating=4, CustomerName="Sara"} }},
        //    new Restaurant(){Id=3, Name="Grill House", Slug="grill-house", CategoryId=3, Category=categories[2],
        //        Address="Nile Corniche, Assiut", Phone="01212345678", DeliveryFee=25, MinOrder=150, DeliveryMinutes=40, IsOpen=false,
        //        MenuItems = { new MenuItem(){Id=5, RestaurantId=3, Name="Mixed Grill 1kg", Price=520, IsPopular=true} },
        //        Reviews = { new Review(){Id=4, RestaurantId=3, Rating=5, CustomerName="Omar"}, new Review(){Id=5, RestaurantId=3, Rating=4, CustomerName="Nada"} }},
        //    new Restaurant(){Id=4, Name="Sweet Tooth", Slug="sweet-tooth", CategoryId=4, Category=categories[3],
        //        Address="Gamal Abdel Nasser St, Assiut", Phone="01022223333", DeliveryFee=10, MinOrder=40, DeliveryMinutes=20, IsOpen=true,
        //        MenuItems = { new MenuItem(){Id=6, RestaurantId=4, Name="Konafa with Cream", Price=70, IsPopular=true} },
        //        Reviews = { new Review(){Id=6, RestaurantId=4, Rating=5, CustomerName="Youssef"} }},
        //};

        public RestaurantsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var Restaurants = await _context.Restaurants.Include(x => x.Reviews)
                .Include(x => x.MenuItems)
                .Include(x => x.Category)
                
                .ToListAsync();
            Restaurants = Restaurants.OrderByDescending(x => x.AverageRating).ToList();
            return View(Restaurants);
        }

        public async Task<IActionResult> Details(int id)
        {
            var Restaurant = await _context.Restaurants
                .Include(x=>x.Reviews)
                .Include(x=>x.MenuItems)
                .Include(x=>x.Category)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (Restaurant is null)
                return NotFound();      // 404

            return View(Restaurant);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Restaurant restaurant)
        {
            //// Model Binding (+ FlexibleDecimalModelBinder) ثم Data Annotations (+ EgyptianPhone)
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Categories = categories;
            //    return View(restaurant);   // نفس الفورم يرجع برسايل الأخطاء
            //}

            //restaurant.Id = restaurants.Max(x => x.Id) + 1;
            //restaurant.Category = categories.FirstOrDefault(c => c.Id == restaurant.CategoryId);
            //restaurants.Add(restaurant);

            return RedirectToAction(nameof(Details), new { id = restaurant.Id });
        }
    }
}
