using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Dishes
{
    public interface IDishService
    {
        public Task<Result<PagedResponse<DishResponse>>> getDishes(int page, int pageSize, string? searchQuery, int? categoryId);

        public Task<Result<DishDetailResponse>> GetDishDetail(int id, int ?userId, string? sessionId);

        public Task<Result<CreateDishResponse>> CreateDish(CreateDishRequest dish);

        public Task<Result> UpdateDish(UpdateDishRequest dish);

        public Task<Result> DeleteDish(int id);

        public Task<Result<IEnumerable<DishResponse>>> GetRelatedDish(int id);

        public Task<Result<IEnumerable<DishResponse>>> GetSuggestedDish(int id, int userId);

        public Task<Result<IEnumerable<DishResponse>>> GetTopViewCount();

        public Task CalculateStructuralDish(int dishId, CancellationToken token);
        Task<Result<IEnumerable<DishResponse>>> GetTopViewDishes();
    }
}
