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
                var activities = await db.UserActivityLogs.Where(x => x.CreatedAt >= formTime)
                    .Where(x => x.ActivityType == UserActivityType.View
                            || x.ActivityType == UserActivityType.Bookmark 
                            && x.TargetType!.Equals(Entity_Type, StringComparison.OrdinalIgnoreCase))
                    .Select(x=> new
                    {
                        ActionKey= x.UserId != null ? $"user_{x.UserId}": $"session_{x.SessionId}",
                        DishId= x.TargetId!.Value, 
                    })
                    .ToListAsync(cancellationToken);

                var groupByUser = activities.GroupBy(x => x.ActionKey)
                    .Select(x => x.Select(x => x.DishId).Distinct().ToList())
                    .Where(list => list.Count>1 )
                    .ToList();

                var relationCounter = new Dictionary<(int, int), int>();
                foreach(var item in groupByUser)
                {
                    for(var i= 0; i< item.Count; i++)
                    {
                        for(var j= i+1; j< item.Count; j++)
                        {
                            var pair_1= (item[i],  item[j]);
                            var pair_2 = (item[j], item[i]);
                            relationCounter[pair_1] = relationCounter.GetValueOrDefault(pair_1) + 1;
                            relationCounter[pair_2] = relationCounter.GetValueOrDefault(pair_2) + 1;
                        }
                    }
                }
                if (relationCounter.Count == 0) {  return; }

                var dishIdsSet = relationCounter.Keys.SelectMany(x => new[] { x.Item1, x.Item2 })
                                 .Distinct().ToList();

                var existingRelation= await db.RelatedDishes.Where(x=> dishIdsSet.Contains(x.DishId) 
                                        && dishIdsSet.Contains(x.RelatedDishId) 
                                        && x.RelationType==DishRelationType.Behavior)
                                        .ToListAsync(cancellationToken);

                foreach(var item in relationCounter)
                {
                    var (dishId, relatedDishId) = item.Key;
                    var weight = item.Value;

                    var existing = existingRelation
                .FirstOrDefault(x =>
                    x.DishId == dishId &&
                    x.RelatedDishId == relatedDishId &&
                    x.RelationType == DishRelationType.Behavior);
                    if (existing != null)
                    {
                        existing.Priority += weight;
                        existing.LastUpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        db.RelatedDishes.Add(new RelatedDish
                        {
                            DishId= dishId,
                            RelatedDishId= relatedDishId,
                            RelationType= DishRelationType.Behavior,
                            Priority= weight,
                            LastUpdatedAt= DateTime.UtcNow
                        });
                    }
                }


                await db.SaveChangesAsync(cancellationToken);
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
                    UserActivityType.View => 5,
                    UserActivityType.Bookmark => 10,
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