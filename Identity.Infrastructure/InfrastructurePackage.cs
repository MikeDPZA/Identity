using Identity.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class InfrastructurePackage
{
    public static IServiceCollection UseIdentityAuthorization(this IServiceCollection services,
        IConfiguration configuration)
    {
        new IdentityServiceProviderResolver()
            .Resolve(services, configuration);
        return services;
    }
}