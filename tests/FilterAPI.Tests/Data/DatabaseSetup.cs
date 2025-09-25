using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FilterAPI.Data;

namespace FilterAPI.Integration.Tests.Data
{

public class TestDatabaseFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> Factory { get; private set; } = null!;
    public string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var dbName = $"FiltersDbTest_{Guid.NewGuid():N}";
        ConnectionString =
            $"Server=localhost,1433;Database={dbName};User Id=sa;Password=BuildingGroupGiraffe9182;TrustServerCertificate=true;";

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<FilterDb>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<FilterDb>(options =>
                        options.UseSqlServer(ConnectionString));
                });
            });

        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FilterDb>();
        await db.Database.EnsureDeletedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FilterDb>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
}

}
