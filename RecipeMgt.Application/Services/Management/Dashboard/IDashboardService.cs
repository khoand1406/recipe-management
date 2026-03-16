using RecipeMgt.Application.DTOs;
using RecipeMgt.Application.DTOs.Response.Management.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Management.Dashboard
{
    public interface IDashboardService
    {
        Task<Result<DashboardMetricResponse>> GetDashboardMetric();

        Task<Result<List<ChartCategoryDishResponse>>> GetChartCategory();

        Task<Result<List<ChartDishCreateResponse>>> GetChartMonthly();

        Task<Result<List<ChartTopRecipeRatingResponse>>> GetTopDishRatingResponse();
    }
}
