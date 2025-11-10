namespace RecipeMgt.Views.Models.Response
{
    public class StepResponse
    {
        public int StepId { get; set; }
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
    }

    public class CreateStepResponse
    {
        public bool Success { get; set; }
        public StepResponse? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateStepResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DeleteStepResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
