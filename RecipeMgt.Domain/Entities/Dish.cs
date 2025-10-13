using System;
using System.Collections.Generic;

namespace RecipeMgt.Domain.Entities;

public partial class Dish
{
    public int DishId { get; set; }

    public string DishName { get; set; }

    public string Description { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
