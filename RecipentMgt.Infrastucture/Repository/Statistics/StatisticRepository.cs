using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Enums;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Statistics
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly RecipeManagementContext _context;

        public StatisticRepository(RecipeManagementContext context)
        {
            _context = context;
        }
        public async Task IncreaseRecipeBookmarkAsync(int recipeId)
        {
            await IncreaseCounter(RecipeStatisticColumn.BookmarkCount, recipeId);
        }

        public async Task IncreaseRecipeCommentAsync(int recipeId)
        {
            await IncreaseCounter(RecipeStatisticColumn.CommentCount, recipeId);
        }

        public async Task IncreaseRecipeViewAsync(int recipeId)
        {
            await _context.Database.ExecuteSqlRawAsync(
            @"
    MERGE RecipeStatistics WITH (HOLDLOCK) AS target
    USING (SELECT @recipeId AS RecipeId) AS source
    ON target.RecipeId = source.RecipeId
    WHEN MATCHED THEN
        UPDATE SET 
            ViewCount = ViewCount + 1,
            LastUpdatedAt = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (RecipeId, ViewCount, LastUpdatedAt)
        VALUES (@recipeId, 1, GETDATE());
    ",
            new SqlParameter("@recipeId", recipeId));
        }


        public async Task IncreaseUserCommentAsync(int userId)
        {
            await IncreaseUserCounter("RecipeCount", userId);
        }

        public async Task IncreaseUserFollowerAsync(int userId)
        {
            await IncreaseUserCounter("FollowerCount", userId);
        }

        public async Task IncreaseUserRatingAsync(int userId)
        {
            await IncreaseUserCounter("RatingCount", userId);
        }

        public async Task IncreaseUserRecipeAsync(int userId)
        {
            await IncreaseUserCounter("RecipeCount", userId);
        }

        public async Task UpdateRecipeRatingAsync(int recipeId, int ratingValue)
        {
            await _context.Database.ExecuteSqlRawAsync(
            @"
                MERGE RecipeStatistics WITH (HOLDLOCK) AS target
                USING (SELECT @recipeId AS RecipeId, @rating AS Rating) AS source
                ON target.RecipeId = source.RecipeId
    WHEN MATCHED THEN
        UPDATE SET
            AvgRating =
                ((AvgRating * RatingCount) + source.Rating) / (RatingCount + 1.0),
            RatingCount = RatingCount + 1,
            LastUpdatedAt = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (RecipeId, AvgRating, RatingCount, LastUpdatedAt)
        VALUES (@recipeId, source.Rating, 1, GETDATE());
    ",
            new SqlParameter("@recipeId", recipeId),
            new SqlParameter("@rating", ratingValue));
        }


        private async Task IncreaseCounter(RecipeStatisticColumn column, int recipeId)
        {
            var columnName = MapEnumString(column);
            await _context.Database.ExecuteSqlRawAsync($@"
    MERGE RecipeStatistics WITH (HOLDLOCK) AS target
    USING (SELECT @recipeId AS RecipeId) AS source
    ON target.RecipeId = source.RecipeId
    WHEN MATCHED THEN
        UPDATE SET 
            {columnName} = {columnName} + 1,
            LastUpdatedAt = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (RecipeId, {columnName}, LastUpdatedAt)
        VALUES (@recipeId, 1, GETDATE());
    ",
            new SqlParameter("@recipeId", recipeId));
        }
        private async Task IncreaseUserCounter(string column, int userId)
        {

            await _context.Database.ExecuteSqlRawAsync(
                $@"UPDATE UserStatistics
               SET {column} = {column} + 1,
                   LastUpdatedAt = GETDATE()
               WHERE UserId = @userId",
                new SqlParameter("@userId", userId));
        }

        private static string MapEnumString(RecipeStatisticColumn column)
        {
            return column switch
            {
                RecipeStatisticColumn.BookmarkCount => "BookmarkCount",
                RecipeStatisticColumn.CommentCount => "CommentCount",
                RecipeStatisticColumn.ViewCount => "ViewCount",
                _ => throw new ArgumentOutOfRangeException(nameof(column)),

            };
        }
    }
}
