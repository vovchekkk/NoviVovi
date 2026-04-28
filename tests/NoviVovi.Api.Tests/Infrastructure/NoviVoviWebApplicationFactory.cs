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
    private static TestDatabaseManager? _sharedDbManager;
    private static int _instanceCount = 0;

    public string TestConnectionString => _sharedDbManager?.ConnectionString 
        ?? throw new InvalidOperationException("Database not initialized");

    public async Task InitializeAsync()
    {
        await _dbSemaphore.WaitAsync();
        try
        {
            _instanceCount++;
            
            // Create database only once for all tests
            if (_sharedDbManager == null)
            {
                _sharedDbManager = new TestDatabaseManager();
                await _sharedDbManager.InitializeAsync();
                Console.WriteLine($"[TestDB] Created shared test database: {_sharedDbManager.ConnectionString}");
            }
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
            _instanceCount--;
            
            // Cleanup database only when last instance is disposed
            if (_instanceCount == 0 && _sharedDbManager != null)
            {
                Console.WriteLine($"[TestDB] Cleaning up shared test database");
                await _sharedDbManager.CleanupAsync();
                _sharedDbManager.Dispose();
                _sharedDbManager = null;
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
