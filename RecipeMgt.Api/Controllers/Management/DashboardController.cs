using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RecipeMgt.Api.Common;
using RecipeMgt.Application.Constant;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Application.Services.Management.Dashboard;
using RecipeMgt.Application.Services.Management.User;

namespace RecipeMgt.Api.Controllers.Management
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IUserManagementService _userManagementService;
        public DashboardController(IDashboardService dashboardService, IUserManagementService userManagement)
        {
            _dashboardService = dashboardService;
            _userManagementService = userManagement;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashBoardStatisatic()
        {
            var dashboardMetrics = await _dashboardService.GetDashboardMetric();
            return Ok(ApiResponseFactory.Success(dashboardMetrics.Value, HttpContext));
        }

        [HttpGet("category-chart")]
        public async Task<IActionResult> GetCategoryChart()
        {
            var chartData = await _dashboardService.GetChartCategory();
            return Ok(ApiResponseFactory.Success(chartData.Value, HttpContext));
        }


        [HttpGet("monthly-chart")]
        public async Task<IActionResult> GetMonthlyChart()
        {
            var monthlyChartData = await _dashboardService.GetChartMonthly();
            return Ok(ApiResponseFactory.Success(monthlyChartData.Value, HttpContext));
        }

        [HttpGet("top-rating-chart")]
        public async Task<IActionResult> GetTopRatingChart()
        {
            var topRatingData = await _dashboardService.GetTopDishRatingResponse();
            return Ok(ApiResponseFactory.Success(topRatingData.Value, HttpContext));    
        }

        

    }
}
