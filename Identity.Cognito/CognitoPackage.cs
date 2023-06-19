using Identity.Cognito.Models.Config;
using Identity.Cognito.Services;
using Identity.Shared.Interfaces;
using Identity.Shared.Models.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Cognito;

public static class CognitoPackage
{
    public static IServiceCollection AddCognitoProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CognitoConfiguration>(configuration.GetSection("CognitoConfiguration"));
        var config = configuration.GetSection("CognitoConfiguration")
            .Get<CognitoConfiguration>();


        services.AddScoped<IOAuthFacade, CognitoOAuthFacade>(_ => new(
                _.GetRequiredService<IOptions<CognitoConfiguration>>(),
                new IdentityClient(config.BaseUrl)
            )
        );

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = config.BaseUrl.AbsolutePath;
                options.Audience = config.ClientId;
            });


        return services;
    }
}