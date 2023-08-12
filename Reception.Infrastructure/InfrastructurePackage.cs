using Reception.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reception.Infrastructure;

public static class InfrastructurePackage
{
    public static IServiceCollection UseReceptionAuthorization(this IServiceCollection services,
        IConfiguration configuration)
    {
        new ReceptionServiceProviderResolver()
            .Resolve(services, configuration);
        return services;
    }
}