using FilterAPI.Data;
using FilterAPI.DTOs;
using FilterAPI.Endpoints;
using FilterAPI.Models;
using FilterAPI.Repositories;
using FilterAPI.Repository;
using FilterAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<FilterRequestDTO, Filter>().ForMember(dest => dest.UserId, opt => opt.Ignore());

    cfg.CreateMap<Filter, FilterResponseDTO>();
    cfg.CreateMap<StoredFilterRequestDTO, StoredFilter>().ForMember(dest => dest.CompanyId, opt => opt.Ignore()).ForMember(dest => dest.UserId, opt => opt.Ignore());
    cfg.CreateMap<StoredFilter, StoredFilterResponseDTO>();
    cfg.CreateMap<FilterCompositionRequestDTO, FilterComposition>();
    cfg.CreateMap<FilterComposition, FilterCompositionResponseDTO>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Filter API", Description = ".", Version = "v1" });
    // Add JWT Bearer token support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowToj", policy =>
    //{
    //    policy.WithOrigins("toj.informer.se")
    //    .AllowAnyHeader()
    //    .AllowAnyMethod()
    //    .AllowCredentials();
    //});
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "tojSystem",
        ValidAudience = "filter-api",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super-secret-shared-key-super-secret-shared-key")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromMinutes(5),
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"Challenge: {context.Error} - {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<FilterDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IFilterRepository, FilterRepository>();
builder.Services.AddScoped<IFilterService, FilterService>();

var app = builder.Build();
//app.UseCors("AllowToj");
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FilterDb>();

    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not migrate database: {ex.Message}");
    }
}

// Map endpoints
app.MapFilterEndpoints();

app.Run();

public partial class Program { }