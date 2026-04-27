using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Api.Tests.Infrastructure;

public class NoviVoviWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly SemaphoreSlim _dbSemaphore = new(1, 1);
    private TestDatabaseManager? _dbManager;

    public string TestConnectionString => _dbManager?.ConnectionString 
        ?? throw new InvalidOperationException("Database not initialized");

    public async Task InitializeAsync()
    {
        await _dbSemaphore.WaitAsync();
        try
        {
            _dbManager = new TestDatabaseManager();
            await _dbManager.InitializeAsync();
        }
        finally
        {
            _dbSemaphore.Release();
        }
    }

    public new async Task DisposeAsync()
    {
        await _dbSemaphore.WaitAsync();
        try
        {
            if (_dbManager != null)
            {
                await _dbManager.CleanupAsync();
                _dbManager.Dispose();
            }
        }
        finally
        {
            _dbSemaphore.Release();
        }
        
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Override connection string with test database
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:LocalDatabase"] = TestConnectionString,
                ["UseLocalDatabase"] = "true",
                ["UseLocalStorage"] = "true"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Mock storage service to avoid S3/file system calls in tests
            services.RemoveAll(typeof(IStorageService));
            services.AddSingleton<IStorageService, MockStorageService>();
        });

        builder.UseEnvironment("Testing");
    }
}
