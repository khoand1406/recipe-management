using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Entities
{
    public class DishStatistic
    {
        public int DishId { get; set; }

        public int ViewCount { get; set; }
        public int BookmarkCount { get; set; }
        public int RecipeCount { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        // Navigation
        public Dish Dish { get; set; }
    }

}
