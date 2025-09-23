using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FilterAPI.Data;
using FilterAPI.Repositories;
using FilterAPI.Models;

namespace FilterAPI.Integration.Tests.Repositories
{
    public class FilterRepositoryTest
    {
        //Temporary Db
        private static FilterDb GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<FilterDb>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;
            return new FilterDb(options);
        }

        //private static FilterDb GetTestDb()
        //{
        //    var connectionString = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION")
        //        ?? "Server=localhost,1433;Database=FiltersDbTest;User Id=sa;Password=BuildingGroupGiraffe9182;TrustServerCertificate=true;";

        //    var options = new DbContextOptionsBuilder<FilterDb>()
        //        .UseSqlServer(connectionString)
        //        .Options;

        //    var context = new FilterDb(options);
        //    context.Database.EnsureCreated(); // creates tables if missing
        //    return context;
        //}

        //Filter
        [Fact]
        public async Task GetFiltersAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            Filter[] filters = [new Filter { SourceId = "p1", UserId = 1, FieldName = "catagory" },
                                new Filter { SourceId = "p1", UserId = 1, FieldName = "status" },
                                new Filter { SourceId = "p2", UserId = 1, FieldName = "catagory" }];

            //When Filters is empty
            Filter[] findfilters = await repo.GetFiltersAsync("p1", 1);
            Assert.Empty(findfilters);


            //When Filters has items
            foreach (Filter filter in filters)
            {
                await repo.AddFilterAsync(filter);
            }

            Filter[] foundfilter = await repo.GetFiltersAsync("p1", 1);
            Assert.NotEmpty(foundfilter);
            Assert.Equal(2, foundfilter.Length);


