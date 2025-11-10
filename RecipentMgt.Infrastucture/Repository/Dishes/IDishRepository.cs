using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Dishes
{
    public interface IDishRepository
    {
        public Task<IEnumerable<Dish>> GetAll();

        public Task<Dish?> GetById(int id);

        public Task<IEnumerable<Dish>> GetByCategory(int categoryId);

        public Task<IEnumerable<Dish>> GetDishesBySearchQuery(string searchQuery);

        public Task<(bool Success, string Message, Dish? Data)> CreateDish(Dish dish, List<Image> images);

        public Task<(bool Success, string Message, int TraceIdentifier)> UpdateDish(Dish dish, List<Image> images);

        public Task<(bool Success, string Message)> DeleteDish(int id);

        Task<List<string>> GetDishImages(int dishId);



    }
}
