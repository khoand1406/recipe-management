using AutoMapper;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.DTOs.Request.Steps;
using RecipeMgt.Application.DTOs.Response.Steps;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Steps;

namespace RecipeMgt.Application.Services.Steps
{
    public class StepService : IStepService
    {
        private readonly IStepRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<StepService> _logger;

        public StepService(IStepRepository repository, IMapper mapper, ILogger<StepService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StepResponse?> GetByIdAsync(int stepId)
        {
            var step = await _repository.GetByIdAsync(stepId);
            if (step == null) return null;
            return _mapper.Map<StepResponse>(step);
        }

        public async Task<IEnumerable<StepResponse>> GetByRecipeIdAsync(int recipeId)
        {
            var steps = await _repository.GetByRecipeIdAsync(recipeId);
            return _mapper.Map<IEnumerable<StepResponse>>(steps);
        }

        public async Task<CreateStepResponse> CreateAsync(CreateStepRequest request)
        {
            try
            {
                var step = _mapper.Map<Step>(request);
                var created = await _repository.CreateAsync(step);
                return new CreateStepResponse
                {
                    Success = true,
                    Data = _mapper.Map<StepResponse>(created),
                    Message = "Step created successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating step");
                return new CreateStepResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<UpdateStepResponse> UpdateAsync(UpdateStepRequest request)
        {
            try
            {
                var step = _mapper.Map<Step>(request);
                var updated = await _repository.UpdateAsync(step);
                if (updated == null)
                {
                    return new UpdateStepResponse
                    {
                        Success = false,
                        Message = "Step not found"
                    };
                }
                return new UpdateStepResponse
                {
                    Success = true,
                    Message = "Step updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating step");
                return new UpdateStepResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DeleteStepResponse> DeleteAsync(int stepId)
        {
            try
            {
                var result = await _repository.DeleteAsync(stepId);
                if (!result)
                {
                    return new DeleteStepResponse
                    {
                        Success = false,
                        Message = "Step not found"
                    };
                }
                return new DeleteStepResponse
                {
                    Success = true,
                    Message = "Step deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting step");
                return new DeleteStepResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
