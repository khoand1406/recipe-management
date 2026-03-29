using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.Constant;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models.ViewModels;
using System.Drawing.Printing;
using System.Net.Http.Headers;

namespace RecipeMgt.Views.Controllers
{
    public class ManagementController : Controller
    {
        private readonly IAdminClient _adminClient;

        public ManagementController(IAdminClient adminClient)
        {
            _adminClient = adminClient;
        }
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel();

            var dashboard = await _adminClient.GetDashboardMetrics();
            var categoryChart = await _adminClient.GetCategoryChart();


            if (dashboard.Success)
            {
                model.Metrics = dashboard.Data;
            }

            if (categoryChart.Success)
            {
                model.DishChart = categoryChart.Data;
            }

            return View(model);
        }

        public async Task<IActionResult> UserManagement(
    string? searchQuery,
    int? status,
    int page = 1,
    int pageSize = 10)
        {
            var result = await _adminClient.GetUsers(
                searchQuery,
                (UserStatus?)status,
                page,
                pageSize
            );

            if (!result.Success)
                return View("Error", result.Errors);

            var data = result.Data;

            var vm = new UsersViewModel
            {
                Users = data?.Items ?? Enumerable.Empty<UserResponseMgtDTO>(),
                Page = data?.Page ?? 1,
                PageSize= data?.PageSize ?? 10,
                TotalCount = data?.TotalItems ?? 0,
                PageCount = data?.TotalPages ?? 0,
                SearchQuery = searchQuery,
                StatusFilter = status
            };

            return View(vm);
        }
        [HttpPatch("/admin/users/{id}/ban")]
        public async Task<IActionResult> BanUser(int id)
        {
            var result = await _adminClient.BanUser(id);

            if(result.Success)
                return Ok(new { message = "User banned successfully" });
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        [HttpPatch("/admin/users/{id}/unban")]
        public async Task<IActionResult> UnBanUser(int id)
        {
            var result = await _adminClient.RecoverUser(id);

            if (result.Success)
                return Ok(new { message = "User unbanned successfully" });
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }


        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportCsv()
        {
            var dashboard = await _adminClient.GetDashboardMetrics();
            var chartData = await _adminClient.GetCategoryChart();
            var sb = new System.Text.StringBuilder();
            
            sb.Append('\uFEFF');
            
            sb.AppendLine("=== THÔNG SỐ TỔNG QUAN ===");
            sb.AppendLine("Thông số,Giá trị");
            if (dashboard.Success && dashboard.Data != null)
            {
                var m = dashboard.Data;
                sb.AppendLine($"Tổng món ăn,{m.TotalDish}");
                sb.AppendLine($"Tổng người dùng,{m.TotalUser}");
                sb.AppendLine($"Tổng công thức,{m.TotalRecipes}");
                sb.AppendLine($"Tổng đánh giá,{m.TotalReviews}");
            }
            sb.AppendLine();
            
            sb.AppendLine("=== BIỂU ĐỒ DANH MỤC ===");
            sb.AppendLine("STT,Danh mục,Số món ăn,Tỷ lệ (%)");
            if (chartData.Success && chartData.Data != null && chartData.Data.Any())
            {
                var sorted = chartData.Data.OrderByDescending(x => x.DishCount).ToList();
                int total = sorted.Sum(x => x.DishCount);
                for (int i = 0; i < sorted.Count; i++)
                {
                    var item = sorted[i];
                    double pct = total > 0 ? (double)item.DishCount / total * 100 : 0;
                    
                    string name = item.CategoryName.Contains(',')
                        ? $"\"{item.CategoryName}\""
                        : item.CategoryName;
                    sb.AppendLine($"{i + 1},{name},{item.DishCount},{pct:F2}");
                }
                sb.AppendLine($",TỔNG CỘNG,{total},100.00");
            }
            sb.AppendLine();
            sb.AppendLine($"Xuất lúc:,{DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine("Xuất bởi:,FoodAdmin System");
            string fileName = $"dashboard_report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "text/csv; charset=utf-8", fileName);
        }
    }
}

