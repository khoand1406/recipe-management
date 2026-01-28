using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Domain.RequestEntity;
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

        public Task<Result<DishDetailResponse>> GetDishDetail(int id);

        public Task<Result<CreateDishResponse>> CreateDish(CreateDishRequest dish);

        public Task<Result> UpdateDish(UpdateDishRequest dish);

        public Task<Result> DeleteDish(int id);
    }
}
