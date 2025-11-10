using AutoMapper;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.DTOs.Request.Ingredients;
using RecipeMgt.Application.DTOs.Response.Ingredients;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Ingredients;

namespace RecipeMgt.Application.Services.Ingredients
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<IngredientService> _logger;

        public IngredientService(IIngredientRepository repository, IMapper mapper, ILogger<IngredientService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IngredientResponse?> GetByIdAsync(int ingredientId)
        {
            var ingredient = await _repository.GetByIdAsync(ingredientId);
            if (ingredient == null) return null;
            return _mapper.Map<IngredientResponse>(ingredient);
        }

        public async Task<IEnumerable<IngredientResponse>> GetByRecipeIdAsync(int recipeId)
        {
            var ingredients = await _repository.GetByRecipeIdAsync(recipeId);
            return _mapper.Map<IEnumerable<IngredientResponse>>(ingredients);
        }

        public async Task<CreateIngredientResponse> CreateAsync(CreateIngredientRequest request)
        {
            try
            {
                var ingredient = _mapper.Map<Ingredient>(request);
                var created = await _repository.CreateAsync(ingredient);
                return new CreateIngredientResponse
                {
                    Success = true,
                    Data = _mapper.Map<IngredientResponse>(created),
                    Message = "Ingredient created successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ingredient");
                return new CreateIngredientResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<UpdateIngredientResponse> UpdateAsync(UpdateIngredientRequest request)
        {
            try
            {
                var ingredient = _mapper.Map<Ingredient>(request);
                var updated = await _repository.UpdateAsync(ingredient);
                if (updated == null)
                {
                    return new UpdateIngredientResponse
                    {
                        Success = false,
                        Message = "Ingredient not found"
                    };
                }
                return new UpdateIngredientResponse
                {
                    Success = true,
                    Message = "Ingredient updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ingredient");
                return new UpdateIngredientResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DeleteIngredientResponse> DeleteAsync(int ingredientId)
        {
            try
            {
                var result = await _repository.DeleteAsync(ingredientId);
                if (!result)
                {
                    return new DeleteIngredientResponse
                    {
                        Success = false,
                        Message = "Ingredient not found"
                    };
                }
                return new DeleteIngredientResponse
                {
                    Success = true,
                    Message = "Ingredient deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ingredient");
                return new DeleteIngredientResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
