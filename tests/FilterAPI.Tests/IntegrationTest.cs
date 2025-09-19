using Xunit;
using AutoMapper;
using FilterAPI.Data;
using Microsoft.EntityFrameworkCore;
using FilterAPI.Repositories;
using FilterAPI.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.Design;
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
        public async Task GetFiltersAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            Filter[] filters = [new Filter { SourceId = "p1", UserId = 1, FieldName = "catagory" },
                                new Filter { SourceId = "p1", UserId = 1, FieldName = "status" },
                                new Filter { SourceId = "p2", UserId = 1, FieldName = "catagory" }];

            Filter[] findfilters = await repo.GetFiltersAsync("p1", 1);
            Assert.Empty(findfilters);

            foreach (Filter filter in filters) { 
                await repo.AddFilterAsync(filter);
            }

            Filter[] foundfilter = await repo.GetFiltersAsync("p1", 1);
            Assert.NotEmpty(foundfilter);
            Assert.Equal(2, foundfilter.Length);
        }




        [Fact]
        public async Task GetFilterByFieldNameAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            var filter = new Filter { SourceId = "p1", UserId = 1, FieldName = "catagory" };
            //await Assert.ThrowsAsync<ArgumentException>(async () => await repo.GetFilterByFieldNameAsync("p1", 1, "catagory"));
            //Here we add the filter
            await repo.AddFilterAsync(filter);
            var foundFilter = await repo.GetFilterByFieldNameAsync("p1", 1, "catagory");
            Assert.NotNull(foundFilter);
            Assert.Equal(filter, foundFilter);
        }

        [Fact]
        public async Task AddFilterAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            var filter = new Filter { SourceId = "p1", UserId = 1, FieldName = "catagory" };

            //Here we look if the list is empty
            var noFilters = await repo.GetFiltersAsync("p1", 1);
            Assert.Empty(noFilters);



            //Here we add the filter the first time
            await repo.AddFilterAsync(filter);
            var oneFilter = await repo.GetFiltersAsync("p1", 1);
            Assert.NotNull(oneFilter);
            Assert.Single(oneFilter);



            //Here we add a new filter
            var aSecondFilter = new Filter { SourceId = "p1", UserId = 1, FieldName = "status" };
            await repo.AddFilterAsync(aSecondFilter);
            var filters = await repo.GetFiltersAsync("p1", 1);
            Assert.NotNull(filters);
            Assert.Equal(2, filters.Length);


            //Here we add the filter a second time
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await repo.AddFilterAsync(filter);
            });
            var justFilter = await repo.GetFiltersAsync("p1", 1);
            Assert.Equal(2, justFilter.Length);
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

        [Fact]
        public async Task UpdateStoredFilterAsync()
        {
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
            await repo.AddStoredFilterAsync(newStoredFilter);

            //Before being uppdated
            StoredFilter storedfilter = await repo.GetStoredFilterAsync(storedFilterId);
            Assert.NotNull(storedfilter);
            Assert.Equal("myFilter", storedfilter.Title);

            //After being updated
            storedfilter.Title = "YourFilter";
            await repo.UpdateStoredFilterAsync(storedfilter);

            Assert.Equal("YourFilter", storedfilter.Title);
        }

        // FilterComposition
        [Fact]
        public async Task GetFilterCompositionsAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);

            FilterComposition[] filterCompositionList = await repo.GetFilterCompositionsAsync(22, "1");

            Assert.Empty(filterCompositionList);

            //There is no way to test this properly
        }

    }
}