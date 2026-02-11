using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.Enums;
using RecipeMgt.Domain.RequestEntity;
using RecipentMgt.Infrastucture.Persistence;
using RecipentMgt.Infrastucture.Utils;
using System.Threading;

namespace RecipentMgt.Infrastucture.Repository.Dishes
{
    public class DishRepository : IDishRepository
    {
        private const string ENTITY_TYPE = "Dish";

        private readonly RecipeManagementContext _context;
        private readonly ILogger<DishRepository> _logger;

        public DishRepository(
            RecipeManagementContext context,
            ILogger<DishRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Create / Update / Delete

        public async Task<(bool Success, string Message, Dish? Data)> CreateDish(
            Dish dish,
            List<Image>? images)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();

                await SaveDishImages(dish.DishId, images);

                await transaction.CommitAsync();
                _logger.LogInformation(
                    "Created dish {DishName} (Id: {DishId})",
                    dish.DishName,
                    dish.DishId);

                return (true, "Dish created successfully", dish);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "CreateDish failed");
                return (false, "Error creating dish", null);
            }
        }

        public async Task<(bool Success, string Message)> DeleteDish(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                    return (false, $"Dish with id {id} not found");

                await RemoveDishImages(id);

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Deleted dish {DishId}", id);

                return (true, "Dish deleted successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "DeleteDish failed for id {DishId}", id);
                return (false, "Error deleting dish");
            }
        }

        public async Task<(bool Success, string Message, int DishId)> UpdateDish(
            Dish dish,
            List<Image>? images)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingDish = await _context.Dishes
                    .FirstOrDefaultAsync(d => d.DishId == dish.DishId);

                if (existingDish == null)
                    return (false, "Dish not found", 0);

                existingDish.DishName = dish.DishName;
                existingDish.Description = dish.Description;

                await RemoveDishImages(dish.DishId);
                await SaveDishImages(dish.DishId, images);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Updated dish {DishId}", dish.DishId);
                return (true, "Dish updated successfully", dish.DishId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "UpdateDish failed");
                return (false, "Error updating dish", 0);
            }
        }

        #endregion

        #region Queries

        public async Task<PagedResponse<Dish>> Search(
            int page,
            int pageSize,
            string? searchQuery = null,
            int? categoryId = null
            )
        {
            var query = BaseDishQuery()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(d =>
                    d.DishName.Contains(searchQuery));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(d =>
                    d.CategoryId == categoryId.Value);
            }

            query = query.OrderByDescending(d => d.DishId);

            var pagedResult = await PaginationHelper
                .ToPagedResponseAsync(query, page, pageSize);

            await LoadImagesForDishes(pagedResult.Items);

            return pagedResult;
        }

        

        public async Task<Dish?> GetById(int id)
        {
            var dish = await BaseDishQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DishId == id);

            if (dish != null)
                dish.Images = await GetDishImagesInternal(id);

            return dish;
        }
        public async Task<List<string>> GetDishImages(int dishId)
        {
            return await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityId == dishId && i.EntityType == ENTITY_TYPE)
                .OrderByDescending(i => i.UploadedAt)
                .Select(i => i.ImageUrl)
                .ToListAsync();
        }

        public async Task<List<Dish>> GetRelateDishAsync(int dishId)
        {
            return await _context.RelatedDishes
        .Where(rd =>
            rd.DishId == dishId &&
            rd.RelationType == DishRelationType.Structural)
        .Include(rd => rd.RelateDish)
            .ThenInclude(d => d.Statistic)
        .Select(rd => rd.RelateDish)
        .Take(10)
        .ToListAsync();
        }

        public async Task<List<Dish>> GetTopViewDishesAsync()
        {
            return await _context.Dishes
         .Include(d => d.Statistic)
         .Where(d => d.Statistic != null)
         .OrderByDescending(d => d.Statistic.ViewCount)
         .Take(5)
         .ToListAsync();
        }

        public async Task<List<Dish>> GetSuggestedDishAsync(int dishId)
        {
            return await _context.RelatedDishes
                .Where(rd =>
                    rd.DishId == dishId &&
                    rd.RelationType == DishRelationType.Behavior &&
                    rd.RelatedDishId != dishId
                )
                .OrderByDescending(rd => rd.Priority)
                .ThenByDescending(rd => rd.LastUpdatedAt)
                .Select(rd => rd.RelateDish)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();
        }



        #endregion

        #region Helpers

        private IQueryable<Dish> BaseDishQuery()
        {
            return _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Recipes)
                .Include(d => d.Statistic);
        }

        private async Task LoadImagesForDishes(IEnumerable<Dish> dishes)
        {
            var dishIds = dishes.Select(d => d.DishId).ToList();

            var images = await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityType == ENTITY_TYPE && dishIds.Contains(i.EntityId))
                .ToListAsync();

            foreach (var dish in dishes)
            {
                dish.Images = images
                    .Where(i => i.EntityId == dish.DishId)
                    .ToList();
            }
        }

        private async Task<List<Image>> GetDishImagesInternal(int dishId)
        {
            return await _context.Images
                .AsNoTracking()
                .Where(i => i.EntityId == dishId && i.EntityType == ENTITY_TYPE)
                .ToListAsync();
        }

        private async Task SaveDishImages(int dishId, List<Image>? images)
        {
            if (images == null || !images.Any()) return;

            foreach (var img in images)
            {
                img.EntityId = dishId;
                img.EntityType = ENTITY_TYPE;
                img.UploadedAt = DateTime.UtcNow;
            }

            _context.Images.AddRange(images);
            await _context.SaveChangesAsync();
        }

        private async Task RemoveDishImages(int dishId)
        {
            var images = await _context.Images
                .Where(i => i.EntityType == ENTITY_TYPE && i.EntityId == dishId)
                .ToListAsync();

            if (images.Any())
                _context.Images.RemoveRange(images);
        }

        public async Task CalculateStuctureDish(int dishId, CancellationToken cancellationToken)
        {
            var dish = await _context.Dishes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DishId == dishId, cancellationToken);

            if (dish == null) return;

            var oldRelations = _context.RelatedDishes
            .Where(x => x.DishId == dishId &&
                        x.RelationType == DishRelationType.Structural);

            _context.RelatedDishes.RemoveRange(oldRelations);

            var candidates = await _context.Dishes
    .Where(x => x.DishId != dishId)
    .Select(x => new
    {
        Dish = x,
        Score =
            (x.CategoryId == dish.CategoryId ? 5 : 0) +
            (x.AuthorId == dish.AuthorId ? 3 : 0)
            
    })
    .Where(x => x.Score > 0)
    .OrderByDescending(x => x.Score)
    .Take(20)
    .ToListAsync(cancellationToken);
            foreach (var item in candidates)
            {
                var realtion = new RelatedDish
                {
                    DishId = dishId,
                    RelatedDishId = item.Dish.DishId,
                    RelationType = DishRelationType.Structural,
                    Priority = item.Score,
                    LastUpdatedAt = DateTime.UtcNow
                };
            }
            
        }

        #endregion


    }
}
