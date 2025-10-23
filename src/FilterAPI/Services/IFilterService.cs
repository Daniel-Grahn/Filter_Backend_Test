using FilterAPI.DTOs;
using FilterAPI.Models;

namespace FilterAPI.Services
{
    public interface IFilterService
    {
        Task<Filter[]> GetFiltersAsync(string sourceId, int userId);
        Task<IResult> AddOrUpdateFilterAsync(Filter filter);
        Task<IResult> BulkUpdateFilterAsync(IEnumerable<Filter> inputFilters);
        Task<IResult> ClearUserFiltersBySource(string sourceId, int userId);
        Task<StoredFilter[]> GetStoredFiltersAsync();
        Task<IResult> AddOrUpdateStoredFilterAsync(StoredFilter sf);
        Task<IResult> DeleteStoredFilterAsync(int id);
        Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId);
        Task<IResult> UpdateFilterCompositionAsync(FilterComposition fc);
        Task<DateRange?> GetDateRangeAsync(int userId, string sourceId);
        Task<IResult> AddOrUpdateDateRangeAsync(DateRange dr);
    }
}
