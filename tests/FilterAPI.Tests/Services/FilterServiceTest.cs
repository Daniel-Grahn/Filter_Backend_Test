using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FilterAPI.Data;
using FilterAPI.Services;
using FilterAPI.DTOs;

namespace FilterAPI.Integration.Tests.Services
{
    public class FilterServiceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FilterServiceTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<FilterDb>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Register DbContext with a unique test database per test
                    var dbName = $"FiltersDbTest_{Guid.NewGuid()}";
                    var connectionString = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION_TEMPLATE")
                        ?? $"Server=localhost,1433;Database={dbName};User Id=sa;Password=BuildingGroupGiraffe9182;TrustServerCertificate=true;";

                    services.AddDbContext<FilterDb>(options =>
                        options.UseSqlServer(connectionString));
                });
            });
        }

        [Fact]
        public async Task GetFiltersAsync_Empty()
        {
            using var scope = _factory.Services.CreateScope();
            var filterService = scope.ServiceProvider.GetRequiredService<IFilterService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<FilterDb>();
            await dbContext.Database.EnsureCreatedAsync();

            var filters = await filterService.GetFiltersAsync("nonexistent", 999);
            Assert.Empty(filters);
        }
        
        [Fact]
        public async Task GetFiltersAsync_Find()
        {
            using var scope = _factory.Services.CreateScope();
            var filterService = scope.ServiceProvider.GetRequiredService<IFilterService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<FilterDb>();
            await dbContext.Database.EnsureCreatedAsync();

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
            using var scope = _factory.Services.CreateScope();
            var filterService = scope.ServiceProvider.GetRequiredService<IFilterService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<FilterDb>();
            await dbContext.Database.EnsureCreatedAsync();

            var filter = new FilterRequestDTO
            {
                FieldName = "name",
                UserId = 1,
                SourceId = "test"
            };

            var nofilters = await filterService.GetFiltersAsync("test", 1);
            // Act
            await filterService.AddOrUpdateFilterAsync(filter);
            var filterList = await filterService.GetFiltersAsync("test", 1);

            //// Assert
            Assert.Single(filterList);
            var savedFilter = filterList.First();
            Assert.Equal("name", savedFilter.FieldName);
            Assert.Equal(1, savedFilter.UserId);
            Assert.Equal("test", savedFilter.SourceId);
        }

    }
}
