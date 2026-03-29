using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using RecipeMgt.Views.Common.Config;
using RecipeMgt.Views.Common.Middleware;
using RecipeMgt.Views.Interface;
using RecipeMgt.Views.Services;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme= CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options=>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";
}).AddGoogle(options=>
{
    options.ClientId = builder.Configuration["Google:ClientID"] ?? throw new InvalidOperationException("Unable to load config");
    options.ClientSecret = builder.Configuration["Google:SecretKey"] ?? throw new InvalidOperationException("Unable to load config");
    options.CallbackPath = "/signin-google";

    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    
});

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddTransient<AuthHandler>();
builder.Services.AddHttpClient<IAuthClient, AuthClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;

builder.Services.AddHttpClient<IAdminClient, AdminClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;
builder.Services.AddHttpClient<IDashboardClient, DashboardClient>((serviceProvider, client)=>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;
builder.Services.AddHttpClient<IDishClient, DishClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;
builder.Services.AddHttpClient<IRecipeClient, RecipeClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;
builder.Services.AddHttpClient<IIngredientClient, IngredientClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;
builder.Services.AddHttpClient<IStepClient, StepClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;
builder.Services.AddHttpClient<ICommentClient, CommentClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>(); ;

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
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
