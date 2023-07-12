using Identity.Adb2c;
using Identity.Cognito;
using Identity.Infrastructure.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Services;

public class IdentityServiceProviderResolver: Dictionary<string, Func<IServiceCollection, IConfiguration, IServiceCollection>>
{
    public IdentityServiceProviderResolver()
    {
        Add("Cognito", (services, configuration) => services.UseCognito(configuration));
        Add("AdB2C", (services, configuration) => services.UseAdB2C(configuration));
    }
    
    public IServiceCollection Resolve(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetSection("Provider").Get<string>();

        if (string.IsNullOrEmpty(provider))
        {
            throw new NoProviderException();
        }
        
        return this[provider](services, configuration);
    }
}