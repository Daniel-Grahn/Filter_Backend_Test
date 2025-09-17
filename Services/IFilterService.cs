using filter_api_test.DTOs;
using filter_api_test.Models;

namespace filter_api_test.Services
{
    public interface IFilterService
    {
        Task<Filter> AddOrUpdateFilterAsync(FilterRequestDTO filter);
        Task<StoredFilter[]> GetStoredFiltersAsync();
        Task<IResult> AddOrUpdateStoredFilterAsync(int id, StoredFilterRequestDTO sf);
        Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId);
    }
}
