using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.RequestEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Dishes
{
    public interface IDishRepository
    {
       
        public Task<Dish?> GetById(int id);

        public Task<PagedResponse<Dish>> Search(int page, int pageSize, string? searchQuery,int? categoryId );
        public Task<(bool Success, string Message, Dish? Data)> CreateDish(Dish dish, List<Image> images);

        public Task<(bool Success, string Message, int DishId)> UpdateDish(Dish dish, List<Image> images);

        public Task<(bool Success, string Message)> DeleteDish(int id);

        Task<List<string>> GetDishImages(int dishId);



    }
}
