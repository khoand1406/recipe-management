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

        public async Task AddAsync(Recipe recipe)
        => await _context.Recipes.AddAsync(recipe);

        public async Task AddRangeAsync(List<Ingredient> ingredients)
    => await _context.Ingredients.AddRangeAsync(ingredients);

        public async Task AddRangeAsync(List<Step> steps)
            => await _context.Steps.AddRangeAsync(steps);

        public async Task AddRangeAsync(List<Image> images)
            => await _context.Images.AddRangeAsync(images);

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
            return await _context.Recipes
        .Include(r => r.Ingredients)
        .Include(r => r.Steps)
        .FirstOrDefaultAsync(r => r.RecipeId == id);
        }
    
        public async Task<List<Image>> getRecipeImages(int recipeId)
        {
            return await _context.Images
                .Where(i => i.EntityType == "Recipe" && i.EntityId == recipeId)
                .ToListAsync();
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

        public void RemoveRange(List<Image> images)=> _context.RemoveRange(images);
        

        public void Update(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
        }

       
    }
}

