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

namespace RecipeMgt.Application.Services.Dishes
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<DishService> _logger;
        private readonly IImageService _imageService;

        public DishService(IDishRepository repo, IMapper mapper, ILogger<DishService> logger, IImageService service)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
            _imageService = service;
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

        public async Task<Result<DishDetailResponse>> GetDishDetail(int id)
        {
            var dish = await _repo.GetById(id);
            if (dish == null)
                return Result<DishDetailResponse>.Failure("DISH_NOT_FOUND");

            var response = _mapper.Map<DishDetailResponse>(dish);
            response.ImageUrls = await _repo.GetDishImages(id);

            return Result<DishDetailResponse>.Success(response);
        }

        public async Task<Result<PagedResponse<DishResponse>>> getDishes(int page, int pageSize, string? searchQuery, int? categoryId)
        {
            var result = await _repo.Search(page, pageSize, searchQuery, categoryId);
            var mappedResult = new PagedResponse<DishResponse>
            {
                Items = _mapper.Map<IEnumerable<DishResponse>>(result.Items),
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                Page = result.Page,
                PageSize = pageSize,
            };
            return Result<PagedResponse<DishResponse>>.Success(mappedResult);
        }
    }

}
