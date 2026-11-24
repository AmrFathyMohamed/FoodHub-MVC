using FoodHub.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Data;

// داتا مبدئية بتنزل مع الـ Migration (HasData)
// ⚠️ كل القيم لازم تكون ثابتة (حتى التواريخ) — وإلا EF هيشوف "تغيير" في كل Migration
public static class SeedData
{
    private static readonly DateTime Seeded = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder b)
    {
        b.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Egyptian", Slug = "egyptian", Icon = "🥙" },
            new Category { Id = 2, Name = "Pizza",    Slug = "pizza",    Icon = "🍕" },
            new Category { Id = 3, Name = "Grill",    Slug = "grill",    Icon = "🍖" },
            new Category { Id = 4, Name = "Burgers",  Slug = "burgers",  Icon = "🍔" },
            new Category { Id = 5, Name = "Desserts", Slug = "desserts", Icon = "🍰" },
            new Category { Id = 6, Name = "Seafood",  Slug = "seafood",  Icon = "🦐" });

        b.Entity<Restaurant>().HasData(
            R(1, "Koshary El Tahrir", "koshary-el-tahrir", 1, "The classic koshary everyone loves.", "Gomhoreya St, Assiut", "01012345678", 15, 50, 25, true),
            R(2, "Pizza Corner",      "pizza-corner",      2, "Stone-baked pizza with fresh dough.", "University St, Assiut", "01112345678", 20, 100, 35, true),
            R(3, "Grill House",       "grill-house",       3, "Kofta, kebab and mixed grills.",     "Nile Corniche, Assiut", "01212345678", 25, 150, 40, false),
            R(4, "Burger Lab",        "burger-lab",        4, "Smash burgers and loaded fries.",    "Al Hamraa, Assiut", "01512345678", 20, 80, 30, true),
            R(5, "Sweet Tooth",       "sweet-tooth",       5, "Konafa, basbousa and cakes.",        "Gamal Abdel Nasser St, Assiut", "01022223333", 10, 40, 20, true),
            R(6, "Sea Breeze",        "sea-breeze",        6, "Fresh fish and shrimp daily.",       "Nile Corniche, Assiut", "01133334444", 30, 200, 45, true),
            R(7, "Abo Tarek Foul",    "abo-tarek-foul",    1, "Foul, falafel and Egyptian breakfast.", "Station Sq, Assiut", "01244445555", 10, 30, 15, true),
            R(8, "Napoli Express",    "napoli-express",    2, "Italian pizza and pasta.",           "Al Walideya, Assiut", "01055556666", 25, 120, 35, true));

        var id = 0;
        MenuItem M(int restaurant, string name, decimal price, string? desc = null, bool popular = false) =>
            new() { Id = ++id, RestaurantId = restaurant, Name = name, Price = price, Description = desc, IsPopular = popular, IsAvailable = true };

        b.Entity<MenuItem>().HasData(
            M(1, "Koshary Large", 45, "Rice, pasta, lentils, crispy onions", true), M(1, "Koshary Medium", 35), M(1, "Rice Pudding", 20),
            M(2, "Margherita", 120, "Tomato, mozzarella, basil", true), M(2, "Pepperoni", 150), M(2, "Chicken Ranch", 165),
            M(3, "Mixed Grill 1kg", 520, "Kofta, kebab, shish tawook", true), M(3, "Kofta 1/2kg", 260), M(3, "Grilled Chicken", 210),
            M(4, "Classic Smash", 140, "Double patty, cheddar, pickles", true), M(4, "Mushroom Swiss", 165), M(4, "Loaded Fries", 75),
            M(5, "Konafa with Cream", 70, popular: true), M(5, "Basbousa", 40), M(5, "Molten Cake", 85),
            M(6, "Grilled Sea Bream", 280, popular: true), M(6, "Shrimp Tagine", 320), M(6, "Calamari", 240),
            M(7, "Foul Sandwich", 10, popular: true), M(7, "Falafel Sandwich", 10), M(7, "Breakfast Box", 60),
            M(8, "Quattro Formaggi", 185, popular: true), M(8, "Penne Arrabbiata", 130), M(8, "Tiramisu", 90));

        b.Entity<Review>().HasData(
            new Review { Id = 1, RestaurantId = 1, Rating = 5, CustomerName = "Mona",  Comment = "Best koshary in Assiut!", CreatedAt = Seeded },
            new Review { Id = 2, RestaurantId = 1, Rating = 4, CustomerName = "Karim", Comment = "Great taste, a bit slow.", CreatedAt = Seeded },
            new Review { Id = 3, RestaurantId = 2, Rating = 4, CustomerName = "Sara",  Comment = "Crispy crust 👌", CreatedAt = Seeded },
            new Review { Id = 4, RestaurantId = 3, Rating = 5, CustomerName = "Omar",  Comment = "The mixed grill is huge.", CreatedAt = Seeded },
            new Review { Id = 5, RestaurantId = 4, Rating = 3, CustomerName = "Nada",  Comment = "Good burger, cold fries.", CreatedAt = Seeded },
            new Review { Id = 6, RestaurantId = 5, Rating = 5, CustomerName = "Youssef", Comment = "Konafa ❤️", CreatedAt = Seeded });
    }

    private static Restaurant R(int id, string name, string slug, int cat, string desc, string address,
                                string phone, decimal fee, decimal min, int minutes, bool open) =>
        new()
        {
            Id = id, Name = name, Slug = slug, CategoryId = cat, Description = desc, Address = address,
            Phone = phone, DeliveryFee = fee, MinOrder = min, DeliveryMinutes = minutes, IsOpen = open,
            CreatedAt = Seeded
        };
}
