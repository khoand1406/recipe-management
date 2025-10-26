using AutoMapper;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Repository.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Recipes
{
    public class RecipeServices : IRecipeServices
    {
        private readonly IRecipeRepository _repository;
        private readonly IDishRepository _dishrepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RecipeServices> _logger;

        public RecipeServices(IRecipeRepository repository, IMapper mapper, ILogger<RecipeServices> logger, IDishRepository dishRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _dishrepository = dishRepository;
        }

        public async Task<CreateRecipeResponse> CreateRecipe(CreateRecipeRequest request)
        {
            try
            {
                var dish = await _dishrepository.GetById(request.DishId);
                if (dish == null)
                {
                    _logger.LogError($"Error occurs when create recipe: Dish with id {request.DishId} not found");
                    return new CreateRecipeResponse { Success = false, Message = "Create Recipe Failed! Not found dish with Id: " + request.DishId };
                }
                var recipe = _mapper.Map<Recipe>(request);
                recipe.CreatedAt = DateTime.Now;
                recipe.UpdatedAt = DateTime.Now;


                var ingredients = request.Ingredients != null && request.Ingredients.Any()
                    ? _mapper.Map<List<Ingredient>>(request.Ingredients)
                    : new List<Ingredient>();

                var steps = request.Steps != null && request.Steps.Any()
                    ? _mapper.Map<List<Step>>(request.Steps)
                    : new List<Step>();


                var result = await _repository.createRecipes(recipe, ingredients, steps);

                return new CreateRecipeResponse 
                { 
                    Success = true, 
                    Message = result.Message, 
                    Data = new RecipeResponse { 
                        RecipeId = result.Traceid, 
                        Title = recipe.Title, 
                        AuthorId = recipe.AuthorId, 
                        CookingTime = recipe.CookingTime, 
                        Description = recipe.Description, 
                        DifficultyLevel = recipe.DifficultyLevel, 
                        Servings = recipe.Servings, 
                        CreatedAt = recipe.CreatedAt, 
                        UpdatedAt = recipe.UpdatedAt 
                    } 
                };


            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurs when creating recipe: " + ex.Message);
                return new CreateRecipeResponse { Success = false, Message = "Create Recipe Failed" };
            }
        }

        public async Task<DeleteRecipeResponse> DeleteRecipe(int id)
        {
            try
            {
                var found= await _repository.getRecipeById(id);
                if(found == null)
                {
                    _logger.LogError("Not found recipe with id:" + id);
                    return new DeleteRecipeResponse { Success = false, Message = "Not found recipe with id: " + id };
                }
                var result= await _repository.deleteRecipes(id);
                if (result)
                {
                    return new DeleteRecipeResponse { Success = true, Message = "Delete successfully" };
                }
                return new DeleteRecipeResponse { Success = false, Message = "Delete Failed" };

            }catch(Exception ex)
            {
                _logger.LogError($"Failed to delete recipe: {id}, {ex.Message}");
                return new DeleteRecipeResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<IEnumerable<RecipeResponse>> GetRecipesByDish(int id)
        {
            var listRecipes= await  _repository.GetRecipes(id);
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeResponse>> GetRecipesByFilter()
        {
            var listRecipes = await _repository.getRecipesByFilter();
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeWithUserInfo>> GetRecipesByUser(int userId)
        {
            var listRecipes = await _repository.GetRecipesByUser(userId);
            var result = _mapper.Map<IEnumerable<RecipeWithUserInfo>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeResponse>> GetRelatedRecipes(int id)
        {
            var listRecipes = await _repository.GetRecipesByUser(id);
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<UpdateRecipeResponse> UpdateRecipe(UpdateRecipeRequest request)
        {
            try
            {
                var dish = await _dishrepository.GetById(request.DishId);
                if (dish == null)
                {
                    _logger.LogError($"Error occurs when create recipe: Dish with id {request.DishId} not found");
                    return new UpdateRecipeResponse { Success = false, Message = "Create Recipe Failed! Not found dish with Id: " + request.DishId };
                }
                var recipeUpdate = _mapper.Map<Recipe>(request);
                recipeUpdate.UpdatedAt = DateTime.Now;
                var ingredients = request.Ingredients != null && request.Ingredients.Any()
                    ? _mapper.Map<List<Ingredient>>(request.Ingredients)
                    : new List<Ingredient>();

                var steps = request.Steps != null && request.Steps.Any()
                    ? _mapper.Map<List<Step>>(request.Steps)
                    : new List<Step>();
                var result= await _repository.updateRecipes(recipeUpdate, ingredients, steps);
                if (result.Success)
                {
                    _logger.LogInformation("Successfully update recipe with id: " + request.RecipeId);
                    return new UpdateRecipeResponse { Success = true, Message = result.Message };

                }
                return new UpdateRecipeResponse { Success = false, };
            }
            catch(Exception ex)
            {
                _logger.LogError("Error while updating recipes: "+ ex.Message);
                return new UpdateRecipeResponse { Success = false, Message= "Update Recipe Failed!" };
            }
        }
    }
}
