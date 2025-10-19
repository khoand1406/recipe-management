using FluentValidation;
using RecipeMgt.Application.DTOs.Request.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Api.Validator.Recipe
{
    public class UpdateRecipeValidator:AbstractValidator<UpdateRecipeRequest>
    {
        public UpdateRecipeValidator() {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Recipe Title is required");

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.CookingTime).InclusiveBetween(1, 300).WithMessage("Cooking time must be in 1 to 300 minutes");

            RuleFor(x => x.Ingredients)
            .NotNull().WithMessage("Ingredients list cannot be null.")
            .Must(x => x.Count > 0).WithMessage("At least one ingredient is required.");

            RuleForEach(x => x.Ingredients).ChildRules(i =>
            {
                i.RuleFor(y => y.Name).NotEmpty().WithMessage("Ingredients can' be null");
                i.RuleFor(y => y.Quantity).NotEmpty().WithMessage("Ingredient quantity must be not empty.");

            });

            RuleFor(x => x.Steps)
                .NotNull().WithMessage("Steps list cannot be null.")
                .Must(x => x.Count > 0).WithMessage("At least one step is required.");

            RuleForEach(x => x.Steps).ChildRules(s =>
            {
                s.RuleFor(y => y.Instruction).NotEmpty().WithMessage("Step instruction is required.");
            });
        }
    }
}
