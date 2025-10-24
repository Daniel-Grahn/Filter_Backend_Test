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
        Task<StoredFilter[]> GetStoredFiltersAsync(string sourceId);
        Task<IResult> AddOrUpdateStoredFilterAsync(StoredFilter sf);
        Task<IResult> DeleteStoredFilterAsync(int id);
        Task<FilterPosition[]> GetFilterPositionsAsync(int companyId, string sourceId);
        Task<IResult> UpdateFilterPositionAsync(FilterPosition fc);
        Task<DateRange?> GetDateRangeAsync(int userId, string sourceId);
        Task<IResult> AddOrUpdateDateRangeAsync(DateRange dr);
    }
}
