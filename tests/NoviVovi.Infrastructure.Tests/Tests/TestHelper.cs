using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NoviVovi.Infrastructure.Tests.Tests;

public static class TestHelper
{
    public static ServiceProvider CreateProvider()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile("test-appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var services = new ServiceCollection();

        services.AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }
}