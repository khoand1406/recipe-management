using RecipeMgt.Application.DTOs.Response.User;

namespace RecipeMgt.Views.Models.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<UserResponseMgtDTO> Users { get; set; } = [];

        public string? SearchQuery { get; set; }

        public int? StatusFilter { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int PageCount { get; set; }

        public int TotalCount { get; set; }


    }
}
