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
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:Key"], typeof(DishProfile));
            services.AddAutoMapper(cfg => cfg.LicenseKey = configuration["AutoMapper:Key"], typeof(RecipeProfile));
            

            return services;
        }
    }
}
