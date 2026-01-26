using AutoMapper;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.Services.Cloudiary;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Repository.Recipes;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Recipes
{
    public class RecipeServices : IRecipeServices
    {
        private readonly IRecipeRepository _repository;
        private readonly IDishRepository _dishrepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly ICloudinaryService _cloudinaryservice;
        private readonly IMapper _mapper;
        private readonly ILogger<RecipeServices> _logger;

        public RecipeServices(IRecipeRepository repository, IMapper mapper, ILogger<RecipeServices> logger, IDishRepository dishRepository, ICloudinaryService service, IUserRepository userRepository, IUnitOfWork uow)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryservice = service;
            _logger = logger;
            _dishrepository = dishRepository;
            _userRepository = userRepository;
            _uow = uow;
        }

        public async Task<RecipeResponse?> GetRecipeById(int id)
        {
            var recipe = await _repository.getRecipeById(id);
            if (recipe == null) return null;
            var result = _mapper.Map<RecipeResponse>(recipe);
            return result;
        }


        public async Task<CreateRecipeResponse> CreateRecipes(CreateRecipeRequest request)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var dish = await _uow.Dishes.GetById(request.DishId);
                if (dish == null)
                {
                    _logger.LogError($"Error occurs when create recipe: Dish with id {request.DishId} not found");
                    return new CreateRecipeResponse { Success = false, Message = "Create Recipe Failed! Not found dish with Id: " + request.DishId };
                }
                var recipe = _mapper.Map<Recipe>(request);
                recipe.CreatedAt = DateTime.Now;
                recipe.UpdatedAt = DateTime.Now;
                await _uow.Recipes.AddAsync(recipe);
                await _uow.SaveChangesAsync();

                var ingredients = request.Ingredients.Select(i => { 
                    var ing = _mapper.Map<Ingredient>(i);
                    ing.RecipeId = recipe.RecipeId;
                    return ing;
                }).ToList();

                var steps = request.Steps.Select(i =>
                {
                    var step = _mapper.Map<Step>(i);
                    step.RecipeId = recipe.RecipeId;
                    return step;
                }).ToList();

                var images = request.ImageUrls != null && request.ImageUrls.Any() ? request.ImageUrls.Select(url => new Image
                {
                    Caption = url,
                    EntityId= recipe.RecipeId,
                    EntityType= "Recipe",
                    ImageUrl = url,
                    UploadedAt = DateTime.Now,
                }).ToList() : null ;

                await _uow.Recipes.AddRangeAsync(images);
                await _uow.Recipes.AddRangeAsync(steps);
                await _uow.Recipes.AddRangeAsync(ingredients);
                await _uow.CommitAsync();
                var author = await _uow.Users.getUserAsync(request.AuthorId);
                return new CreateRecipeResponse { Success = true, Message = "Create recipe successfully",
                    Data = new RecipeResponse
                    {
                        RecipeId = recipe.RecipeId,
                        Title = recipe.Title,
                        AuthorId = recipe.AuthorId,
                        CookingTime = recipe.CookingTime,
                        Description = recipe.Description,
                        DifficultyLevel = recipe.DifficultyLevel,
                        Servings = recipe.Servings,
                        CreatedAt = recipe.CreatedAt,
                        UpdatedAt = recipe.UpdatedAt,
                        Images = request.ImageUrls?? new List<string>(),
                        Author = author,

                    }
                };
            }catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "CreateRecipe failed");
                return new CreateRecipeResponse { Success = false, Message = "Create recipe failed" };
            }
        }
        public async Task<CreateRecipeResponse> CreateRecipe(CreateRecipeRequest request)
        {
            try
            {
                var dish = await _dishrepository.GetById(request.DishId);
                if (dish == null)
                {
                    _logger.LogError($"Error occurs when create recipe: Dish with id {request.DishId} not found");
                    return new CreateRecipeResponse { Success = false, Message = "Create Recipe Failed! Not found dish with Id: " + request.DishId };
                }
                var recipe = _mapper.Map<Recipe>(request);
                recipe.CreatedAt = DateTime.Now;
                recipe.UpdatedAt = DateTime.Now;


                var ingredients = request.Ingredients != null && request.Ingredients.Any()
                    ? _mapper.Map<List<Ingredient>>(request.Ingredients)
                    : new List<Ingredient>();

                var steps = request.Steps != null && request.Steps.Any()
                    ? _mapper.Map<List<Step>>(request.Steps)
                    : new List<Step>();

                var images = new List<Image>();
                if (request.ImageUrls != null && request.ImageUrls.Any())
                {
                    foreach (var img in request.ImageUrls)
                    {
                        var image= new Image { ImageUrl= img, UploadedAt= DateTime.Now, Caption= img, EntityType= "Recipe", EntityId=0 };
                        images.Add(image);
                    }
                }


                var result = await _repository.createRecipes(recipe, ingredients, steps, images);
                var authorInfo = await _userRepository.getUserAsync(request.AuthorId);
                var imageRecipes = await _repository.getRecipeImages(recipe.RecipeId);

                return new CreateRecipeResponse 
                { 
                    Success = true, 
                    Message = result.Message, 
                    Data = new RecipeResponse { 
                        RecipeId = recipe.RecipeId, 
                        Title = recipe.Title, 
                        AuthorId = recipe.AuthorId, 
                        CookingTime = recipe.CookingTime, 
                        Description = recipe.Description, 
                        DifficultyLevel = recipe.DifficultyLevel, 
                        Servings = recipe.Servings, 
                        CreatedAt = recipe.CreatedAt, 
                        UpdatedAt = recipe.UpdatedAt,
                        Images= imageRecipes,
                        Author= authorInfo,
                        
                    } 
                };


            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurs when creating recipe: " + ex.Message);
                return new CreateRecipeResponse { Success = false, Message = "Create Recipe Failed" };
            }
        }

        public async Task<DeleteRecipeResponse> DeleteRecipe(int id)
        {
            try
            {
                var found= await _repository.getRecipeById(id);
                if(found == null)
                {
                    _logger.LogError("Not found recipe with id:" + id);
                    return new DeleteRecipeResponse { Success = false, Message = "Not found recipe with id: " + id };
                }
                var result= await _repository.deleteRecipes(id);
                if (result)
                {
                    return new DeleteRecipeResponse { Success = true, Message = "Delete successfully" };
                }
                return new DeleteRecipeResponse { Success = false, Message = "Delete Failed" };

            }catch(Exception ex)
            {
                _logger.LogError($"Failed to delete recipe: {id}, {ex.Message}");
                return new DeleteRecipeResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<IEnumerable<RecipeResponse>> GetRecipesByDish(int id)
        {
            var listRecipes= await  _repository.GetRecipes(id);
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeResponse>> GetRecipesByFilter()
        {
            var listRecipes = await _repository.getRecipesByFilter();
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeWithUserInfo>> GetRecipesByUser(int userId)
        {
            var listRecipes = await _repository.GetRecipesByUser(userId);
            var result = _mapper.Map<IEnumerable<RecipeWithUserInfo>>(listRecipes);
            return result;
        }

        public async Task<IEnumerable<RecipeResponse>> GetRelatedRecipes(int id)
        {
            var listRecipes = await _repository.GetRecipesByUser(id);
            var result = _mapper.Map<IEnumerable<RecipeResponse>>(listRecipes);
            return result;
        }

        public async Task<Domain.RequestEntity.PagedResponse<Recipe>> GetSearchResult(Domain.RequestEntity.SearchRecipeRequest request)
        {
            return await _repository.GetSearchedResult(request);
        }

        public async Task<UpdateRecipeResponse> UpdateRecipe(UpdateRecipeRequest request)
        {
            try
            {
                
                var recipeUpdate = _mapper.Map<Recipe>(request);
                recipeUpdate.UpdatedAt = DateTime.Now;
                var ingredients = request.Ingredients != null && request.Ingredients.Any()
                    ? _mapper.Map<List<Ingredient>>(request.Ingredients)
                    : new List<Ingredient>();

                var steps = request.Steps != null && request.Steps.Any()
                    ? _mapper.Map<List<Step>>(request.Steps)
                    : new List<Step>();

                var images = new List<Image>();
                if (request.Images != null && request.Images.Any())
                {
                    foreach (var img in request.Images)
                    {
                        var uploadResult = await _cloudinaryservice.UploadImageAsync(img);
                        images.Add(new Image
                        {
                            EntityType = "Recipe",
                            ImageUrl = uploadResult,
                            Caption = Path.GetFileNameWithoutExtension(img.FileName),
                            UploadedAt = DateTime.Now
                        });
                    }
                }
                var result= await _repository.updateRecipes(recipeUpdate, ingredients, steps, images);
                if (result.Success)
                {
                    _logger.LogInformation("Successfully update recipe with id: " + request.RecipeId);
                    return new UpdateRecipeResponse { Success = true, Message = result.Message };

                }
                return new UpdateRecipeResponse { Success = false, };
            }
            catch(Exception ex)
            {
                _logger.LogError("Error while updating recipes: "+ ex.Message);
                return new UpdateRecipeResponse { Success = false, Message= "Update Recipe Failed!" };
            }
        }
    }
}
