using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<(bool Success, string Message, int TraceIdentifier)> CreateDish(Dish dish)
        {
            try
            {
                await _context.Dishes.AddAsync(dish);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfull created dish with id: " + dish.DishId);
                return (true, "Create new dish successfully", dish.DishId);
            }catch (Exception ex)
            {
                _logger.LogError("Error occurs while insert dish record to database: "+  ex.Message);
                return (false, ex.Message, 0);
            }
        }

        public async Task<(bool Success, string Message)> DeleteDish(int id)
        {
            try
            {
                var dish= await GetById(id);
                if(dish == null)
                {
                    return (false, "Not found dish with id: " + id);
                }
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
                return (true, "Successful delete dish with id: " + id);
            }catch (Exception ex)
            {
                _logger.LogError("Error occurs while insert dish record to database: " + ex.Message);
                return (false, ex.Message);
            }
        }

        public async Task<IEnumerable<Dish>> GetAll()
        {
            return await _context.Dishes.ToListAsync();
        }

        public async Task<IEnumerable<Dish>> GetByCategory(int categoryId)
        {
            return await _context.Dishes.Where(dish=> dish.CategoryId == categoryId).ToListAsync();
        }

        public async Task<Dish?> GetById(int id)
        {
            return await _context.Dishes.Include(dish=> dish.Recipes).FirstOrDefaultAsync(dish=> dish.DishId== id);
        }

        public async Task<IEnumerable<Dish>> GetDishesBySearchQuery(string searchQuery)
        {
            return await _context.Dishes.Where(dish=> dish.DishName.Contains(searchQuery)).ToListAsync();
        }

        public async Task<(bool Success, string Message, int TraceIdentifier)> UpdateDish(Dish dish )
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                
                var dishFound= await GetById(dish.DishId);
                if (dishFound == null)
                {
                    return (false, $"Not found dish with id: {dish.DishId}", 0);
                }
                _context.Dishes.Update(dish);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Update dish record with id: {dish.DishId} successfully");

                return (true, $"Update dish with id: {dish.DishId} successfully", dish.DishId);
            }catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Error occurs while insert dish record to database: " + ex.Message);
                return (false, ex.ToString(), 0);
            }
        }
    }
}
