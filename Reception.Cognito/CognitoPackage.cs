using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Reception.Cognito.Models.Config;
using Reception.Shared.Clients;
using Reception.Shared.Interfaces;
using Reception.Shared.Models.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reception.Cognito.Services;
using Reception.Shared.Exceptions;

namespace Reception.Cognito;

[ExcludeFromCodeCoverage]
public static class CognitoPackage
{
    public static IServiceCollection UseCognito(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CognitoConfiguration>(configuration.GetSection("CognitoConfiguration"));
        var config = configuration.GetSection("CognitoConfiguration")
            .Get<CognitoConfiguration>();

        if (config == default)
        {
            throw new PackageSetupException();
        }
        
        services.AddScoped<IRestClient, ReceptionClient>(_ => new ReceptionClient(config.BaseUrl));
        services.AddScoped<IOAuthFacade, CognitoOAuthFacade>();

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
                    .FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier);

                if (subjectClaim is not null)
                {
                    var userId = subjectClaim.Value;
                }
            }
        };
    }
}