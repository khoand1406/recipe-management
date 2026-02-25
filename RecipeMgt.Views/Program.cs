using Microsoft.Extensions.Options;
using RecipeMgt.Views.Common.Config;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Services;
using System.Runtime.Intrinsics.Arm;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient<IAuthClient, AuthClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});
builder.Services.AddHttpClient<IDashboardClient, DashboardClient>((serviceProvider, client)=>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});
builder.Services.AddHttpClient<IDishClient, DishClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});
builder.Services.AddHttpClient<IRecipeClient, RecipeClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});
builder.Services.AddHttpClient<IIngredientClient, IngredientClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});
builder.Services.AddHttpClient<IStepClient, StepClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});
builder.Services.AddHttpClient<ICommentClient, CommentClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
