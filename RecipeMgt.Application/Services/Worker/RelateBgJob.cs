using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.Enums;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Worker
{
    public class RelateBgJob : IRelateBgJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ILogger<RelateBgJob> _logger;
        private const string Entity_Type = "Dish";
        public RelateBgJob(IServiceScopeFactory serviceScopeFactory, ILogger<RelateBgJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RecipeManagementContext>();
            try
            {
                var formTime = DateTime.UtcNow.AddMinutes(-30);
                var activities = await db.UserActivityLogs.Where(x => x.CreatedAt >= formTime).Where(x => x.ActivityType == Domain.Enums.UserActivityType.View
                || x.ActivityType == Domain.Enums.UserActivityType.Bookmark).OrderBy(x => x.CreatedAt).ToListAsync(cancellationToken);
                var groupByUser = activities.GroupBy(x => x.UserId);
                foreach (var item in groupByUser)
                {
                    var dishIds = item
                    .Where(x => x.TargetType == "Dish")
                    .Select(x => x.TargetId!.Value)
                    .Distinct()
                    .ToList();
                    for (int i = 0; i < dishIds.Count; i++)
                    {
                        for (int j = 1; j < dishIds.Count; j++)
                        {
                            var weight = CalculateWeight(item);
                            await UpsertRelateDishAsync(db, dishIds[i], dishIds[j], weight, cancellationToken);
                            await UpsertRelateDishAsync(db, dishIds[j], dishIds[i], weight, cancellationToken);
                        }
                    }
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Background Job Run Failed");
            }
        }

        private static int CalculateWeight(IEnumerable<UserActivityLog> logs)
        {
            var score = 0;

            foreach (var log in logs)
            {
                score += log.ActivityType switch
                {
                    UserActivityType.View => 1,
                    UserActivityType.Bookmark => 5,
                    _ => 0
                };
            }

            return score;
        }

        private static async Task UpsertRelateDishAsync(RecipeManagementContext db, int dishId, int relateDishId, int weight, CancellationToken cancellationToken)
        {
            var entity = await db.RelatedDishes.FirstOrDefaultAsync(x => x.DishId == dishId && x.RelatedDishId == relateDishId
            && x.RelationType.Equals(DishRelationType.Behavior), cancellationToken);
            if (entity != null)
            {
                entity.Priority += weight;
                entity.LastUpdatedAt = DateTime.UtcNow;
            }
            else
            {
                db.RelatedDishes.Add(new RelatedDish { 
                    DishId= dishId, 
                    LastUpdatedAt = DateTime.UtcNow, 
                    RelatedDishId= relateDishId, 
                    RelationType= DishRelationType.Behavior, 
                    Priority= weight 
                });
            }
        }
    }
}