using Xunit;
using AutoMapper;
using FilterAPI.Data;
using Microsoft.EntityFrameworkCore;
using FilterAPI.Repositories;
using FilterAPI.Models;
namespace FilterApi.Test
{
    public class FilterRepositoryTest
    {
        //Temporary Db
        private static FilterDb GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<FilterDb>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            return new FilterDb(options);
        }
        //Filter
        [Fact]
        public async Task GetFilterAsyncTest()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            var filter = new Filter { SourceId = "S1", UserId = 1, FieldName = "Field1" };
            //await Assert.ThrowsAsync<ArgumentException>(async () => await repo.GetFilterByFieldNameAsync("S1", 1, "Field1"));
            //Here we add the filter
            await repo.AddFilterAsync(filter);
            var foundFilter = await repo.GetFilterByFieldNameAsync("S1", 1, "Field1");
            Assert.NotNull(foundFilter);
            Assert.Equal(filter, foundFilter);
        }
        [Fact]
        public async Task UpdateFilterAsyncTest()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            var filter = new Filter { SourceId = "1", UserId = 2, FieldName = "Field2" };
            await repo.AddFilterAsync(filter);

            var foundFilter = await repo.GetFilterByFieldNameAsync(filter.SourceId, filter.UserId, filter.FieldName);
            Assert.Equal(filter, foundFilter);
            Assert.Null(foundFilter.Data);
            foundFilter.Data = ["C-1", "C-2"];
            await repo.UpdateFilterAsync(foundFilter);

            var updatedfoundFilter = await repo.GetFilterByFieldNameAsync(filter.SourceId, filter.UserId, filter.FieldName);
            Assert.NotNull(updatedfoundFilter.Data);
            Assert.Equal(["C-1", "C-2"], updatedfoundFilter.Data);
        }
        // StoredFilter
        [Fact]
        public async Task GetStoredFiltersAsyncTest() // Not Done yet (Remeber: it is a list, check for more possible outcomes)
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            StoredFilter[] storedFilters = await repo.GetStoredFiltersAsync();
            Assert.Empty(storedFilters);
            StoredFilter newStoredFilter = new StoredFilter
            {
                Id = 1,
                Title = "myFilter",
                CompanyId = 22,
                UserId = 1,
                IsPersonal = true,
                SourceId = "1",
                CreatedAt = DateTime.Now,
            };
            await repo.AddStoredFilterAsync(newStoredFilter);
            storedFilters = await repo.GetStoredFiltersAsync();
            Assert.Single(storedFilters);
            Assert.Equal("myFilter", storedFilters[0].Title);
        }

        [Fact]
        public async Task GetStoredFilterAsync() { 
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);


            int storedFilterId = 1;
            StoredFilter newStoredFilter = new StoredFilter
            {
                Id = storedFilterId,
                Title = "myFilter",
                CompanyId = 22,
                UserId = 1,
                IsPersonal = true,
                SourceId = "1",
                CreatedAt = DateTime.Now,
            };

            //Before being added
            Assert.Null(await repo.GetStoredFilterAsync(storedFilterId));


            await repo.AddStoredFilterAsync(newStoredFilter);

            //Find the StoredFilter after being added
            StoredFilter foundStoredFilter = await repo.GetStoredFilterAsync(storedFilterId);
            Assert.Equal(newStoredFilter, foundStoredFilter);


            //Find a StoredFilter that dose not excist
            Assert.Null(await repo.GetStoredFilterAsync(78));


        }

    }
}