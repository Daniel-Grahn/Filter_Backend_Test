using FilterAPI.Data;
using FilterAPI.Models;
using FilterAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace FilterAPI.Repositories
{
    public class FilterRepository : IFilterRepository
    {
        private readonly FilterDb _db;
        public FilterRepository(FilterDb db) => _db = db;

        // Filter methods
        public async Task<Filter[]> GetFiltersAsync(string sourceId, int userId) => await _db.Filter.Where(f => f.SourceId == sourceId && f.UserId == userId).ToArrayAsync();

        public async Task<Filter> GetFilterByFieldNameAsync(string sourceId, int userId, string fieldName) => await _db.Filter.FindAsync(sourceId, userId, fieldName);

        public async Task AddFilterAsync(Filter filter)
        {
            _db.Filter.Add(filter);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateFilterAsync(Filter filter)
        {
            _db.Filter.Update(filter);
            await _db.SaveChangesAsync();
        }

        // StoredFilter methods
        // Will eventually get all filters based on source and user
        public async Task<StoredFilter[]> GetStoredFiltersAsync() =>
            await _db.StoredFilter.ToArrayAsync();

        // Will probably not be needed
        public async Task<StoredFilter?> GetStoredFilterAsync(int id) =>
            await _db.StoredFilter.FindAsync(id);

        public async Task AddStoredFilterAsync(StoredFilter sf)
        {
            _db.StoredFilter.Add(sf);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateStoredFilterAsync(StoredFilter sf)
        {
            _db.StoredFilter.Update(sf);    
            await _db.SaveChangesAsync();   
        }

        public async Task DeleteStoredFilterAsync(StoredFilter sf)
        {
            _db.StoredFilter.Remove(sf);
            await _db.SaveChangesAsync();
        }

        // FilterComposition methods
        public async Task<FilterComposition[]> GetFilterCompositionsAsync(int companyId, string sourceId) =>
            await _db.FilterComposition
                .Where(fc => fc.CompanyId == companyId && fc.SourceId == sourceId)
                .ToArrayAsync();
    }
}

