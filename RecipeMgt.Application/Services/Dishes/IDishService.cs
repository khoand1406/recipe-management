using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Response.Dishes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Dishes
{
    public interface IDishService
    {
        public Task<IEnumerable<DishResponse>> getDishes();

        public Task<DishDetailResponse> GetDishDetail(int id);

        public Task<CreateDishResponse> CreateDish(CreateDishRequest dish);

        public Task<UpdateDishResponse> UpdateDish(UpdateDishRequest dish);

        public Task<IEnumerable<DishResponse>> getDishesByCategory(int categoryId);

        public Task<DeleteDishResponse> deleteDish(int id);
    }
}
