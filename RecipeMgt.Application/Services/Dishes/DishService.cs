using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecipeMgt.Application.Services.Images;
using RecipeMgt.Domain.RequestEntity;
using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipentMgt.Infrastucture.Repository.Statistics;
using RecipentMgt.Infrastucture.Repository.Recipes;
using RecipentMgt.Infrastucture.Repository.Users;

namespace RecipeMgt.Application.Services.Dishes
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _repo;
        private readonly IStatisticRepository _statisticRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DishService> _logger;
        private readonly IImageService _imageService;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;
        private const string ENTITY_TYPE = "Dish";

        public DishService(IDishRepository repo, IMapper mapper, ILogger<DishService> logger, IImageService service, IStatisticRepository statisticRepository, IRecipeRepository recipeRepository, IUserRepository userRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
            _imageService = service;
            _statisticRepository = statisticRepository;
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<CreateDishResponse>> CreateDish(CreateDishRequest request)
        {
            try
            {
                var dish = _mapper.Map<Dish>(request);

                var images = new List<Image>();
                if (request.Images?.Count > 0)
                {
                    images = await _imageService
                        .UploadEntityImagesAsync(request.Images, "Dish");
                }

                var result = await _repo.CreateDish(dish, images);
                if (!result.Success || result.Data == null)
                    return Result<CreateDishResponse>.Failure(result.Message);

                var response = _mapper.Map<CreateDishResponse>(result.Data);
                response.Data.ImageUrls = result.Data.Images?
                    .Select(i => i.ImageUrl)
                    .ToList() ?? [];

                return Result<CreateDishResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dish {DishName}", request.DishName);
                return Result<CreateDishResponse>.Failure("CREATE_DISH_FAILED");
            }
        }


        public async Task<Result> UpdateDish(UpdateDishRequest request)
        {
            try
            {
                var dishUpdate = _mapper.Map<Dish>(request);
                var images = new List<Image>();

                if (request.Images != null && request.Images.Count != 0)
                    images = await _imageService.UploadEntityImagesAsync(request.Images, "Dish");

                var (Success, Message, DishId) = await _repo.UpdateDish(dishUpdate, images);

                if (!Success)
                    return Result.Failure(Message);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating dish: {ex.Message}");
                return Result.Failure("UPDATE_DISH_FAILED");
            }
        }

        public async Task<Result> DeleteDish(int id)
        {
            var (Success, Message) = await _repo.DeleteDish(id);
            if (!Success)
            {
                return Result.Failure(Message);
            }
            return Result.Success();
        }

        public async Task<Result<DishDetailResponse>> GetDishDetail(int id, int userId)
        {
            var dish = await _repo.GetById(id);
            if (dish == null)
                return Result<DishDetailResponse>.Failure("DISH_NOT_FOUND");
            var relatedDishes = await GetRelatedDish(id);
            var suggestedDishes= await GetSuggestedDish(id);

            _ = Task.Run(async () =>
            {
                await _statisticRepository.IncreaseDishViewCount(id);
                await _userRepository.CreateUserActivityLog(userId, Domain.Enums.UserActivityType.Comment, ENTITY_TYPE, id, "");
            });
            
            var response = new DishDetailResponse
            {
                DishId = dish.DishId,
                DishName = dish.DishName,
                Category = dish.Category,
                CategoryId = dish.CategoryId,
                Description = dish.Description,
                Recipes = dish.Recipes.Select(x => new RecipeResponse
                {
                    RecipeId = x.RecipeId,
                    Title = x.Title,
                    DifficultyLevel = x.DifficultyLevel,
                    AuthorId = x.AuthorId,
                    Author = x.Author,
                    Description = x.Description,
                    CookingTime = x.CookingTime,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Images = [],
                    Servings = x.Servings
                }).ToList(),
                RelateDishes = relatedDishes.Value!= null ? relatedDishes.Value.ToList() : [],
                SuggestedDishes = suggestedDishes.Value!= null? suggestedDishes.Value.ToList(): [],

            };
        response.ImageUrls = await _repo.GetDishImages(id);
            return Result<DishDetailResponse>.Success(response);
        }


        public async Task<Result<DTOs.Response.Recipe.PagedResponse<DishResponse>>> getDishes(int page, int pageSize, string? searchQuery, int? categoryId)
        {
            var result = await _repo.Search(page, pageSize, searchQuery, categoryId);
            var mappedResult = new DTOs.Response.Recipe.PagedResponse<DishResponse>
            {
                Items = _mapper.Map<IEnumerable<DishResponse>>(result.Items),
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                Page = result.Page,
                PageSize = pageSize,
            };
            return Result<DTOs.Response.Recipe.PagedResponse<DishResponse>>.Success(mappedResult);
        }


        public async Task<Result<IEnumerable<DishResponse>>> GetRelatedDish(int id)
        {
            var result= await _repo.GetRelateDishAsync(id);
            var mappedResult = result.Select(item => _mapper.Map<DishResponse>(item));
            return Result<IEnumerable<DishResponse>>.Success(mappedResult);
        }

        public async Task<Result<IEnumerable<DishResponse>>> GetSuggestedDish(int id)
        {
            var result = await _repo.GetSuggestedDishAsync(id);
            var mappedResult = result.Select(item => _mapper.Map<DishResponse>(item));
            return Result<IEnumerable<DishResponse>>.Success(mappedResult) ;
        }

        public async Task<Result<IEnumerable<DishResponse>>> GetTopViewCount()
        {
            var result = await _repo.GetTopViewDishesAsync();
            var mappedResult = result.Select(item => _mapper.Map<DishResponse>(item));
            return Result<IEnumerable<DishResponse>>.Success(mappedResult);
        }
    }

}
