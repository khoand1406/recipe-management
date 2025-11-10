namespace RecipeMgt.Application.DTOs.Request.Steps
{
    public class CreateStepRequest
    {
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
    }

    public class UpdateStepRequest
    {
        public int StepId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
    }
}
