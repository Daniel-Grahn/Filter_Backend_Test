using filter_api_test.Models;

namespace filter_api_test.Repository
{
    public interface IFilterRepository
    {
        // Filter
        Task<Filter[]> GetFiltersAsync(string sourceId, int userId);
        Task<Filter> GetFilterByFieldNameAsync(string sourceId, int userId, string fieldName);

        Task AddFilterAsync(Filter filter);
        Task UpdateFilterAsync(Filter filter);

        // StoredFilter
        Task<StoredFilter[]> GetStoredFiltersAsync();
        Task<StoredFilter?> GetStoredFilterAsync(int id);
        Task AddStoredFilterAsync(StoredFilter sf);

        Task UpdateStoredFilterAsync(StoredFilter sf);

        // FilterComposition
        Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId);
    }
}
