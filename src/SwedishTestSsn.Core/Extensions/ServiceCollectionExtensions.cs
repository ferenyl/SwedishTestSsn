using Microsoft.Extensions.DependencyInjection;

namespace SwedishTestSsn.Core.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwedishTestSsn(this IServiceCollection services)
    {
        services.AddHttpClient("SwedishTestSsnClient", client =>
        {
            client.BaseAddress = new Uri("https://skatteverket.entryscape.net/rowstore/dataset/b4de7df7-63c0-4e7e-bb59-1f156a591763");
        });
        services.AddSingleton<IClient, Client>();
        return services;
    }

}
