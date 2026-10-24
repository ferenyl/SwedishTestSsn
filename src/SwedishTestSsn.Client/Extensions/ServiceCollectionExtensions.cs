using Microsoft.Extensions.DependencyInjection;

namespace SwedishTestSsn.Client.Extensions;
/// <summary>
/// Dependency injection registration for SwedishTestSsn.Client.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ITestSsnClient"/> as a typed <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddSwedishTestSsn(this IServiceCollection services)
    {
        services.AddHttpClient<ITestSsnClient, TestSsnClient>(TestSsnClient.Configure);
        return services;
    }

}
