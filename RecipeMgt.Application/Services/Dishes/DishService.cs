using AutoMapper;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Dishes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Dishes
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<DishService> _logger;
        public DishService(IDishRepository repo, IMapper mapper, ILogger<DishService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateDishResponse> CreateDish(CreateDishRequest dish)
        {
            try
            {
                var dishMapped = _mapper.Map<Dish>(dish);
                var result = await _repo.CreateDish(dishMapped);
                if (result.Success)
                {

                    return new CreateDishResponse
                    {
                        Success = true,
                        Message = "Create Dish Successfully",
                        Data = { DishId = result.TraceIdentifier, DishName = dish.DishName, Description = dish.Description, CategoryId = dish.CategoryId }

                    };
                }
                return new CreateDishResponse { Success = false, Message = result.Message };
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error creating dish: {DishName}", dish.DishName);
                return new CreateDishResponse
                {
                    Success = false,
                    Message = "An error occurred while creating dish: "
                };
            }
            
        }

        public async Task<DeleteDishResponse> deleteDish(int id)
        {
            try
            {
                var result = await _repo.DeleteDish(id);
                if (result.Success)
                {
                    return new DeleteDishResponse
                    {
                        Success = true,
                        Message = result.Message
                    };
                }
                return new DeleteDishResponse
                {
                    Success = false,
                    Message = result.Message
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating dish: {id}", id);
                return new DeleteDishResponse
                {
                    Success = false,
                    Message = "Error occurs during update dish"
                };
            }
            
        }

        public async Task<DishDetailResponse> GetDishDetail(int id)
        {
            var dishDetail=  await _repo.GetById(id);
            var mapped= _mapper.Map<DishDetailResponse>(dishDetail);
            return mapped;
        }

        public async Task<IEnumerable<DishResponse>> getDishes()
        {
            var listDish= await _repo.GetAll();
            var mapped= _mapper.Map<IEnumerable<DishResponse>>(listDish);
            return mapped;
        }

        public async Task<IEnumerable<DishResponse>> getDishesByCategory(int categoryId)
        {
            var listDish= await _repo.GetByCategory(categoryId);
            var mapped = _mapper.Map<IEnumerable<DishResponse>>(listDish);
            return mapped;
        }

        public async Task<UpdateDishResponse> UpdateDish(UpdateDishRequest dish)
        {
            try
            {
                var mapped= _mapper.Map<Dish>(dish);
                var result = await _repo.UpdateDish(mapped);
                if(result.Success)
                {
                    return new UpdateDishResponse
                    {
                        Success = true,
                        Message = result.Message
                    };
                }
                return new UpdateDishResponse
                {
                    Success = false,
                    Message = result.Message
                };
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating dish: {id}", dish.DishId);
                return new UpdateDishResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
