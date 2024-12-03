using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Post.Common.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection RegisterConfig<T>(this IServiceCollection serviceCollection, IConfiguration configuration)
        where T : class, new()
    {
        var configurationObject = Activator.CreateInstance<T>();
        configuration.Bind(configurationObject);
        serviceCollection.AddSingleton(configurationObject);

        return serviceCollection;
    }
}