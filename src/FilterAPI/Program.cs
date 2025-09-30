using FilterAPI.Data;
using FilterAPI.DTOs;
using FilterAPI.Endpoints;
using FilterAPI.Models;
using FilterAPI.Repositories;
using FilterAPI.Repository;
using FilterAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<FilterRequestDTO, Filter>();
    cfg.CreateMap<Filter, FilterResponseDTO>();
    cfg.CreateMap<StoredFilterRequestDTO, StoredFilter>();
    cfg.CreateMap<StoredFilter, StoredFilterResponseDTO>();
    cfg.CreateMap<FilterCompositionRequestDTO, FilterComposition>();
    cfg.CreateMap<FilterComposition, FilterCompositionResponseDTO>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Filter API", Description = ".", Version = "v1" });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowToj", policy =>
//    {
//        policy.WithOrigins("toj.informer.se")
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//        .AllowCredentials();
//    });
//});

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "tojSystem",
        ValidAudience = "filter-api",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super-secret-shared-key")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest },
        SignatureValidator = (token, parameters) =>
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt;
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
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Filter API V1");
    });

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