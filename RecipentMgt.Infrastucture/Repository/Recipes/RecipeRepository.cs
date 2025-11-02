using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.RequestEntity;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Recipes
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeManagementContext _context;
        private readonly ILogger<RecipeRepository> _logger;

        public RecipeRepository(RecipeManagementContext context, ILogger<RecipeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, int Traceid)> createRecipes(Recipe request, List<Ingredient> ingredients, List<Step> steps, List<Image> images)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                request.CreatedAt = DateTime.Now;
                request.UpdatedAt = DateTime.Now;
                await _context.Recipes.AddAsync(request);
                await _context.SaveChangesAsync();

                foreach (var ingredient in ingredients)
                {
                    ingredient.RecipeId = request.RecipeId;
                }

                foreach (var step in steps)
                {
                    step.RecipeId = request.RecipeId;
                }

                foreach (var image in images)
                {
                    image.EntityType = "Recipe";
                    image.EntityId = request.RecipeId;
                    
                    image.UploadedAt = DateTime.Now;
                }
                await _context.Ingredients.AddRangeAsync(ingredients);
                await _context.Steps.AddRangeAsync(steps);
                await _context.Images.AddRangeAsync(images);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation($"Successfully created recipe with id {request.RecipeId}");
                return (true, "Create new recipe successfully", request.RecipeId);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error occurs while creating recipe: {ex.Message}");
                return (false, ex.Message, 0);

            }
        }

        public async Task<bool> deleteRecipes(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var found = await _context.Recipes.FindAsync(id);
                if (found != null)
                {
                    _context.Recipes.Remove(found);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error occurs while creating recipe: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Recipe>> getAll()
        {
            return await _context.Recipes.ToListAsync();
        }

        public async Task<Recipe?> getRecipeById(int id)
        {
            return await _context.Recipes.FindAsync(id);
        }

        public async Task<IEnumerable<Recipe>> GetRecipes(int dishId)
        {
            return await _context.Recipes.Where(x => x.DishId == dishId).ToListAsync();
        }

        public Task<IEnumerable<Recipe>> getRecipesByFilter()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByUser(int userId)
        {
            return await _context.Recipes
                                        .Where(x => x.AuthorId == userId).Include(x=> x.Author)
                                        .ToListAsync();
        }

        public Task<IEnumerable<Recipe>> GetRelatedRecipes(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResponse<Recipe>> GetSearchedResult(SearchRecipeRequest request)
        {
            var query = _context.Recipes.Include(r=> r.Author).Include(r=> r.Ingredients).AsQueryable();
            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(r => r.Title.Contains(request.Title));

            if (!string.IsNullOrEmpty(request.Difficulty))
                query = query.Where(r => r.DifficultyLevel == request.Difficulty);

            if (request.MaxCookingTime.HasValue)
                query = query.Where(r => r.CookingTime <= request.MaxCookingTime.Value);

            if (!string.IsNullOrEmpty(request.CreatorName))
                query = query.Where(r => r.Author.FullName.Contains(request.CreatorName));

            if (!string.IsNullOrEmpty(request.Ingredient))
                query = query.Where(r => r.Ingredients.Any(i => i.Name.Contains(request.Ingredient)));

            var totalCounts = await query.CountAsync();

            query = request.SortBy?.ToLower() switch
            {
                "title" => request.SortOrder == "asc" ? query.OrderBy(r => r.Title) : query.OrderByDescending(r => r.Title),
                "cookingtime" => request.SortOrder == "asc" ? query.OrderBy(r => r.CookingTime) : query.OrderByDescending(r => r.CookingTime),
                "difficulty" => request.SortOrder == "asc" ? query.OrderBy(r => r.DifficultyLevel) : query.OrderByDescending(r => r.DifficultyLevel),
                "creator" => request.SortOrder == "asc" ? query.OrderBy(r => r.Author.FullName) : query.OrderByDescending(r => r.Author.FullName),
                _ => request.SortOrder == "asc" ? query.OrderBy(r => r.RecipeId) : query.OrderByDescending(r => r.RecipeId)
            };

            var skip = (request.Page - 1) * request.PageSize;
            var items = await query.Skip(skip).Take(request.PageSize).ToListAsync();

            return new PagedResponse<Recipe>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCounts,
                TotalPages = (int)Math.Ceiling(totalCounts / (double)request.PageSize)
            };
        }

        public async Task<(bool Success, string Message, int Traceid)> updateRecipes(Recipe request, List<Ingredient> ingredients, List<Step> steps, List<Image> images)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                request.UpdatedAt = DateTime.Now;
                _context.Recipes.Update(request);
                await _context.SaveChangesAsync();

                var existingIngredients = await _context.Ingredients
           .Where(i => i.RecipeId == request.RecipeId)
           .ToListAsync();

                var toDeleteIngredients = existingIngredients
                    .Where(ei => !ingredients.Any(i => i.IngredientId == ei.IngredientId))
                    .ToList();

                _context.Ingredients.RemoveRange(toDeleteIngredients);


                foreach (var ingredient in ingredients)
                {
                    ingredient.RecipeId = request.RecipeId;
                    if (ingredient.IngredientId == 0)
                        await _context.Ingredients.AddAsync(ingredient);
                    else
                        _context.Ingredients.Update(ingredient);
                }

                var existingSteps = await _context.Steps
                    .Where(s => s.RecipeId == request.RecipeId)
                    .ToListAsync();

                var toDeleteSteps = existingSteps
                    .Where(es => !steps.Any(s => s.StepId == es.StepId))
                    .ToList();

                _context.Steps.RemoveRange(toDeleteSteps);

                foreach (var step in steps)
                {
                    step.RecipeId = request.RecipeId;
                    if (step.StepId == 0)
                        await _context.Steps.AddAsync(step);
                    else
                        _context.Steps.Update(step);
                }

                var existingImages = await _context.Images.Where(s => s.EntityId == request.RecipeId && s.EntityType.Contains("Recipe")).ToListAsync();

                _context.Images.RemoveRange(existingImages);

                foreach (var image in images)
                {
                    image.EntityId = request.RecipeId;
                    image.EntityType = "Recipe";
                    image.UploadedAt = DateTime.Now;
                    await _context.Images.AddAsync(image);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Update successfully", request.RecipeId);
            }


            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating recipe {RecipeId}", request.RecipeId);
                return (false, ex.Message, 0);
            }
        }
    }
}

