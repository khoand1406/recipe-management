using Microsoft.AspNetCore.Mvc;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Enums;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Models.ViewModels;
using RecipeMgt.Views.Services;
using System.Drawing.Printing;
using System.Net.Http.Headers;
using System.Text;

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

            var statistics= await _adminClient.GetUserStatistics();



            var vm = new UsersViewModel
            {
                Users = data?.Items ?? Enumerable.Empty<UserResponseMgtDTO>(),
                ActiveCount = statistics?.Data?.ActiveCount ?? 0,
                PendingCount = statistics?.Data?.PendingCount ?? 0,
                BannedCount = statistics?.Data?.BannedCount ?? 0,
                RecipeCount= statistics?.Data?.RecipeCount?? 0,
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

        [HttpPost("/admin/users/create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            var result = await _adminClient.CreateUser(model);
            if (result.Success)
                return Ok(new { message = "User created successfully" });
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        [HttpDelete("/admin/users/{id}/delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminClient.DeleteUser(id);
            if (result.Success)
                return Ok(new { message = "User deleted successfully" });
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        [HttpPut("/admin/users/{id}/update")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            var result = await _adminClient.UpdateUser(id, model);
            if (result.Success)
                return Ok(new { message = "User updated successfully" });
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }
        [HttpPost("/admin/users/upload-csv")]
        public async Task<IActionResult> UploadUsersFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });
            var result = await _adminClient.CreateUsersFromCsv(file);
            if (!result.Success)
                return BadRequest(new { message = result.Message, errors = result.Errors });

            return Ok(new
            {
                data = result.Data,
                message = result?.Data?.SuccessCount == 0 ? "IMPORT_FAILED"
                         : result?.Data?.FailedCount > 0 ? "IMPORT_PARTIAL_SUCCESS"
                         : "IMPORT_SUCCESS"
            });
        }


        [HttpGet("/admin/users/export")]
    public async Task<IActionResult> ExportUsers([FromQuery] UserQueryRequest query)
    {
        // ❗ Bỏ pagination để export full
        query.Page = 1;
        query.PageSize = int.MaxValue;

        var result = await _adminClient.GetUsers(query.SearchQuery, (UserStatus?)query.Status,query.Page, query.PageSize);

        if (!result.Success || result.Data == null)
            return BadRequest("Cannot export users");

        var users = result.Data.Items;

        var csv = new StringBuilder();

        // Header
        csv.AppendLine("Id,FullName,Email,Role,Status,Recipes,Followers,CreatedAt");

        foreach (var u in users)
        {
            csv.AppendLine(
                $"{u.UserId}," +
                $"{Escape(u.FullName)}," +
                $"{Escape(u.Email)}," +
                $"{Escape(u.RoleName)}," +
                $"{u.UserStatus}," +
                $"{u.TotalRecipes}," +
                $"{u.TotalFollowers}," +
                $"{(u.CreatedAt.HasValue ? u.CreatedAt.Value.ToString("dd/MM/yyyy") : "")}"
            );
        }

        // ✅ Fix lỗi tiếng Việt Excel
        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
            .ToArray();

        var fileName = $"users_{DateTime.Now:yyyyMMddHHmmss}.csv";

        return File(bytes, "text/csv", fileName);
    }
        public class UserQueryRequest
        {
            public string? SearchQuery { get; set; }
            public int? Status { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
        private string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
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

