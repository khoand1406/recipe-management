using System;
using System.Collections.Generic;

namespace RecipeMgt.Domain.Entities;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public int RecipeId { get; set; }

    public string Name { get; set; }

    public string Quantity { get; set; }

    public virtual Recipe Recipe { get; set; }
}
