using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecipeMgt.Domain.Entities;

public partial class Dish
{
    public int DishId { get; set; }

    public string DishName { get; set; }

    public string Description { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

    [NotMapped]
    public List<Image>? Images { get; set; }

    [JsonIgnore]
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<RelatedDish> RelatedDishes { get; set;}

    public virtual DishStatistic Statistic { get; set; }

}
