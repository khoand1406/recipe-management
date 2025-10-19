using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
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

        public async Task<(bool Success, string Message, int Traceid)> createRecipes(Recipe request, List<Ingredient> ingredients, List<Step> steps)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                request.CreatedAt = DateTime.Now;
                request.UpdatedAt= DateTime.Now;
                await _context.Recipes.AddAsync(request);
                await _context.SaveChangesAsync();

                foreach(var ingredient in ingredients)
                {
                    ingredient.RecipeId= request.RecipeId;
                }

                foreach (var step in steps)
                {
                    step.RecipeId= request.RecipeId;
                }
                await _context.Ingredients.AddRangeAsync(ingredients);
                await _context.Steps.AddRangeAsync(steps);
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
            using var transaction= await _context.Database.BeginTransactionAsync();
            try
            {
                var found= await _context.Recipes.FindAsync(id);
                if (found != null)
                {
                     _context.Recipes.Remove(found);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                return false;
            }catch (Exception ex)
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
            return await _context.Recipes.Where(x=> x.DishId==dishId).ToListAsync();
        }

        public Task<IEnumerable<Recipe>> getRecipesByFilter()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Recipe>> GetRecipesByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Recipe>> GetRelatedRecipes(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, string Message, int Traceid)> updateRecipes(Recipe request, List<Ingredient> ingredients, List<Step> steps)
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

