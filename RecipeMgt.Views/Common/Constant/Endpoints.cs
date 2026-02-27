namespace RecipeMgt.Views.Common.Constant
{
    public static class Endpoints
    {
        public const string AuthLoginEndpoint = "/api/auth/login";
        public const string AuthRegisterEndpoint = "/api/auth/register";
        public const string AuthLogoutEndpoint = "/api/auth/logout";
        public const string AuthRefreshTokenEndpoint = "/api/auth/refresh-token";
        public const string AuthUserChangePasswordEndpoint = "/api/auth/change-password";


        public const string ApiCategoryEndpoint = "/api/category";

        public const string ApiDishEndpoint = "/api/dish";
        public const string ApiDishDetailEndpoint = "/api/dish/{0}";
        public const string ApiTopDishViewEndpoint = "/api/dish/top-view";
        public const string ApiLatestDishesEndpoint = "/api/dish/latest";
        public const string ApiDishByCategoryEndpoint = "/api/dish/cate/{0}";
        public const string ApiDishCreateEndpoint = "/api/dish/create";
        public const string ApiDishUpdateEndpoint = "/api/dish/update";
        public const string ApiDishDeleteEndpoint = "/api/dish/delete/{0}";


        public const string ApiUserEndpoint = "/api/user";
        public const string ApiTopContributorEndpoint = "/api/user/top-contributors";
        
        

    }
}
