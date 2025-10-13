using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;
using RecipentMgt.Infrastucture.Repository;

namespace RecipentMgt.Infrastucture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<RecipeManagementContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("ConnectionString")));
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
