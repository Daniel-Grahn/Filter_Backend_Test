using AutoMapper;
using FilterAPI.DTOs;
using FilterAPI.Helpers;
using FilterAPI.Models;
using FilterAPI.Services;
using Microsoft.AspNetCore.Mvc;
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
            filterGroup.MapGet("/page/{sourceId}", async (string sourceId, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var results = await service.GetFiltersAsync(sourceId, claimValues.GetValueOrDefault("userId"));

                List<FilterResponseDTO> response = [];
                foreach (var item in results)
                {
                    response.Add(mapper.Map<FilterResponseDTO>(item));
                }

                var responseArray = response.ToArray();

                return responseArray.Length > 0 ? Results.Ok(responseArray) : Results.NoContent();
            }).RequireAuthorization();

            //FilterRequestDTO
            filterGroup.MapPut("", async (FilterRequestDTO filter, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                Filter inputFilter = mapper.Map<Filter>(filter);
                inputFilter.UserId = claimValues.GetValueOrDefault("userId");

                var result = await service.AddOrUpdateFilterAsync(inputFilter);
                return result;
            }).RequireAuthorization();

            filterGroup.MapPut("/bulksave", async ([FromBody] FilterRequestDTO[] filters, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                List<Filter> inputFilters = new();
                foreach (var filter in filters)
                {
                    inputFilters.Add(mapper.Map<Filter>(filter));
                    inputFilters.Last().UserId = claimValues.GetValueOrDefault("userId");
                }

                var result = await service.BulkUpdateFilterAsync(inputFilters);
                return result;
            }).RequireAuthorization();

            filterGroup.MapDelete("/page/{sourceId}/clear", async (string sourceId, IFilterService service, ClaimsPrincipal claims) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var results = await service.ClearUserFiltersBySource(sourceId, claimValues.GetValueOrDefault("userId"));
                return results;
            }).RequireAuthorization();

            
            filterGroup.MapGet("/page/{sourceId}/getstoredfilters", async (string sourceId, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var results = await service.GetStoredFiltersAsync(sourceId);

                List<StoredFilterResponseDTO> response = [];
                foreach (var item in results)
                {
                    response.Add(mapper.Map<StoredFilterResponseDTO>(item));
                }

                var responseArray = response.ToArray();

                return responseArray.Length > 0 ? Results.Ok(responseArray) : Results.NoContent();
            }).RequireAuthorization();

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

            filterGroup.MapGet("/page/{sourceId}/getposition", async (string sourceId, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var results = await service.GetFilterPositionsAsync(claimValues.GetValueOrDefault("companyId"), sourceId);

                List<FilterPositionResponseDTO> response = [];
                foreach (var item in results)
                {
                    response.Add(mapper.Map<FilterPositionResponseDTO>(item));                   
                }

                var responseArray = response.ToArray();

                return responseArray.Length > 0 ? Results.Ok(responseArray) : Results.NoContent();
            }).RequireAuthorization();

            filterGroup.MapPut("/updateposition", async (FilterPositionRequestDTO fp, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                FilterPosition inputFilterPosition = mapper.Map<FilterPosition>(fp);
                inputFilterPosition.CompanyId = claimValues.GetValueOrDefault("companyId");

                var results = await service.UpdateFilterPositionAsync(inputFilterPosition);
                return Results.Ok(results);
            }).RequireAuthorization();

            filterGroup.MapPut("/page/{sourceId}/putdate", async (string sourceId, DateRangeRequestDTO dr, ClaimsPrincipal claims, IFilterService service, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var inputRange = mapper.Map<DateRange>(dr);
                inputRange.UserId = claimValues.GetValueOrDefault("userId");
                inputRange.SourceId = sourceId;
                var result = await service.AddOrUpdateDateRangeAsync(inputRange);
                return result;
            }).RequireAuthorization();

            filterGroup.MapGet("/page/{sourceId}/getdate", async (string sourceId, IFilterService service, ClaimsPrincipal claims, IMapper mapper) =>
            {
                var claimValues = ClaimConverterHelper.FindValues(claims);
                var result = await service.GetDateRangeAsync(claimValues.GetValueOrDefault("userId"), sourceId);
                var response = mapper.Map<DateRangeResponseDTO>(result);
                return response != null ? Results.Ok(response) : Results.NoContent();
            }).RequireAuthorization();

        }       
    }
}

