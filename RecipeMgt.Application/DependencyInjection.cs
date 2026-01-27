using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeMgt.Application.Mapper;
using RecipeMgt.Application.Services.Auth;
using RecipeMgt.Application.Services.Cloudiary;
using RecipeMgt.Application.Services.Dishes;
using RecipeMgt.Application.Services.Recipes;
using RecipentMgt.Infrastucture.Repository.Recipes;
using RecipeMgt.Application.Services.Comments;
using RecipeMgt.Application.Services.Bookmarks;
using RecipeMgt.Application.Services.Ratings;
using RecipeMgt.Application.Services.Followings;
using RecipeMgt.Application.Services.Images;
using RecipeMgt.Application.Services.Steps;
using RecipeMgt.Application.Services.Ingredients;

namespace RecipeMgt.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IRecipeServices, RecipeServices>();
            services.AddScoped<ICommentServices, CommentServices>();
            services.AddScoped<IBookmarkService, BookmarkService>();
            services.AddScoped<IRatingService, RatingService>();    
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IFollowingService,  FollowingService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IStepService, StepService>();
            services.AddScoped<IIngredientService, IngredientService>();
            
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:Key"], typeof(DishProfile));
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:Key"], typeof(RecipeProfile));
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:Key"], typeof(StepProfile));
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:Key"], typeof(IngredientProfile));
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:Key"], typeof(CommentProfile));
            

            return services;
        }
    }
}
