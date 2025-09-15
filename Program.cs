using filter_api_test;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Filter API", Description = ".", Version = "v1" });
});

builder.Services.AddCors();
builder.Services.AddDbContext<FilterDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Filter API V1");
    });

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FilterDb>();
        dbContext.Database.Migrate();
    }
}

var filterItems = app.MapGroup("/filteritems");

filterItems.MapGet("/page/{sourceId}", GetAllFilters);
//filterItems.MapGet("/page/{sourceId}", GetFilterByUserId);
filterItems.MapPut("/", UpdateCurrentFilter);

filterItems.MapGet("/getsavedfilters", GetAllSavedFilters);
filterItems.MapPut("/savefilter/{id}", UpdateSavedFilter);

filterItems.MapGet("/getcomposition/company/{companyId}/page/{sourceId}", GetFilterCompositionsFromSource);
//filterItems.MapGet("/getcomposition/company/{companyId}", GetAllFilterCompositions);


static async Task<IResult> GetAllFilters(string sourceId, FilterDb db)
{
    var results = await db.Filter
        .Where(f => f.SourceId == sourceId)
        .ToArrayAsync();

    return results.Length > 0
        ? TypedResults.Ok(results)
        : TypedResults.NotFound();
}

//static async Task<IResult> GetFilterByUserId(string sourceId, int userId, string fieldName, FilterDb db)
//{
//    return await db.Filter.FindAsync(sourceId, userId, fieldName) is Filter filter ? TypedResults.Ok(filter) : TypedResults.NotFound();
//}

static async Task<IResult> UpdateCurrentFilter(Filter filter, FilterDb db)
{
    var foundFilter = await db.Filter.FindAsync(filter.SourceId, filter.UserId, filter.FieldName);

    if (foundFilter is null)
    {
        db.Filter.Add(filter);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/filteritems/{filter.Id}", filter);
    }
    else
    {
        foundFilter.Data = filter.Data;
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}

static async Task<IResult> UpdateSavedFilter(int id, StoredFilterInput sf, FilterDb db)
{
    var foundSavedFilter = await db.StoredFilter.FindAsync(id);

    if(foundSavedFilter is null)
    {
        var sfDTO = new StoredFilter
        {
            Id = id,
            Title = sf.Title,
            CompanyId = sf.CompanyId,
            UserId = sf.UserId,
            IsPersonal = sf.IsPersonal,
            SourceId = sf.SourceId,
            CreatedAt = DateTime.UtcNow,
            Filters = sf.Filters,
        };

        db.StoredFilter.Add(sfDTO);
    }
    else
    {
        foundSavedFilter.Filters = sf.Filters;
        foundSavedFilter.Title = sf.Title;
        foundSavedFilter.IsPersonal = sf.IsPersonal;
        foundSavedFilter.UpdatedAt = DateTime.Now;
    }

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> GetAllSavedFilters(FilterDb db)
{
    return TypedResults.Ok(await db.StoredFilter.ToArrayAsync());
}

static async Task<IResult> GetFilterCompositionsFromSource(int companyId, string sourceId, FilterDb db)
{
    var results = await db.FilterComposition
        .Where(fc => fc.CompanyId == companyId && fc.SourceId == sourceId)
        .ToArrayAsync();

    return results.Length > 0
        ? TypedResults.Ok(results)
        : TypedResults.NotFound();

}

//static async Task<IResult> GetAllFilterCompositions(int companyId, FilterDb db) {

//    var results = await db.FilterComposition
//        .Where(fc => fc.CompanyId == companyId)
//        .ToArrayAsync();

//    return results.Length > 0 
//        ? TypedResults.Ok(results) 
//        : TypedResults.NotFound();
//}

app.Run();
