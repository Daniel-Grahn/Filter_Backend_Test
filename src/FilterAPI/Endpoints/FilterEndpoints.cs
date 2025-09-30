using AutoMapper;
using FilterAPI.DTOs;
using FilterAPI.Helpers;
using FilterAPI.Models;
using FilterAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FilterAPI.Endpoints
{
    public static class FilterEndpoints
    {

        public static void MapFilterEndpoints(this WebApplication app)
        {
            var filterGroup = app.MapGroup("/filteritems");

            ///<summary>
            ///
            ///</summary>
            filterGroup.MapGet("/page/{sourceId}/", async (string sourceId, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var results = await service.GetFiltersAsync(sourceId, claimValues.GetValueOrDefault("userId"));

                List<FilterResponseDTO> response = [];
                foreach (var item in results)
                {
                    response.Add(mapper.Map<FilterResponseDTO>(item));
                }

                var responseArray = response.ToArray();

                return responseArray.Length > 0 ? Results.Ok(responseArray) : Results.NotFound();
            }).RequireAuthorization();

            //FilterRequestDTO
            filterGroup.MapPut("/", async (FilterRequestDTO filter, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                Filter inputFilter = mapper.Map<Filter>(filter);
                inputFilter.UserId = claimValues.GetValueOrDefault("userId");

                var result = await service.AddOrUpdateFilterAsync(inputFilter);
                return result;
            }).RequireAuthorization();

            filterGroup.MapGet("/getstoredfilters", async (IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var results = await service.GetStoredFiltersAsync();

                List<StoredFilterResponseDTO> response = [];
                foreach (var item in results)
                {
                    response.Add(mapper.Map<StoredFilterResponseDTO>(item));
                }

                var responseArray = response.ToArray();

                return responseArray.Length > 0 ? Results.Ok(responseArray) : Results.NotFound();
            }).RequireAuthorization();

            //StoredFilterRequestDTO
            filterGroup.MapPut("/addstoredfilter", async (StoredFilterRequestDTO sf, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                StoredFilter inputSf = mapper.Map<StoredFilter>(sf);
                var values = ClaimConverterHelper.FindValues(claims);

                inputSf.CompanyId = values.GetValueOrDefault("companyId");
                inputSf.UserId = values.GetValueOrDefault("userId");
                
                var result = await service.AddOrUpdateStoredFilterAsync(inputSf);
                return result;
            }).RequireAuthorization();

            filterGroup.MapDelete("/deletefilter/{id}", async (int id, IFilterService service) =>
            {
                var result = await service.DeleteStoredFilterAsync(id);
                return result;
            }).RequireAuthorization();

            filterGroup.MapGet("/page/{sourceId}/getcomposition/", async (string sourceId, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var results = await service.GetFilterCompositionsAsync(claimValues.GetValueOrDefault("companyId"), sourceId);

                List<FilterCompositionResponseDTO> response = [];
                foreach (var item in results)
                {
                    response.Add(mapper.Map<FilterCompositionResponseDTO>(item));                   
                }

                var responseArray = response.ToArray();

                return responseArray.Length > 0 ? Results.Ok(responseArray) : Results.NotFound();
            }).RequireAuthorization();
            
            filterGroup.MapDelete("/page/{sourceId}/clear", async (string sourceId, IFilterService service, ClaimsPrincipal claims) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var results = await service.ClearUserFiltersBySource(sourceId, claimValues.GetValueOrDefault("userId"));
                return results;
            }).RequireAuthorization();





            // ----------- TEST FUNCTIONS ----------------
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
            }).RequireAuthorization(); 

            filterGroup.MapGet("/debug-token", (HttpContext context) =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader?.StartsWith("Bearer ") == true)
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var handler = new JwtSecurityTokenHandler();

                    try
                    {
                        var jwtToken = handler.ReadJwtToken(token);
                        return Results.Ok(new
                        {
                            claims = jwtToken.Claims.Select(c => new { c.Type, c.Value }),
                            issuer = jwtToken.Issuer,
                            audience = jwtToken.Audiences,
                            validTo = jwtToken.ValidTo,
                            signatureAlgorithm = jwtToken.SignatureAlgorithm
                        });
                    }
                    catch (Exception ex)
                    {
                        return Results.BadRequest($"Token parsing failed: {ex.Message}");
                    }
                }
                return Results.BadRequest("No token provided");
            }).AllowAnonymous();


        }

        
    }
}

