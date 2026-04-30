using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Api.Tests.Infrastructure;

[Collection("Database collection")]
public abstract class IntegrationTestBase : IClassFixture<NoviVoviWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly NoviVoviWebApplicationFactory Factory;
    private IServiceScope? _scope;
    protected IUnitOfWork? UnitOfWork;
    
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(),
            new TransitionResponseConverter(),
            new StepResponseConverter()
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    protected IntegrationTestBase(NoviVoviWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Factory.InitializeAsync();
        _scope = Factory.Services.CreateScope();
        UnitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        // Clean database before each test
        await ClearDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        if (UnitOfWork != null)
        {
            await UnitOfWork.DisposeAsync();
        }
        
        // Don't call _scope.Dispose() because UnitOfWork only implements IAsyncDisposable
        // The scope will be cleaned up by the test framework
    }

    protected async Task<TResponse?> PostAsync<TResponse>(string url, object request)
    {
        var response = await Client.PostAsJsonAsync(url, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    protected async Task<TResponse?> GetAsync<TResponse>(string url)
    {
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    protected async Task<List<TResponse>?> GetListAsync<TResponse>(string url)
    {
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<TResponse>>(JsonOptions);
    }

    protected async Task<TResponse?> PatchAsync<TResponse>(string url, object request)
    {
        var response = await Client.PatchAsJsonAsync(url, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    protected async Task DeleteAsync(string url)
    {
        var response = await Client.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    protected async Task<HttpResponseMessage> PostRawAsync(string url, object request)
    {
        return await Client.PostAsJsonAsync(url, request, JsonOptions);
    }

    protected async Task<HttpResponseMessage> GetRawAsync(string url)
    {
        return await Client.GetAsync(url);
    }

    protected async Task<HttpResponseMessage> PatchRawAsync(string url, object request)
    {
        return await Client.PatchAsJsonAsync(url, request);
    }

    protected async Task<HttpResponseMessage> DeleteRawAsync(string url)
    {
        return await Client.DeleteAsync(url);
    }

    /// <summary>
    /// Executes raw SQL query for test verification.
    /// </summary>
    protected async Task<T?> QuerySingleAsync<T>(string sql, object? param = null)
    {
        return await UnitOfWork.Connection.QuerySingleOrDefaultAsync<T>(sql, param, UnitOfWork.Transaction);
    }

    /// <summary>
    /// Executes raw SQL query for test verification.
    /// </summary>
    protected async Task<List<T>> QueryAsync<T>(string sql, object? param = null)
    {
        var result = await UnitOfWork.Connection.QueryAsync<T>(sql, param, UnitOfWork.Transaction);
        return result.ToList();
    }

    /// <summary>
    /// Clears all data from test database for test isolation.
    /// </summary>
    protected async Task ClearDatabaseAsync()
    {
        await UnitOfWork.Connection.ExecuteAsync(@"
            TRUNCATE TABLE ""Novels"" CASCADE;
        ", transaction: UnitOfWork.Transaction);
    }
}
