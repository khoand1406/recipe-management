using System;
using System.Collections.Generic;

namespace RecipeMgt.Domain.Entities;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public int DishId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int AuthorId { get; set; }

    public int? CookingTime { get; set; }

    public int? Servings { get; set; }

    public string DifficultyLevel { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Author { get; set; }

    public virtual Dish Dish { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<RelatedRecipe> RelatedRecipeRecipes { get; set; } = new List<RelatedRecipe>();

    public virtual ICollection<RelatedRecipe> RelatedRecipeRelatedRecipeNavigations { get; set; } = new List<RelatedRecipe>();

    public virtual ICollection<Step> Steps { get; set; } = new List<Step>();
}
