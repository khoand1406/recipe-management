using RecipeMgt.Application.DTOs.Response.Management.Dashboard;
using RecipeMgt.Domain.RequestEntity;



namespace RecipeMgt.Views.Models.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardMetricResponse? Metrics { get; set; }

        

        public List<ChartCategoryDishResponse>? DishChart { get; set; }
    }
}
