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

        public async Task<CreateDishResponse> CreateDish(CreateDishRequest request)
        {
            try
            {
                var dish = _mapper.Map<Dish>(request);
                var images = await _imageService.UploadEntityImagesAsync(request?.Images, "Dish");

                var result = await _repo.CreateDish(dish, images);
                if (!result.Success)
                    return new CreateDishResponse { Success = false, Message = result.Message };

                return new CreateDishResponse
                {
                    Success = true,
                    Message = "Create dish successfully",
                    Data = new DishResponse
                    {
                        DishId = dish.DishId,
                        DishName = dish.DishName,
                        Description = dish.Description,
                        CategoryId = dish.CategoryId,
                        ImageUrls = images.Select(i => i.ImageUrl).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dish: {DishName}", request.DishName);
                return new CreateDishResponse { Success = false, Message = "Error creating dish" };
            }
        }

        public async Task<UpdateDishResponse> UpdateDish(UpdateDishRequest request)
        {
            try
            {
                var dishUpdate = _mapper.Map<Dish>(request);
                var images = new List<Image>();

                if (request.Images != null && request.Images.Any())
                    images = await _imageService.UploadEntityImagesAsync(request.Images, "Dish");

                var result = await _repo.UpdateDish(dishUpdate, images);

                if (result.Success)
                    return new UpdateDishResponse { Success = true, Message = result.Message };

                return new UpdateDishResponse { Success = false, Message = result.Message ?? "Update failed" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating dish: {ex.Message}");
                return new UpdateDishResponse { Success = false, Message = "Update Dish Failed!" };
            }
        }

        public async Task<DeleteDishResponse> deleteDish(int id)
        {
            var result = await _repo.DeleteDish(id);
            return new DeleteDishResponse { Success = result.Success, Message = result.Message };
        }

        public async Task<DishDetailResponse?> GetDishDetail(int id)
        {
            var dish = await _repo.GetById(id);
            if (dish == null) return null;

            var response = _mapper.Map<DishDetailResponse>(dish);
            response.ImageUrls = await _repo.GetDishImages(id);
            return response;
        }

        public async Task<IEnumerable<DishResponse>> getDishes()
        {
            var dishes = await _repo.GetAll();
            var responses = _mapper.Map<IEnumerable<DishResponse>>(dishes).ToList();

            foreach (var dish in responses)
                dish.ImageUrls = await _repo.GetDishImages(dish.DishId);

            return responses;
        }

        public async Task<IEnumerable<DishResponse>> getDishesByCategory(int categoryId)
        {
            var dishes = await _repo.GetByCategory(categoryId);
            var responses = _mapper.Map<IEnumerable<DishResponse>>(dishes).ToList();

            foreach (var dish in responses)
                dish.ImageUrls = await _repo.GetDishImages(dish.DishId);

            return responses;
        }
    }

}
