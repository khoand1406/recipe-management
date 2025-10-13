using System;
using System.Collections.Generic;

namespace RecipeMgt.Domain.Entities;

public partial class RelatedRecipe
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public int RelatedRecipeId { get; set; }

    public virtual Recipe Recipe { get; set; }

    public virtual Recipe RelatedRecipeNavigation { get; set; }
}
