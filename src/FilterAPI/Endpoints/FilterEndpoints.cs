using FilterAPI.DTOs;
using FilterAPI.Services;
using System.Security.Claims;

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

            filterGroup.MapGet("/getsavedfilters", async (IFilterService service, ClaimsPrincipal claims) =>
            {
                var results = await service.GetStoredFiltersAsync();
                var test = claims;
                return Results.Ok(results + $"Auth: {claims.Identity?.IsAuthenticated}");
            }).RequireAuthorization();

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

            filterGroup.MapGet("/whoami", (HttpContext context) =>
            {
                var user = context.User;
                return user.Identity?.IsAuthenticated == true
                    ? Results.Ok(new
                    {
                        Name = user.Identity.Name,
                        Claims = user.Claims.Select(c => new { c.Type, c.Value })
                    })
                    : Results.Unauthorized();
            }).RequireAuthorization(); // 👈 this line is crucial


        }

        
    }
}

