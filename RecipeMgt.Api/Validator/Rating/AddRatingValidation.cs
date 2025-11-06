using FluentValidation;
using RecipeMgt.Application.DTOs.Request.Rating;

namespace RecipeMgt.Api.Validator.Rating
{
    public class AddRatingValidation : AbstractValidator<AddRatingRequest>
    {
        public AddRatingValidation() {
            RuleFor(x => x.RecipeId)
            .GreaterThan(0).WithMessage("RecipeId must be greater than 0.");

            RuleFor(x => x.Score)
                .InclusiveBetween(1, 5)
                .WithMessage("Score must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(500)
                .WithMessage("Comment must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Comment));
        }
    }
}
