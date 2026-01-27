using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.Constant;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.Services.Cloudiary;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Repository.Recipes;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Recipes
{
    public class RecipeServices : IRecipeServices
    {


        private readonly IUnitOfWork _uow;

        private readonly IMapper _mapper;
        private readonly ILogger<RecipeServices> _logger;

        public RecipeServices(IMapper mapper, ILogger<RecipeServices> logger, IUnitOfWork uow)
        {

            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        public async Task<RecipeResponse?> GetRecipeById(int id)
        {
            var recipe = await _uow.Recipes.getRecipeById(id);
            if (recipe == null) return null;
            var result = _mapper.Map<RecipeResponse>(recipe);
            return result;
        }

        public async Task<Result<RecipeResponse>> CreateRecipeAsync(CreateRecipeRequest request)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var dish = await _uow.Dishes.GetById(request.DishId);
                if (dish == null)
                {
                    _logger.LogError($"Error occurs when create recipe: Dish with id {request.DishId} not found");
                    return Result<RecipeResponse>.Failure(RecipeErrorMessage.DishNotFound);
                }
                var recipe = _mapper.Map<Recipe>(request);
                recipe.CreatedAt = DateTime.Now;
                recipe.UpdatedAt = DateTime.Now;
                await _uow.Recipes.AddAsync(recipe);
                await _uow.SaveChangesAsync();

                var ingredients = request.Ingredients.Select(i =>
                {
                    var ing = _mapper.Map<Ingredient>(i);
                    ing.RecipeId = recipe.RecipeId;
                    return ing;
                }).ToList();

                var steps = request.Steps.Select(i =>
                {
                    var step = _mapper.Map<Step>(i);
                    step.RecipeId = recipe.RecipeId;
                    return step;
                }).ToList();

                var images = request.ImageUrls != null && request.ImageUrls.Any() ? request.ImageUrls.Select(url => new Image
                {
                    Caption = url,
                    EntityId = recipe.RecipeId,
                    EntityType = "Recipe",
                    ImageUrl = url,
                    UploadedAt = DateTime.Now,
                }).ToList() : null;

                if (images != null)
                {
                    await _uow.Recipes.AddRangeAsync(images);
                }
                await _uow.Recipes.AddRangeAsync(steps);
                await _uow.Recipes.AddRangeAsync(ingredients);
                await _uow.CommitAsync();
               
                var response = _mapper.Map<RecipeResponse>(recipe);
                response.Images = request.ImageUrls ?? [];

                return Result<RecipeResponse>.Success(response);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "CreateRecipe failed");

                return Result<RecipeResponse>.Failure(RecipeErrorMessage.CreateFailed);
            }
        }


        public async Task<Result> DeleteRecipe(int id)
        {
            try
            {
                var found = await _uow.Recipes.getRecipeById(id);
                if (found == null)
                {
                    _logger.LogError(message: $"Not found recipe with id:{id}");
                    return Result.Failure(RecipeErrorMessage.NotFound);
                }
                var result = await _uow.Recipes.deleteRecipes(id);
                if (result)
                {
                    return Result.Success();
                }
                return Result.Failure(RecipeErrorMessage.DeleteFailed);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete recipe: {id}, {ex.Message}");
                 return Result.Failure(RecipeErrorMessage.DeleteFailed);
            }
        }

        public async Task<IEnumerable<RecipeResponse>> GetRecipesByDish(int id)
        {
            var listRecipes = await _uow.Recipes.GetRecipes(id);
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeResponse>> GetRecipesByFilter()
        {
            var listRecipes = await _uow.Recipes.getRecipesByFilter();
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeWithUserInfo>> GetRecipesByUser(int userId)
        {
            var listRecipes = await _uow.Recipes.GetRecipesByUser(userId);
            var result = _mapper.Map<IEnumerable<RecipeWithUserInfo>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeResponse>> GetRelatedRecipes(int id)
        {
            var listRecipes = await _uow.Recipes.GetRecipesByUser(id);
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<Domain.RequestEntity.PagedResponse<RecipeResponse>> GetSearchResult(SearchRecipeRequest request)
        {
            var pagedResponse=  await _uow.Recipes.GetSearchedResult(request.Title, request.Ingredient, request.Difficulty, request.MaxCookingTime, 
                            request.CreatorName, request.Page, request.PageSize, request.SortBy, request.SortOrder);
            var result= _mapper.Map<Domain.RequestEntity.PagedResponse<RecipeResponse>>(pagedResponse);
            return result;
        }

        public async Task<Result> UpdateRecipeAsync(UpdateRecipeRequest request, int currentUserId)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var recipe = await _uow.Recipes.getRecipeById(request.RecipeId);
                if (recipe == null)
                {
                    return Result<RecipeResponse>.Failure(RecipeErrorMessage.NotFound);
                }
                if (recipe.AuthorId != currentUserId)
                    return Result.Failure(RecipeErrorMessage.Forbidden);

                recipe.Title = request.Title;
                recipe.Description = request.Description;
                recipe.CookingTime = request.CookingTime;
                recipe.DifficultyLevel = request.DifficultyLevel;
                recipe.Servings = request.Servings;
                recipe.UpdatedAt = DateTime.Now;
                _uow.Recipes.Update(recipe);

                var newIncomingIngredients = request.Ingredients.Select(item => _mapper.Map<Ingredient>(item)).ToList();
                foreach (var item in newIncomingIngredients)
                {
                    item.RecipeId = recipe.RecipeId;
                }
                var toDeleteIngredients = recipe.Ingredients
                    .Where(db => !newIncomingIngredients.Any(i => i.IngredientId == db.IngredientId))
                    .ToList();

                var toAddIngredients = newIncomingIngredients
                    .Where(i => i.IngredientId == 0)
                    .ToList();

                var toUpdateIngredients = newIncomingIngredients
                    .Where(i => i.IngredientId != 0)
                    .ToList();
                _uow.Ingredients.RemoveRange(toDeleteIngredients);
                await _uow.Ingredients.AddRangeAsync(toAddIngredients);
                _uow.Ingredients.UpdateRange(toUpdateIngredients);

                var incomingSteps = request.Steps
            .Select(s => _mapper.Map<Step>(s))
            .ToList();

                foreach (var step in incomingSteps)
                    step.RecipeId = recipe.RecipeId;

                var toDeleteSteps = recipe.Steps
                    .Where(db => !incomingSteps.Any(s => s.StepId == db.StepId))
                    .ToList();

                var toAddSteps = incomingSteps
                    .Where(s => s.StepId == 0)
                    .ToList();

                var toUpdateSteps = incomingSteps
                    .Where(s => s.StepId != 0)
                    .ToList();
                _uow.Steps.RemoveRange(toDeleteSteps);
                await _uow.Steps.AddRangeAsync(toAddSteps);
                _uow.Steps.UpdateRangeAsync(toUpdateSteps);

                var oldImages = await _uow.Recipes.getRecipeImages(recipe.RecipeId);
                _uow.Recipes.RemoveRange(oldImages);
                var newImages = request.ImageUrls != null && request.ImageUrls.Count == 0 ? request.ImageUrls.Select(item => new Image
                {
                    EntityType = "Recipe",
                    EntityId = recipe.RecipeId,
                    ImageUrl = item,
                    Caption = Path.GetFileName(item),
                    UploadedAt = DateTime.Now
                }).ToList() : null;
                if (newImages != null) await _uow.Recipes.AddRangeAsync(newImages);
                await _uow.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "UpdateRecipe failed: {RecipeId}", request.RecipeId);
                return Result.Failure(RecipeErrorMessage.UpdateFailed);
            }
        }
    }
}

