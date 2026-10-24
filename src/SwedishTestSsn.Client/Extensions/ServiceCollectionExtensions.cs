using Microsoft.Extensions.DependencyInjection;

namespace SwedishTestSsn.Client.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwedishTestSsn(this IServiceCollection services)
    {
        services.AddHttpClient<ITestSsnClient, TestSsnClient>(TestSsnClient.Configure);
        return services;
    }

}
