using RecipeMgt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Entities
{
    public class RelatedDish
    {
        public int Id { get; set; }

        public int DishId { get; set; }
        public int RelatedDishId { get; set; }

        public DishRelationType RelationType { get; set; }
        public int Priority { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public Dish Dish { get; set; }
        public Dish RelateDish { get; set; }
    }
}
