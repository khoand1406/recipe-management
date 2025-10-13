using System;
using System.Collections.Generic;

namespace RecipeMgt.Domain.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
