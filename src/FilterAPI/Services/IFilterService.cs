using FilterAPI.DTOs;
using FilterAPI.Models;

namespace FilterAPI.Services
{
    public interface IFilterService
    {
        Task<Filter[]> GetFiltersAsync(string sourceId, int userId);
        Task<Filter> AddOrUpdateFilterAsync(FilterRequestDTO filter);
        Task<StoredFilter[]> GetStoredFiltersAsync();
        Task<IResult> AddOrUpdateStoredFilterAsync(int id, StoredFilterRequestDTO sf);
        Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId);

        Task<IResult> ClearDataInFilters(string sourceId, int userId);

        //UpdateDataInAllFilters
        Task<IResult> UpdateDataInAllFilters(string sourceId, int userId);

    }
}
