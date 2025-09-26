using FilterAPI.DTOs;
using FilterAPI.Services;

namespace FilterAPI.Endpoints
{
    public static class FilterEndpoints
    {
        public static void MapFilterEndpoints(this WebApplication app)
        {
            var filterGroup = app.MapGroup("/filteritems");

            filterGroup.MapGet("/page/{sourceId}/user/{userId}", async (string sourceId, int userId, IFilterService service) =>
            {
                var results = await service.GetFiltersAsync(sourceId, userId);
                return results.Length > 0 ? Results.Ok(results) : Results.NotFound();
            });

            //FilterRequestDTO
            filterGroup.MapPut("/", async (FilterRequestDTO filter, IFilterService service) =>
            {
                var updated = await service.AddOrUpdateFilterAsync(filter);
                return Results.Ok(updated);
            });

            filterGroup.MapGet("/getsavedfilters", async (IFilterService service) =>
            {
                var results = await service.GetStoredFiltersAsync();
                return Results.Ok(results);
            });

            //StoredFilterRequestDTO
            filterGroup.MapPut("/savefilter/{id}", async (int id, StoredFilterRequestDTO sf, IFilterService service) =>
            {
                await service.AddOrUpdateStoredFilterAsync(id, sf);
                return Results.NoContent();
            });

            filterGroup.MapDelete("/deletefilter/{id}", async (int id, IFilterService service) =>
            {
                await service.DeleteStoredFilterAsync(id);
                return Results.NoContent();
            });

            filterGroup.MapGet("/getcomposition/company/{companyId}/page/{sourceId}", async (int companyId, string sourceId, IFilterService service) =>
            {
                var results = await service.GetFilterCompositionsAsync(companyId, sourceId);
                return results.Length > 0 ? Results.Ok(results) : Results.NotFound();
            });
            
            //--------------Clear Field (return a empty array)------------------
            filterGroup.MapPut("/page/{sourceId}/user/{userId}/clear", async (string sourceId, int userId, IFilterService service) =>
            {
                var results = await service.ClearDataInFilters(sourceId, userId);
                return Results.Ok(results);
            });


        }

        
    }
}

