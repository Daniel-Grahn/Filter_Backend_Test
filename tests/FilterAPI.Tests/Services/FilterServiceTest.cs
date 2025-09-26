using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FilterAPI.Data;
using FilterAPI.Services;
using FilterAPI.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using FilterAPI.Integration.Tests.Data;
using System.Reflection;

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

            foreach(var filter in filters)
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
            
            FilterRequestDTO filter = new() {FieldName = "status", UserId = 1, SourceId = "test"};
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
            
            FilterRequestDTO filter = new() {FieldName = "status", UserId = 1, SourceId = "test", Data=["c-1"]};
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
    }
}
