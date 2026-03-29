using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IDishClient
    {
        Task<List<Models.Response.DishResponse>> GetAllAsync();

        Task<PagedResponse<CategoryDishResponse>> GetDishesAsync(int page, int pageSize, string? search, int? categoryId);

        Task<ApiResponse<Models.Response.DishDetailResponse>> GetDetailAsync(int id);

        Task<Models.Response.CreateDishResponse> CreateAsync(MultipartFormDataContent form);

        Task DeleteAsync(int id);

        Task <UpdateDishResponse> UpdateAsync(MultipartFormDataContent form);


    }
}
