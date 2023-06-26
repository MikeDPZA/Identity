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
            .AddJwtBearer("UC",options =>
            {
                options.Authority = config.AuthorityUrl.ToString();
                options.Audience = config.UserCredentialsFlow.ClientId;
                options.RequireHttpsMetadata = false;
                
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true
                };
                options.Events = GetBearerEvents();
            });
        
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("CC",options =>
            {
                options.Authority = config.AuthorityUrl.ToString();
                options.Audience = config.ClientCredentialsFlow.ClientId;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new()
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidateLifetime = true
                };

                options.Events = GetBearerEvents();
            });


        return services;
    }

    private static JwtBearerEvents GetBearerEvents()
    {
        return  new JwtBearerEvents
        {
            OnAuthenticationFailed = async ctx =>
            {
                var x = ctx.Exception;
            },
            OnChallenge = async ctx =>
            {
                var x = ctx;
            },
            OnForbidden = async ctx =>
            {
                var x = ctx;
            },
            OnMessageReceived = async ctx =>
            {
                var x = ctx;
            },
            OnTokenValidated = async ctx =>
            {
                var subjectClaim = ctx.Principal?
                    .Claims
                    .FirstOrDefault(_ => _.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

                if (subjectClaim is not null)
                {
                    var userId = subjectClaim.Value;
                }
            }
        };
    }
}