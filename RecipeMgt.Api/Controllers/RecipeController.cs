using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Api.Common;
using RecipeMgt.Api.Common.Extension;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.Services.Bookmarks;
using RecipeMgt.Application.Services.Cloudiary;
using RecipeMgt.Application.Services.Comments;
using RecipeMgt.Application.Services.Recipes;
using RecipeMgt.Application.Services.Statistics.Recipe;
using RecipeMgt.Application.Services.Statistics.User;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeServices _recipeService;
    private readonly ICommentServices _commentService;
    private readonly IBookmarkService _bookmarkService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IStatisticService _statisticService;
    private readonly IUserStatisticService _userStatisticService;
    private readonly IValidator<CreateRecipeRequest> _createValidator;
    private readonly IValidator<UpdateRecipeRequest> _updateValidator;

    public RecipeController(
        IRecipeServices recipeService,
        ICommentServices commentService,
        IBookmarkService bookmarkService,
        ICloudinaryService cloudinaryService,
        IStatisticService statisticService,
        IUserStatisticService userStatisticService,
        IValidator<CreateRecipeRequest> createValidator,
        IValidator<UpdateRecipeRequest> updateValidator)
    {
        _recipeService = recipeService;
        _commentService = commentService;
        _bookmarkService = bookmarkService;
        _cloudinaryService = cloudinaryService;
        _statisticService = statisticService;
        _userStatisticService = userStatisticService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _recipeService.GetRecipeById(id);
        await _statisticService.RecipeViewd(id);
        return Ok(ApiResponseFactory.Success(result, HttpContext));
    }

    [HttpGet("dish/{dishId:int}")]
    public async Task<IActionResult> GetByDish(int dishId)
    {
        var result = await _recipeService.GetRecipesByDish(dishId);
        return Ok(ApiResponseFactory.Success(result, HttpContext));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyRecipes()
    {
        var userId = HttpContext.GetUserId();
        var result = await _recipeService.GetRecipesByUser(userId);
        return Ok(ApiResponseFactory.Success(result, HttpContext));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchRecipeRequest request)
    {
        var result = await _recipeService.GetSearchResult(request);
        return Ok(ApiResponseFactory.Success(result, HttpContext));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecipeRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        request.AuthorId = HttpContext.GetUserId();

        var result = await _recipeService.CreateRecipeAsync(request);
        await _userStatisticService.UserCreatedRecipe(request.AuthorId);
        
        return Ok(ApiResponseFactory.Success(result, HttpContext));
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRecipeRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var userId = HttpContext.GetUserId();
        await _recipeService.UpdateRecipeAsync(request, userId);

        return Ok(ApiResponseFactory.Success("Update recipe successfully", HttpContext));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _recipeService.DeleteRecipe(id);
        return Ok(ApiResponseFactory.Success("Delete successfully", HttpContext));
    }

    [Authorize]
    [HttpPost("upload-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ValidationException("File is required");

        var url = await _cloudinaryService.UploadImageAsync(file);
        return Ok(ApiResponseFactory.Success(new { imageUrl = url }, HttpContext));
    }

    [Authorize]
    [HttpPost("{recipeId:int}/comment")]
    public async Task<IActionResult> AddComment(int recipeId, [FromBody] string content)
    {
        var userId = HttpContext.GetUserId();
        await _commentService.AddCommentAsync(userId, recipeId, content);
        await _statisticService.RecipeComment(recipeId, userId);
        
        
        return Ok(ApiResponseFactory.Success("Comment added", HttpContext));
    }

    [Authorize]
    [HttpPost("{recipeId:int}/bookmark")]
    public async Task<IActionResult> Bookmark(int recipeId)
    {
        var userId = HttpContext.GetUserId();
        await _bookmarkService.AddBookmarkAsync(userId, recipeId);
        await _statisticService.RecipeBookmark(recipeId);
        return Ok(ApiResponseFactory.Success("Bookmarked", HttpContext));
    }

    [HttpGet("{recipeId:int}/comments")]
    public async Task<IActionResult> GetComments(int recipeId)
    {
        var result = await _commentService.GetCommentsAsync(recipeId);
        return Ok(ApiResponseFactory.Success(result, HttpContext));
    }
}
