using RecipeMgt.Views.Models;
using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Interface
{
    public interface IDishClient
    {
        Task<List<DishResponse>> GetAllAsync();

        Task<List<DishResponse>> GetByCategoryAsync(int categoryId);

        Task<ApiResponse<DishDetailResponse>> GetDetailAsync(int id);

        Task<CreateDishResponse> CreateAsync(MultipartFormDataContent form);

        Task DeleteAsync(int id);

        Task <UpdateDishResponse> UpdateAsync(MultipartFormDataContent form);


    }
}
