namespace RecipeMgt.Application.DTOs.Request.Ingredients
{
    public class CreateIngredientRequest
    {
        public int RecipeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
    }

    public class UpdateIngredientRequest
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
    }
}
