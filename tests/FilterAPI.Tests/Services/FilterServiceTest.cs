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

            var filter = new FilterRequestDTO
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

            var filter = new FilterRequestDTO
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

            FilterRequestDTO[] filters = [
                new() {FieldName = "status", UserId = 1, SourceId = "test"},
                new() {FieldName = "category", UserId = 1, SourceId = "test"},
                new() {FieldName = "status", UserId = 1, SourceId = "develop"}];

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

            FilterRequestDTO filter = new() { FieldName = "status", UserId = 1, SourceId = "test" };
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

            FilterRequestDTO filter = new() { FieldName = "status", UserId = 1, SourceId = "test", Data = ["c-1"] };
            await filterService.AddOrUpdateFilterAsync(filter);
            var Filters = await filterService.GetFiltersAsync("test", 1);
            Assert.Single(Filters);

            var theFilter = Filters[0];
            Assert.Equal("status", theFilter.FieldName);
            Assert.Equal(1, theFilter.UserId);
            Assert.Equal("test", theFilter.SourceId);
            Assert.NotNull(theFilter.Data);
            Assert.Equal(["c-1"], theFilter.Data);

            FilterRequestDTO updateFilter = new() { FieldName = "status", UserId = 1, SourceId = "test", Data = ["c-1", "c-2"] };
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
            var StoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Empty(StoredFilters);
        }

        [Fact]
        public async Task GetStoredFiltersAsync_Find()
        {
            var (filterService, _) = GetServices();
            var storedFilters = await filterService.GetStoredFiltersAsync();
            Assert.Empty(storedFilters);

            StoredFilterRequestDTO storedFileter = new() { Title = "foo", CompanyId = 22, UserId = 1, SourceId = "1" }; //Title could be null but is not allowed in DB

            await filterService.AddOrUpdateStoredFilterAsync(1, storedFileter);

            storedFilters = await filterService.GetStoredFiltersAsync();
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
            var nostoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Empty(nostoredFilters);

            StoredFilterRequestDTO storedFilter = new() { Title = "foo", SourceId = "1" };
            await filterService.AddOrUpdateStoredFilterAsync(1, storedFilter);

            var allStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Single(allStoredFilters);

            var theStoredFilter = allStoredFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.Equal("1", theStoredFilter.SourceId);
        }

        [Fact]
        public async Task AddOrUpdateStoredFilterAsync_AddMultple()
        {
            var (filterService, _) = GetServices();
            var nostoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Empty(nostoredFilters);

            StoredFilterRequestDTO[] storedFilters = [
                new() { Title="foo", CompanyId = 21, UserId=1, SourceId="1"},
                new() { Title="bar", CompanyId = 22, UserId=1, SourceId="3"},
                new() { Title="ten", CompanyId = 23, UserId=1, SourceId="5"}
            ];

            for (int i = 0; i < storedFilters.Length; i++)
            {
                await filterService.AddOrUpdateStoredFilterAsync(i + 1, storedFilters[i]);
            }

            var allStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Equal(3, allStoredFilters.Length);

            var theStoredFilter = allStoredFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.Equal(21, theStoredFilter.CompanyId);
            Assert.Equal("1", theStoredFilter.SourceId);
        }

        [Fact]
        public async Task AddOrUpdateStoredFilterAsync_Uppdate()
        {
            var (filterService, _) = GetServices();
            StoredFilterRequestDTO storedFilter = new() { Title = "foo", CompanyId = 21, UserId = 1, SourceId = "1", IsPersonal = true };
            await filterService.AddOrUpdateStoredFilterAsync(1, storedFilter);

            var allStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Single(allStoredFilters);

            var theStoredFilter = allStoredFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.True(theStoredFilter.IsPersonal);

            storedFilter = new() { Title = "foo", CompanyId = 21, UserId = 1, SourceId = "1", IsPersonal = false };
            await filterService.AddOrUpdateStoredFilterAsync(1, storedFilter);

            allStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Single(allStoredFilters);

            theStoredFilter = allStoredFilters[0];
            Assert.Equal("foo", theStoredFilter.Title);
            Assert.False(theStoredFilter.IsPersonal);
        }

        [Fact]
        public async Task DeleteStoredFilterAsync_Excisting()
        {
            var (filterService, _) = GetServices();
            StoredFilterRequestDTO[] storedFilters = [
                new() { Title = "foo", CompanyId = 21, UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "bar", CompanyId = 22, UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "keke", CompanyId = 23, UserId = 1, SourceId = "1", IsPersonal = true },
            ];

            for (int i = 0; i < storedFilters.Length; i++)
            {
                await filterService.AddOrUpdateStoredFilterAsync(i + 1, storedFilters[i]);
            }
            
            StoredFilter[] foundStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Equal(3, foundStoredFilters.Length);
            var lastStoredFilter = foundStoredFilters[foundStoredFilters.Length - 1];
            Assert.Equal("keke", lastStoredFilter.Title);
            Assert.Equal(23, lastStoredFilter.CompanyId);

            await filterService.DeleteStoredFilterAsync(lastStoredFilter.Id);
            foundStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Equal(2, foundStoredFilters.Length);

            lastStoredFilter = foundStoredFilters[foundStoredFilters.Length - 1];
            Assert.Equal("bar", lastStoredFilter.Title);
            Assert.Equal(22, lastStoredFilter.CompanyId);

        }

        [Fact]
        public async Task DeleteStoredFilterAsync_NotExcisting()
        {
            var (filterService, _) = GetServices();
            StoredFilterRequestDTO[] storedFilters = [
                new() { Title = "foo", CompanyId = 21, UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "bar", CompanyId = 22, UserId = 1, SourceId = "1", IsPersonal = true },
                new() { Title = "keke", CompanyId = 23, UserId = 1, SourceId = "1", IsPersonal = true },
            ];

            for (int i = 0; i < storedFilters.Length; i++)
            {
                await filterService.AddOrUpdateStoredFilterAsync(i + 1, storedFilters[i]);
            }

            StoredFilter[] foundStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Equal(3, foundStoredFilters.Length);
          

            await filterService.DeleteStoredFilterAsync(99);
            foundStoredFilters = await filterService.GetStoredFiltersAsync();
            Assert.Equal(3, foundStoredFilters.Length);
        }

        //No test for GetFilterCompositionsAsync

        [Fact]
        public async Task ClearDataInFilters_WithoutData()
        {
            var (filterService, _) = GetServices();
            FilterRequestDTO filter = new()
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
            Assert.NotNull(theFilter.Data);

            await filterService.ClearDataInFilters("nonexistent", 999);

            allFilters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Single(allFilters);

            theFilter = allFilters[0];
            Assert.Equal("myFilter", theFilter.FieldName);
            Assert.NotNull(theFilter.Data);
        }


        [Fact]
        public async Task ClearDataInFilters_WithData()
        {

            var (filterService, _) = GetServices();
            FilterRequestDTO[] filters = [
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

            await filterService.ClearDataInFilters("nonexistent", 999);

            var user999Filters = await filterService.GetFiltersAsync("nonexistent", 999);
            var user1Filters = await filterService.GetFiltersAsync("nonexistent", 1);

            var user999Filter = Assert.Single(user999Filters);
            Assert.Equal("bar", user999Filter.FieldName);
            Assert.NotNull(user999Filter.Data);
            Assert.Empty(user999Filter.Data);

            var user1Filter = Assert.Single(user1Filters);
            Assert.Equal("foo", user1Filter.FieldName);
            Assert.NotNull(user1Filter.Data);
            Assert.Equal(["f1"], user1Filter.Data);
        }

        [Fact]
        public async Task ClearDataInFilters_WithNoFilter()
        {
            var (filterService, _) = GetServices();
            Filter[] allFilters = await filterService.GetFiltersAsync("nonexistent", 1);
            Assert.Empty(allFilters);

            await filterService.ClearDataInFilters("nonexistent", 999);

            allFilters = await filterService.GetFiltersAsync("nonexistent", 1);
            Assert.Empty(allFilters);
        }

    }
}
