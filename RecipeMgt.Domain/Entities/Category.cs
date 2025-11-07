using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeMgt.Domain.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    [JsonIgnore]
    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
