using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FilterAPI.Data;
using FilterAPI.Services;
using FilterAPI.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using FilterAPI.Integration.Tests.Data;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using FilterAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FilterAPI.Integration.Tests.Services
{
    //[Collection("Sequential")] // Ser till att tester körs seriellt
    public class FilterServiceTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
    {
        private readonly TestDatabaseFixture _fixture;

        public FilterServiceTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        // Reset database before each test
        public async Task InitializeAsync() => await _fixture.ResetDatabaseAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        private (IFilterService service, FilterDb db) GetServices()
        {
            var scope = _fixture.Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IFilterService>();
            var db = scope.ServiceProvider.GetRequiredService<FilterDb>();
            return (service, db);
        }

        [Fact]
        public async Task GetFiltersAsync_Empty()
        {
            var (filterService, _) = GetServices();

            var filters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Empty(filters);
        }

        [Fact]
        public async Task GetFiltersAsync_Find()
        {
            var (filterService, _) = GetServices();

            var filters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Empty(filters);

            var filter = new Filter
            {
                FieldName = "myFilter",
                UserId = 999,
                SourceId = "nonexistent"
            };

            await filterService.AddOrUpdateFilterAsync(filter);

            filters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Single(filters);
            Assert.Equal("myFilter", filters[0].FieldName);
        }

        [Fact]
        public async Task AddOrUpdateFilterAsync_Add()
        {
            var (filterService, _) = GetServices();

            var filter = new Filter
            {
                FieldName = "name",
                UserId = 1,
                SourceId = "test"
            };


            var filterList = await filterService.GetFiltersAsync("test", 1);
            Assert.Empty(filterList);

            await filterService.AddOrUpdateFilterAsync(filter);
            filterList = await filterService.GetFiltersAsync("test", 1);
            Assert.Single(filterList);

            var savedFilter = filterList.First();
            Assert.Equal("name", savedFilter.FieldName);
            Assert.Equal(1, savedFilter.UserId);
            Assert.Equal("test", savedFilter.SourceId);
        }

        [Fact]
        public async Task AddOrUpdateFilterAsync_AddMultiple()
        {
            var (filterService, _) = GetServices();

            Filter[] filters = [
                new() {FieldName = "status", UserId = 1, SourceId = "test"},
                new() {FieldName = "category", UserId = 1, SourceId = "test"},
                new() {FieldName = "status", UserId = 1, SourceId = "develop"}
            ];

            var filterList = await filterService.GetFiltersAsync("test", 1);
            Assert.Empty(filterList);

            foreach (var filter in filters)
            {
                await filterService.AddOrUpdateFilterAsync(filter);
            }

            filterList = await filterService.GetFiltersAsync("test", 1);
            Assert.Equal(2, filterList.Length);

            foreach (var filter in filterList)
            {
                Assert.Equal("test", filter.SourceId);
                Assert.Equal(1, filter.UserId);
            }
        }

        [Fact]
        public async Task AddOrUpdateFilterAsync_AddSameFilterTwice()
        {
            var (filterService, _) = GetServices();

            Filter filter = new() { FieldName = "status", UserId = 1, SourceId = "test" };
            await filterService.AddOrUpdateFilterAsync(filter);
            var theFilter = await filterService.GetFiltersAsync("test", 1);
            Assert.Single(theFilter);



            await filterService.AddOrUpdateFilterAsync(filter);
            theFilter = await filterService.GetFiltersAsync("test", 1);
            Assert.Single(theFilter);
        }

        [Fact]
        public async Task AddOrUpdateFilterAsync_UpdateFilter()
        {
            var (filterService, _) = GetServices();

            Filter filter = new() { FieldName = "status", UserId = 1, SourceId = "test", Data = ["c-1"] };
            await filterService.AddOrUpdateFilterAsync(filter);
            var Filters = await filterService.GetFiltersAsync("test", 1);
            Assert.Single(Filters);

            var theFilter = Filters[0];
            Assert.Equal("status", theFilter.FieldName);
            Assert.Equal(1, theFilter.UserId);
            Assert.Equal("test", theFilter.SourceId);
            Assert.NotNull(theFilter.Data);
            Assert.Equal(["c-1"], theFilter.Data);

            Filter updateFilter = new() { FieldName = "status", UserId = 1, SourceId = "test", Data = ["c-1", "c-2"] };
            await filterService.AddOrUpdateFilterAsync(updateFilter);
            Filters = await filterService.GetFiltersAsync("test", 1);
            Assert.Single(Filters);

            theFilter = Filters[0];
            Assert.Equal("status", theFilter.FieldName);
            Assert.Equal(1, theFilter.UserId);
            Assert.Equal("test", theFilter.SourceId);
            Assert.NotNull(theFilter.Data);
            Assert.Equal(["c-1", "c-2"], theFilter.Data);
        }

        [Fact]
        public async Task GetStoredFiltersAsync_Empty()
        {
            var (filterService, _) = GetServices();
            var StoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Empty(StoredFilters);
        }

        [Fact]
        public async Task GetStoredFiltersAsync_Find()
        {
            var (filterService, _) = GetServices();
            var storedFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Empty(storedFilters);

            StoredFilter storedFilter = new() { Title = "foo", CreatedAt = DateTime.UtcNow, CompanyId = 22, UserId = 1, SourceId = "1" }; //Title could be null but is not allowed in DB

            await filterService.AddOrUpdateStoredFilterAsync(storedFilter);

            storedFilters = await filterService.GetStoredFiltersAsync(storedFilter.SourceId);
            Assert.Single(storedFilters);

            var theStoredFilter = storedFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.Equal(22, theStoredFilter.CompanyId);
            Assert.Equal("1", theStoredFilter.SourceId);
        }

        [Fact]
        public async Task AddOrUpdateStoredFilterAsync_AddSimple()
        {
            var (filterService, _) = GetServices();
            var nostoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Empty(nostoredFilters);

            StoredFilter storedFilter = new() { Title = "foo", CreatedAt = DateTime.UtcNow, SourceId = "1" };
            await filterService.AddOrUpdateStoredFilterAsync(storedFilter);

            var allStoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Single(allStoredFilters);

            var theStoredFilter = allStoredFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.Equal("1", theStoredFilter.SourceId);
        }

        [Fact]
        public async Task AddOrUpdateStoredFilterAsync_Update()
        {
            var (filterService, _) = GetServices();
            var CreatedAt = DateTime.UtcNow;
            var UpdatedAt = DateTime.UtcNow;


            StoredFilter storedFilter = new() { Title = "foo", CreatedAt = CreatedAt, CompanyId = 21, UserId = 1, SourceId = "1", IsPersonal = true };
            await filterService.AddOrUpdateStoredFilterAsync(storedFilter);

            var allStoredFilters = await filterService.GetStoredFiltersAsync(storedFilter.SourceId);
            Assert.Single(allStoredFilters);

            var theStoredFilter = allStoredFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.True(theStoredFilter.IsPersonal);

            storedFilter.IsPersonal = false; 
            await filterService.AddOrUpdateStoredFilterAsync(storedFilter);

            allStoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Single(allStoredFilters);

            theStoredFilter = allStoredFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.False(theStoredFilter.IsPersonal);
        }

        [Fact]
        public async Task DeleteStoredFilterAsync_Excisting()
        {
            var (filterService, _) = GetServices();
            StoredFilter[] storedFilters = [
                new() { Title = "foo",  CreatedAt = DateTime.UtcNow, CompanyId = 21, UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "bar", CreatedAt = DateTime.UtcNow, CompanyId = 22,  UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "keke", CreatedAt = DateTime.UtcNow, CompanyId = 23, UserId = 1, SourceId = "1", IsPersonal = true },
            ];

            for (int i = 0; i < storedFilters.Length; i++)
            {
                await filterService.AddOrUpdateStoredFilterAsync(storedFilters[i]);
            }

            StoredFilter[] foundStoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Equal(3, foundStoredFilters.Length);
            var lastStoredFilter = foundStoredFilters[foundStoredFilters.Length - 1];
            Assert.Equal("keke", lastStoredFilter.Title);
            Assert.Equal(23, lastStoredFilter.CompanyId);

            await filterService.DeleteStoredFilterAsync(lastStoredFilter.Id);
            foundStoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Equal(2, foundStoredFilters.Length);

            lastStoredFilter = foundStoredFilters[foundStoredFilters.Length - 1];
            Assert.Equal("bar", lastStoredFilter.Title);
            Assert.Equal(22, lastStoredFilter.CompanyId);

        }

        [Fact]
        public async Task DeleteStoredFilterAsync_NotExcisting()
        {
            var (filterService, _) = GetServices();
            StoredFilter[] storedFilters = [
                new() { Title = "foo", CreatedAt = DateTime.UtcNow, CompanyId = 21, UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "bar", CreatedAt = DateTime.UtcNow, CompanyId = 22, UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "keke", CreatedAt = DateTime.UtcNow, CompanyId = 23, UserId = 1, SourceId = "1", IsPersonal = true },
            ];

            for (int i = 0; i < storedFilters.Length; i++)
            {
                await filterService.AddOrUpdateStoredFilterAsync(storedFilters[i]);
            }

            StoredFilter[] foundStoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Equal(3, foundStoredFilters.Length);


            await filterService.DeleteStoredFilterAsync(99);
            foundStoredFilters = await filterService.GetStoredFiltersAsync("1");
            Assert.Equal(3, foundStoredFilters.Length);
        }


        [Fact]
        public async Task ClearUserFiltersBySource_WithoutData() 
        {
            var (filterService, _) = GetServices();
            Filter filter = new()
            {
                FieldName = "myFilter",
                UserId = 999,
                SourceId = "nonexistent",
                Data = null
            };
            await filterService.AddOrUpdateFilterAsync(filter);

            var allFilters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Single(allFilters);

            var theFilter = allFilters[0];
            Assert.Equal("myFilter", theFilter.FieldName);
            Assert.Null(theFilter.Data);

            await filterService.ClearUserFiltersBySource("nonexistent", 999);

            allFilters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Empty(allFilters);
        }

        [Fact]
        public async Task ClearUserFiltersBySource_WithEmptyList()
        {
            var (filterService, _) = GetServices();
            Filter filter = new()
            {
                FieldName = "myFilter",
                UserId = 999,
                SourceId = "nonexistent",
                Data = []
            };
            await filterService.AddOrUpdateFilterAsync(filter);

            var allFilters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Single(allFilters);

            var theFilter = allFilters[0];
            Assert.Equal("myFilter", theFilter.FieldName);
            Assert.NotNull(theFilter.Data);

            await filterService.ClearUserFiltersBySource("nonexistent", 999);
            allFilters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Empty(allFilters);
        }


        [Fact]
        public async Task ClearUserFiltersBySource_WithData()
        {

            var (filterService, _) = GetServices();
            Filter[] filters = [
                new(){ FieldName = "foo", UserId = 1, SourceId = "nonexistent", Data = ["f1"]},
                new(){ FieldName = "bar", UserId = 999, SourceId = "nonexistent", Data = ["b1", "b2"]}
            ];

            foreach (var filter in filters)
            {
                await filterService.AddOrUpdateFilterAsync(filter);
            }

            var allFilters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Single(allFilters);
            var theFilter = allFilters[0];
            Assert.Equal("bar", theFilter.FieldName);
            Assert.NotNull(theFilter.Data);
            Assert.Equal(["b1", "b2"], theFilter.Data);

            allFilters = await filterService.GetFiltersAsync("nonexistent", 1);
            Assert.Single(allFilters);
            theFilter = allFilters[0];
            Assert.Equal("foo", theFilter.FieldName);
            Assert.NotNull(theFilter.Data);
            Assert.Equal(["f1"], theFilter.Data);

            //Should only remove the users filter by source
            await filterService.ClearUserFiltersBySource("nonexistent", 999);

            var user999Filters = await filterService.GetFiltersAsync("nonexistent", 999);
            var user1Filters = await filterService.GetFiltersAsync("nonexistent", 1);

            //The filter does no longer exsist
            Assert.Empty(user999Filters);

            var user1Filter = Assert.Single(user1Filters);
            Assert.Equal("foo", user1Filter.FieldName);
            Assert.NotNull(user1Filter.Data);
            Assert.Equal(["f1"], user1Filter.Data);
        }

        [Fact]
        public async Task ClearUserFiltersBySource_WithNoFilter()
        {
            var (filterService, _) = GetServices();
            Filter[] allFilters = await filterService.GetFiltersAsync("nonexistent", 1);
            Assert.Empty(allFilters);

            await filterService.ClearUserFiltersBySource("nonexistent", 1);

            allFilters = await filterService.GetFiltersAsync("nonexistent", 1);
            Assert.Empty(allFilters);
        }

        //No test for GetFilterCompositionsAsync
    }
}