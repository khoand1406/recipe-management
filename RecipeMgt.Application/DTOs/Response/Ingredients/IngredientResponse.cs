namespace RecipeMgt.Application.DTOs.Response.Ingredients
{
    public class IngredientResponse
    {
        public int IngredientId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
    }

    public class CreateIngredientResponse
    {
        public bool Success { get; set; }
        public IngredientResponse? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateIngredientResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DeleteIngredientResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
