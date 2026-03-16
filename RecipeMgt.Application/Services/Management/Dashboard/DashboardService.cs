using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response.Management.Dashboard;
using RecipentMgt.Infrastucture.Repository.Categories;
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Repository.Recipes;
using RecipentMgt.Infrastucture.Repository.Statistics;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Management.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStatisticRepository _statisticRepository;
        private readonly IRecipeRepository _recipeRepository;


        public DashboardService(IDishRepository dishRepository, IUserRepository userRepository, ICategoryRepository categoryRepository, IStatisticRepository statisticRepository, IRecipeRepository recipeRepository) {
            _categoryRepository = categoryRepository;
            _dishRepository = dishRepository;
            _userRepository = userRepository;
            _statisticRepository = statisticRepository;
            _recipeRepository = recipeRepository;
        }
        public async Task<Result<List<ChartCategoryDishResponse>>> GetChartCategory()
        {
            var category= await _categoryRepository.GetAll();
            var categoryDict= await _categoryRepository.GetDishCount();
            var mappedResult= category.Select(c=> new ChartCategoryDishResponse{
                                                    CategoryId= c.CategoryId, 
                                                    CategoryName=c.CategoryName, 
                                                    DishCount= categoryDict.GetValueOrDefault(c.CategoryId)
                                                    }).ToList();
            return Result<List<ChartCategoryDishResponse>>.Success(mappedResult);

        }

        public async Task<Result<List<ChartDishCreateResponse>>> GetChartMonthly()
        {
            var dishData = await _dishRepository.GetChartCreateMonthly();
            var mappedResult= dishData.Select(c=> new ChartDishCreateResponse
            {
                Month = c.Month,
                RecipeCount = c.RecipeCount,
            }).ToList();
            return Result<List<ChartDishCreateResponse>>.Success(mappedResult);

        }

        public async Task<Result<DashboardMetricResponse>> GetDashboardMetric()
        {
            var totalRecipes = await _dishRepository.CountAsync();
            var totalUsers = await _userRepository.CountAsync();
            var totalCategories = await _categoryRepository.CountAsync();
            var totalReviews = await _statisticRepository.GetRatingCount();

            var response = new DashboardMetricResponse
            {
                TotalRecipes = totalRecipes,
                TotalUser = totalUsers,
                TotalDish = totalCategories,
                TotalReviews = totalReviews
            };

            return Result<DashboardMetricResponse>.Success(response);
        }

        public async Task<Result<List<ChartTopRecipeRatingResponse>>> GetTopDishRatingResponse()
        {
            var recipeStatistic = await _recipeRepository.GetTopRecipeRating();
            var mappedResult= recipeStatistic.Select(x=> new ChartTopRecipeRatingResponse { 
                RecipeId= x.RecipeId, 
                RecipeName= x.Title, 
                RecipeRating= x.RecipeStatistic.AvgRating})
                .ToList();
            return Result<List<ChartTopRecipeRatingResponse>>.Success(mappedResult);


        }
    }
}
