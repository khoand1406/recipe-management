using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Dishes
{
    public class DishRepository : IDishRepository
    {
        private readonly RecipeManagementContext _context;
        private readonly ILogger<DishRepository> _logger;

        public DishRepository(RecipeManagementContext context, ILogger<DishRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, Dish? Data)> CreateDish(Dish dish, List<Image> images)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();

                // Liên kết ảnh với Dish mới
                if (images != null && images.Any())
                {
                    foreach (var img in images)
                    {
                        img.EntityId = dish.DishId;
                        img.EntityType = "Dish";
                        img.UploadedAt = DateTime.Now;
                    }

                    _context.Images.AddRange(images);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                _logger.LogInformation($"Created dish '{dish.DishName}' successfully with {images?.Count} images.");

                return (true, "Dish created successfully", dish);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error creating dish: {ex.Message}");
                return (false, "Error creating dish", null);
            }
        }

        public async Task<(bool Success, string Message)> DeleteDish(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                    return (false, $"Dish with id {id} not found");

                // Xóa cả hình ảnh liên quan (nếu có)
                var relatedImages = _context.Images
                    .Where(img => img.EntityType == "Dish" && img.EntityId == id);
                _context.Images.RemoveRange(relatedImages);

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Deleted dish with id: {DishId}", id);
                return (true, $"Successfully deleted dish with id: {id}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting dish {DishId}", id);
                return (false, ex.Message);
            }
        }

        public async Task<IEnumerable<Dish>> GetAll()
        {
            var dishes = await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Recipes)
                .ToListAsync();

            // Nạp ảnh cho từng dish
            foreach (var dish in dishes)
            {
                dish.Images = await _context.Images
                    .Where(img => img.EntityType == "Dish" && img.EntityId == dish.DishId)
                    .ToListAsync();
            }

            return dishes;
        }


        public async Task<IEnumerable<Dish>> GetByCategory(int categoryId)
        {
            var dishes = await _context.Dishes
                .Where(d => d.CategoryId == categoryId)
                .Include(d => d.Category)
                .Include(d => d.Recipes)
                .ToListAsync();

            foreach (var dish in dishes)
            {
                dish.Images = await _context.Images
                    .Where(img => img.EntityType == "Dish" && img.EntityId == dish.DishId)
                    .ToListAsync();
            }

            return dishes;
        }


        public async Task<Dish?> GetById(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Recipes)
                .FirstOrDefaultAsync(d => d.DishId == id);

            if (dish != null)
            {
                dish.Images = await _context.Images
                    .Where(img => img.EntityType == "Dish" && img.EntityId == dish.DishId)
                    .ToListAsync();
            }

            return dish;
        }


        public async Task<IEnumerable<Dish>> GetDishesBySearchQuery(string searchQuery)
        {
            var dishes = await _context.Dishes
                .Where(d => d.DishName.Contains(searchQuery))
                .Include(d => d.Category)
                .Include(d => d.Recipes)
                .ToListAsync();

            foreach (var dish in dishes)
            {
                dish.Images = await _context.Images
                    .Where(img => img.EntityType == "Dish" && img.EntityId == dish.DishId)
                    .ToListAsync();
            }

            return dishes;
        }

        public async Task<List<string>> GetDishImages(int dishId)
        {
            return await _context.Images
            .Where(i => i.EntityId == dishId && i.EntityType == "Dish")
             .OrderByDescending(i => i.UploadedAt)
             .Select(i => i.ImageUrl)
            .ToListAsync();
        }

        public async Task<(bool Success, string Message, int TraceIdentifier)> UpdateDish(Dish dish, List<Image> images)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingDish = await _context.Dishes
                    .Include(d => d.Category)
                    .FirstOrDefaultAsync(d => d.DishId == dish.DishId);

                if (existingDish == null)
                    return (false, "Dish not found", 0);

                // Update thông tin cơ bản
                existingDish.DishName = dish.DishName;
                existingDish.Description = dish.Description;



                // Xoá ảnh cũ nếu có
                var oldImages = await _context.Images
                    .Where(i => i.EntityType == "Dish" && i.EntityId == dish.DishId)
                    .ToListAsync();

                if (oldImages.Any())
                {
                    _context.Images.RemoveRange(oldImages);
                    await _context.SaveChangesAsync();
                }

                // Thêm ảnh mới
                if (images != null && images.Any())
                {
                    foreach (var img in images)
                    {
                        img.EntityId = dish.DishId;
                        img.EntityType = "Dish";
                        img.UploadedAt = DateTime.Now;
                    }

                    _context.Images.AddRange(images);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Updated dish {dish.DishId} successfully with {images?.Count} new images.");

                return (true, "Dish updated successfully", dish.DishId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error updating dish: {ex.Message}");
                return (false, "Error updating dish", 0);
            }
        }
    }
}
