using System.ComponentModel.DataAnnotations;

namespace FoodHub.Models
{
    // نوع المطبخ: Egyptian / Pizza / Grill ...
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(60)]
        public string Slug { get; set; } = string.Empty;   // للـ URL: /cuisine/pizza (Session 4)

        [StringLength(8)]
        public string Icon { get; set; } = "🍽️";

        public List<Restaurant> Restaurants { get; set; } = [];
    }
}
