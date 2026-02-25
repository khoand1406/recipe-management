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

                var baseQuery = db.UserActivityLogs
    .Where(x => x.CreatedAt >= fromTime)
    .Where(x =>
        (x.ActivityType == UserActivityType.View ||
         x.ActivityType == UserActivityType.Bookmark)
        && x.TargetType == Entity_Type
    )
    .Select(x => new
    {
        x.UserId,
        x.SessionId,
        DishId = x.TargetId!.Value
    });

                var relations = await (
    from a in baseQuery
    from b in baseQuery
    where
        (
            (a.UserId != null && a.UserId == b.UserId) ||
            (a.UserId == null && b.UserId == null && a.SessionId == b.SessionId)
        )
        && a.DishId < b.DishId
    group 1 by new
    {
        DishId = a.DishId,
        RelatedDishId = b.DishId
    } into g
    select new
    {
        DishId = g.Key.DishId,
        RelatedDishId = g.Key.RelatedDishId,
        Weight = g.Count()
    }
).ToListAsync(cancellationToken);

                if (relations.Count == 0)
                    return;

               
                var dishIds = relations
                    .SelectMany(x => new[] { x.DishId, x.RelatedDishId })
                    .Distinct()
                    .ToList();

                var existingRelations = await db.RelatedDishes
                    .Where(x =>
                        dishIds.Contains(x.DishId) &&
                        dishIds.Contains(x.RelatedDishId) &&
                        x.RelationType == DishRelationType.Behavior
                    )
                    .ToListAsync(cancellationToken);

                
                foreach (var r in relations)
                {
                    Upsert(existingRelations, db, r.DishId, r.RelatedDishId, r.Weight);
                    Upsert(existingRelations, db, r.RelatedDishId, r.DishId, r.Weight);
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