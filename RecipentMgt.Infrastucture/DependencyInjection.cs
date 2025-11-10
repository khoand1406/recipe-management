using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using RecipentMgt.Infrastucture.Repository.Bookmarks;
using RecipentMgt.Infrastucture.Repository.Categories;
using RecipentMgt.Infrastucture.Repository.Comments;
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Repository.Following;
using RecipentMgt.Infrastucture.Repository.Ingredients;
using RecipentMgt.Infrastucture.Repository.Ratings;
using RecipentMgt.Infrastucture.Repository.Steps;
using RecipentMgt.Infrastucture.Repository.Users;

namespace RecipentMgt.Infrastucture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<RecipeManagementContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("ConnectionString")));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDishRepository, DishRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IBookmarkRepository, BookmarkRepository>();
            services.AddScoped<IFollowingRepository, FollowingRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IStepRepository, StepRepository>();
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            
            

            return services;
        }
    }
}
