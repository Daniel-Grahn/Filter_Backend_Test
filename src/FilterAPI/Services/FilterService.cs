using AutoMapper;
using FilterAPI.DTOs;
using FilterAPI.Models;
using FilterAPI.Repository;

namespace FilterAPI.Services
{
    public class FilterService : IFilterService

    {
        private readonly IMapper _mapper;
        private readonly IFilterRepository _repo;
        public FilterService(IFilterRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public Task<Filter[]> GetFiltersAsync(string sourceId, int userId) => _repo.GetFiltersAsync(sourceId, userId);
        public async Task<IResult> AddOrUpdateFilterAsync(Filter filter)
        {
            var existing = await _repo.GetFilterByFieldNameAsync(filter.SourceId, filter.UserId, filter.FieldName);
            if (existing == null)
            {
                await _repo.AddFilterAsync(filter);
                return TypedResults.NoContent();
            }

            existing.Data = filter.Data;
            await _repo.UpdateFilterAsync(existing);
            return TypedResults.NoContent();
        }

        public async Task<IResult> BulkUpdateFilterAsync(IEnumerable<Filter> inputFilters)
        {
            var firstFilter = inputFilters.First();
            var existingFilters = await _repo.GetFiltersAsync(firstFilter.SourceId, firstFilter.UserId);
            var listOfFilters = existingFilters.ToList();
            foreach (var filter in inputFilters)
            {
                var existing = await _repo.GetFilterByFieldNameAsync(filter.SourceId, filter.UserId, filter.FieldName);
                if (existing == null)
                {
                    await _repo.AddFilterAsync(filter);
                }
                else
                {
                    listOfFilters.Remove(existing);
                    existing.Data = filter.Data;
                    await _repo.UpdateFilterAsync(existing);
                }
            }

            foreach (var filter in listOfFilters)
            {
                await _repo.DeleteFilterAsync(filter);
            }

            return TypedResults.NoContent();
        }

        public async Task<IResult> ClearUserFiltersBySource(string sourceId, int userId)
        {
            var filterList = await _repo.GetFiltersAsync(sourceId, userId);

            foreach (var filter in filterList)
            {
                await _repo.DeleteFilterAsync(filter);
            }

            return TypedResults.NoContent();
        }

        public Task<StoredFilter[]> GetStoredFiltersAsync() => _repo.GetStoredFiltersAsync();
        public async Task<IResult> AddOrUpdateStoredFilterAsync(StoredFilter sf)
        {
            var existing = await _repo.GetStoredFilterAsync(sf.Id);
            if (existing == null)
            {
                sf.Id = 0;
                await _repo.AddStoredFilterAsync(sf);
                return TypedResults.NoContent();
            }
            else
            {
                existing.Update(sf);
                await _repo.UpdateStoredFilterAsync(existing);
                return TypedResults.NoContent();
            }
        }

        public async Task<IResult> DeleteStoredFilterAsync(int id)
        {
            var existing = await _repo.GetStoredFilterAsync(id);
            if (existing == null)
            {
                return TypedResults.NotFound();
            }
            else
            {
                await _repo.DeleteStoredFilterAsync(existing);
                return TypedResults.NoContent();
            }
        }
        public Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId) => _repo.GetFilterCompositionsAsync(companyId, sourceId);

        public async Task<IResult> UpdateFilterCompositionAsync(FilterComposition fc)
        {
            var existing = await _repo.GetFilterCompositionAsync(fc.Id);
            if (existing != null)
            {
                existing.Update(fc);
                await _repo.UpdateFilterCompositionAsync(existing);
                return TypedResults.NoContent();
            }
            else
            {
                return TypedResults.NotFound();
            }
        }
    }
}
