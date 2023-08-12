using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reception.Adb2c;

public static class AdB2CPackage
{
    public static IServiceCollection UseAdB2C(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }
}