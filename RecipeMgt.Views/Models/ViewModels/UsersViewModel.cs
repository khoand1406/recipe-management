using RecipeMgt.Application.DTOs.Response.User;

namespace RecipeMgt.Views.Models.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<UserResponseMgtDTO> Users { get; set; } = [];

        public int ActiveCount { get; set; }
        public int PendingCount { get; set; }
        public int BannedCount { get; set; }

        public int RecipeCount { get; set; }    

        public int TotalRecipes { get; set; }

        public string? SearchQuery { get; set; }

        public int? StatusFilter { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int PageCount { get; set; }

        public int TotalCount { get; set; }


    }
}
