using FilterAPI.Models;

namespace FilterAPI.Repository
{
    public interface IFilterRepository
    {
        // Filter
        Task<Filter[]> GetFiltersAsync(string sourceId, int userId);
        Task<Filter?> GetFilterByFieldNameAsync(string sourceId, int userId, string fieldName);

        Task AddFilterAsync(Filter filter); //We do not throw an ArgumentException if we try to add two filter that are identical
        Task UpdateFilterAsync(Filter filter);
        Task DeleteFilterAsync(Filter filter);

        // StoredFilter
        Task<StoredFilter[]> GetStoredFiltersAsync();
        Task<StoredFilter?> GetStoredFilterAsync(int id);
        Task AddStoredFilterAsync(StoredFilter sf); //We do not throw an ArgumentException if we try to add two filter that are identical

        Task UpdateStoredFilterAsync(StoredFilter sf);

        Task DeleteStoredFilterAsync(StoredFilter sf);

        // FilterComposition
        Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId);
    }
}
