using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;

namespace RecipentMgt.Infrastucture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastucture(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<RecipeManagementContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("ConnectionString")));
            return services;
        }
    }
}
