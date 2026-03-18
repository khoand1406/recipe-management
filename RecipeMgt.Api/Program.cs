using CloudinaryDotNet;
using dotenv.net;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecipeMgt.Api.Common;
using RecipeMgt.Api.Common.Middleware;
using RecipeMgt.Application;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Request.Recipes;
using RecipeMgt.Application.Validator.Auth;
using RecipeMgt.Application.Validator.Csv;
using RecipeMgt.Application.Validator.Rating;
using RecipentMgt.Infrastucture;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OpenRecipe API", Version = "v1" });

            // Add JWT authentication support
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
        });
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<RegistetRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateDishRequest>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateDishRequest>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateRecipeRequest>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateRecipeRequest>();
        builder.Services.AddValidatorsFromAssemblyContaining<AddRatingValidation>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCsvDtoValidator>();


        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
.AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
    };
});

        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "127.0.0.1:6379";
            options.InstanceName = "RecipeMgt:";
        });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                        ? "INVALID_FIELD"
                        : e.ErrorMessage)
                    .ToList();

                var response = ApiResponseFactory.Fail(
                    "VALIDATION_ERROR",
                    context.HttpContext,
                    errors
                );

                return new BadRequestObjectResult(response);
            };
        });
        builder.Services.AddLogging();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseMiddleware<ErrorHandlingMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseMiddleware<UserContextMiddleware>();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}