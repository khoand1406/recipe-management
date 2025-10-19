using FluentValidation;
using RecipeMgt.Application.DTOs.Request.Dishes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Api.Validator.Dish
{
    public class CreateDishRequestValidator: AbstractValidator<CreateDishRequest>
    {
        public CreateDishRequestValidator() {
            RuleFor(x=> x.DishName).NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.CategoryId).NotEqual(0).WithMessage("Category is required");
        }
    }
}
