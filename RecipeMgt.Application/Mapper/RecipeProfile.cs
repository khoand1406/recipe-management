using AutoMapper;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Mapper
{
    public class RecipeProfile: Profile
    {
        public RecipeProfile() {
            CreateMap<Recipe, RecipeResponse>();
            CreateMap<Recipe, RecipeWithUserInfo>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));

            CreateMap<CreateRecipeRequest, Recipe>();
            CreateMap<UpdateRecipeRequest, Recipe>();
            CreateMap<IngredientDto, Ingredient>();
            CreateMap<StepDto, Step>();
        }
    }
}
