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
        public async Task<IResult> AddOrUpdateFilterAsync(FilterRequestDTO requestFilter)
        {
            //mapp requets to filter
            var inputFilter = _mapper.Map<Filter>(requestFilter);
            var existing = await _repo.GetFilterByFieldNameAsync(inputFilter.SourceId, inputFilter.UserId, inputFilter.FieldName);
            if (existing == null)
            {
                await _repo.AddFilterAsync(inputFilter);
                return TypedResults.Ok();
            }

            existing.Data = inputFilter.Data;
            await _repo.UpdateFilterAsync(existing);
            return TypedResults.Ok();
        }

        public Task<StoredFilter[]> GetStoredFiltersAsync() => _repo.GetStoredFiltersAsync();
        public async Task<IResult> AddOrUpdateStoredFilterAsync(int id, StoredFilterRequestDTO sf)
        {
            var existing = await _repo.GetStoredFilterAsync(id);
            if (existing == null)
            {
                var storedFilter = _mapper.Map<StoredFilter>(sf);
                await _repo.AddStoredFilterAsync(storedFilter);
                return TypedResults.Ok();
            }
            else
            {
                existing.Update(sf);
                await _repo.UpdateStoredFilterAsync(existing);
                return TypedResults.NoContent();
            }

        }
        public Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId) => _repo.GetFilterCompositionsAsync(companyId, sourceId);

        public async Task<IResult> ClearDataInFilters(string sourceId, int userId)
        {
            var filterList = await _repo.GetFiltersAsync(sourceId, userId);

            foreach (var filter in filterList)
            {
                filter.Data = [];
                await _repo.UpdateFilterAsync(filter);
            }
            
            return TypedResults.Ok();
        }

    }
}
