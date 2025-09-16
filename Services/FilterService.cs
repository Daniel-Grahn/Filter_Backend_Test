using AutoMapper;
using filter_api_test.DTOs;
using filter_api_test.Models;
using filter_api_test.Repository;
using Microsoft.EntityFrameworkCore.Metadata;

namespace filter_api_test.Services
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

        public Task<Filter[]> GetFiltersAsync(string sourceId) => _repo.GetFiltersBySourceAsync(sourceId);

        public async Task<Filter> AddOrUpdateFilterAsync(FilterRequestDTO requestFilter)
        {
            //mapp requets to filter
            var inputFilter = _mapper.Map<Filter>(requestFilter);
            var existing = await _repo.GetFilterAsync(inputFilter.SourceId, inputFilter.UserId, inputFilter.FieldName);
            if (existing == null)
            {
                await _repo.AddFilterAsync(inputFilter);
                return inputFilter;
            }

            existing.Data = inputFilter.Data;
            await _repo.UpdateFilterAsync(existing);
            return existing;
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

    }
}
