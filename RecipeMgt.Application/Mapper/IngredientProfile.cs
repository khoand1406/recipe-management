using AutoMapper;
using RecipeMgt.Application.DTOs.Request.Ingredients;
using RecipeMgt.Application.DTOs.Response.Ingredients;
using RecipeMgt.Domain.Entities;

namespace RecipeMgt.Application.Mapper
{
    public class IngredientProfile : Profile
    {
        public IngredientProfile()
        {
            CreateMap<Ingredient, IngredientResponse>();
            CreateMap<CreateIngredientRequest, Ingredient>();
            CreateMap<UpdateIngredientRequest, Ingredient>();
        }
    }
}
