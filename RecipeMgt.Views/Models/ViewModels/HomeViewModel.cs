using RecipeMgt.Views.Models.Response;

namespace RecipeMgt.Views.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<CategoryDto> Categories { get; set; } = [];
        public List<DishResponse> TopDishes { get; set; } = [];
        public List<UserResponse> TopUsers { get; set; } = [];
    }
}