            Filter[] noFilters = await repo.GetFiltersAsync("p1", 0);
            Assert.Empty(noFilters);

        }




        [Fact]
        public async Task GetFilterByFieldNameAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            var filter = new Filter { SourceId = "p1", UserId = 1, FieldName = "catagory" };

            //There is no filter to get
            Filter noFilter = await repo.GetFilterByFieldNameAsync("p1", 1, "catagory");
            Assert.Null(noFilter);

            //Here we add the filter
            await repo.AddFilterAsync(filter);
            var foundFilter = await repo.GetFilterByFieldNameAsync("p1", 1, "catagory");
            Assert.NotNull(foundFilter);
            Assert.Equal(filter, foundFilter);
        }

        [Fact]
        public async Task AddFilterAsync() // Could be better (Fix this later)
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
            //add the filter
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            var filter = new Filter { SourceId = "1", UserId = 2, FieldName = "Field2" };
            await repo.AddFilterAsync(filter);

            //find the filter
            var foundFilter = await repo.GetFilterByFieldNameAsync(filter.SourceId, filter.UserId, filter.FieldName);
            Assert.Equal(filter, foundFilter);
            Assert.Null(foundFilter.Data);

            //Update the filter
            foundFilter.Data = ["C-1", "C-2"];
            await repo.UpdateFilterAsync(foundFilter);

            var updatedfoundFilter = await repo.GetFilterByFieldNameAsync(filter.SourceId, filter.UserId, filter.FieldName);
            Assert.NotNull(updatedfoundFilter.Data);
            Assert.Equal(["C-1", "C-2"], updatedfoundFilter.Data);
        }



        // StoredFilter
        [Fact]
        public async Task GetStoredFiltersAsyncTest()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);
            StoredFilter[] storedFilters = await repo.GetStoredFiltersAsync();
            Assert.Empty(storedFilters);


            // add a storde filter
            StoredFilter newStoredFilter = new()
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


            //add a second Stored Filter
            StoredFilter aSecondStoredFilter = new()
            {
                Id = 2,
                Title = "yourFilter",
                CompanyId = 22,
                UserId = 1,
                IsPersonal = true,
                SourceId = "1",
                CreatedAt = DateTime.Now,
            };

            await repo.AddStoredFilterAsync(aSecondStoredFilter);
            storedFilters = await repo.GetStoredFiltersAsync();
            Assert.Equal(2, storedFilters.Length);
        }

        [Fact]
        public async Task GetStoredFilterAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);

            //Before being added
            int storedFilterId = 1;
            StoredFilter newStoredFilter = new()
            {
                Id = storedFilterId,
                Title = "myFilter",
                CompanyId = 22,
                UserId = 1,
                IsPersonal = true,
                SourceId = "1",
                CreatedAt = DateTime.Now,
            };
            Assert.Null(await repo.GetStoredFilterAsync(storedFilterId));

            //Find the StoredFilter after being added
            await repo.AddStoredFilterAsync(newStoredFilter);
            StoredFilter? foundStoredFilter = await repo.GetStoredFilterAsync(storedFilterId);
            Assert.Equal(newStoredFilter, foundStoredFilter);

            //Find a StoredFilter that dose not excist
            StoredFilter? noStordeFilter = await repo.GetStoredFilterAsync(78);
            Assert.Null(noStordeFilter);
        }

        [Fact]
        public async Task AddStoredFilterAsync() //Fix this later
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);

            //Before being added
            int storedFilterId = 1;
            StoredFilter newStoredFilter = new()
            {
                Id = storedFilterId,
                Title = "myFilter",
                CompanyId = 22,
                UserId = 1,
                IsPersonal = true,
                SourceId = "1",
                CreatedAt = DateTime.Now,
            };

            StoredFilter[] getNoStoredFilter = await repo.GetStoredFiltersAsync();
            Assert.Empty(getNoStoredFilter);

            //check if there is only one
            await repo.AddStoredFilterAsync(newStoredFilter);
            StoredFilter[] storedFilter = await repo.GetStoredFiltersAsync();
            Assert.NotEmpty(storedFilter);
            Assert.Single(storedFilter);

            //add the same filter twice

            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await repo.AddStoredFilterAsync(newStoredFilter);
            //});

            //StoredFilter[] findstoredFilter = await repo.GetStoredFiltersAsync();
            //Assert.NotEmpty(findstoredFilter);
            //Assert.Single(findstoredFilter);

            //add a new Storde filter
            StoredFilter secondStoredFilter = new()
            {
                Id = 2,
                Title = "foo",
                CompanyId = 22,
                UserId = 1,
                IsPersonal = true,
                SourceId = "1",
                CreatedAt = DateTime.Now,
            };
            await repo.AddStoredFilterAsync(secondStoredFilter);
            StoredFilter[] foundStoredFilters = await repo.GetStoredFiltersAsync();
            Assert.Equal(2, foundStoredFilters.Length);
        }

        [Fact]
        public async Task UpdateStoredFilterAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);

            //add a storedFilter
            int storedFilterId = 1;
            StoredFilter newStoredFilter = new()
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
            StoredFilter? storedfilter = await repo.GetStoredFilterAsync(storedFilterId);
            Assert.NotNull(storedfilter);
            Assert.Equal("myFilter", storedfilter.Title);

            //After being updated
            storedfilter.Title = "YourFilter";
            await repo.UpdateStoredFilterAsync(storedfilter);

            StoredFilter? getStoredFilter = await repo.GetStoredFilterAsync(storedFilterId); ;
            Assert.NotNull(getStoredFilter);
            Assert.Equal("YourFilter", getStoredFilter.Title);
        }

        // FilterComposition
        [Fact]
        public async Task GetFilterCompositionsAsync()
        {
            var db = GetInMemoryDb();
            var repo = new FilterRepository(db);

            FilterComposition[] filterCompositionList = await repo.GetFilterCompositionsAsync(22, "1");

            Assert.Empty(filterCompositionList);

            // There is no way to test this properly
            // there is no add/get/update method for Filter Composition
        }

    }
}