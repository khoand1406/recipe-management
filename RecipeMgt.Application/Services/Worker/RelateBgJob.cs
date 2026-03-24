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
        private readonly ILogger<RelateBgJob> _logger;

        private const string Entity_Type = "Dish";

        public RelateBgJob(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RelateBgJob> logger)
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
                var fromTime = DateTime.UtcNow.AddMinutes(-30);

                
                var confirmedDishIds = db.Dishes
                    .Where(d => d.IsConfirm)
                    .Select(d => d.DishId);

                var activities = await db.UserActivityLogs
                    .Where(log =>
                        log.CreatedAt >= fromTime &&
                        (log.ActivityType == UserActivityType.View ||
                         log.ActivityType == UserActivityType.Bookmark) &&
                        log.TargetType == Entity_Type &&
                        log.TargetId != null &&
                        confirmedDishIds.Contains(log.TargetId.Value))
                    .Select(log => new
                    {
                        log.UserId,
                        log.SessionId,
                        DishId = log.TargetId!.Value
                    })
                    .ToListAsync(cancellationToken);


                var grouped = activities
    .GroupBy(x => x.UserId.HasValue
        ? $"u_{x.UserId.Value}"
        : $"s_{x.SessionId!}");

                var relationDict = new Dictionary<(int, int), int>();

                foreach (var group in grouped)
                {
                    var dishes = group
                        .Select(x => x.DishId)
                        .Distinct()
                        .ToList();

                    int n = dishes.Count;

                    for (int i = 0; i < n; i++)
                    {
                        for (int j = i + 1; j < n; j++)
                        {
                            var d1 = dishes[i];
                            var d2 = dishes[j];

                            
                            var key = d1 < d2 ? (d1, d2) : (d2, d1);

                            if (!relationDict.TryAdd(key, 1))
                                relationDict[key]++;
                        }
                    }
                }

                if (relationDict.Count == 0)
                    return;

                var dishIds = relationDict.Keys
                    .SelectMany(k => new[] { k.Item1, k.Item2 })
                    .Distinct()
                    .ToList();

                
                var existingRelations = await db.RelatedDishes
                    .Where(x =>
                        x.RelationType == DishRelationType.Behavior &&
                        dishIds.Contains(x.DishId) &&
                        dishIds.Contains(x.RelatedDishId))
                    .ToListAsync(cancellationToken);

                var existingDict = existingRelations.ToDictionary(
                    x => (Math.Min(x.DishId, x.RelatedDishId),
                          Math.Max(x.DishId, x.RelatedDishId))
                );

                
                foreach (var kv in relationDict)
                {
                    var (dishId, relatedDishId) = kv.Key;
                    var weight = kv.Value;

                    if (existingDict.TryGetValue((dishId, relatedDishId), out var entity))
                    {
                        entity.Priority += weight;
                        entity.LastUpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        db.RelatedDishes.Add(new RelatedDish
                        {
                            DishId = dishId,
                            RelatedDishId = relatedDishId,
                            RelationType = DishRelationType.Behavior,
                            Priority = weight,
                            LastUpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                await db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RelateBgJob failed");
            }
        }

        private static void Upsert(
            List<RelatedDish> existing,
            RecipeManagementContext db,
            int dishId,
            int relatedDishId,
            int weight)
        {
            var entity = existing.FirstOrDefault(x =>
                x.DishId == dishId &&
                x.RelatedDishId == relatedDishId &&
                x.RelationType == DishRelationType.Behavior);

            if (entity != null)
            {
                entity.Priority += weight;
                entity.LastUpdatedAt = DateTime.UtcNow;
            }
            else
            {
                db.RelatedDishes.Add(new RelatedDish
                {
                    DishId = dishId,
                    RelatedDishId = relatedDishId,
                    RelationType = DishRelationType.Behavior,
                    Priority = weight,
                    LastUpdatedAt = DateTime.UtcNow
                });
            }
        }
    }
}