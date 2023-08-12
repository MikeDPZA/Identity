using Reception.Adb2c;
using Reception.Cognito;
using Reception.Infrastructure.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reception.Infrastructure.Services;

public class ReceptionServiceProviderResolver: Dictionary<string, Func<IServiceCollection, IConfiguration, IServiceCollection>>
{
    public ReceptionServiceProviderResolver()
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