using AutoMapper;
using filter_api_test;
using filter_api_test.Data;
using filter_api_test.DTOs;
using filter_api_test.Endpoints;
using filter_api_test.Models;
using filter_api_test.Repositories;
using filter_api_test.Repository;
using filter_api_test.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//test
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

builder.Services.AddCors();
builder.Services.AddDbContext<FilterDb>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IFilterRepository, FilterRepository>();
builder.Services.AddScoped<IFilterService, FilterService>();

var app = builder.Build();


app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Filter API V1");
    });

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FilterDb>();
    dbContext.Database.Migrate();
    
}

// Map endpoints
app.MapFilterEndpoints();

app.Run();

public partial class Program { }