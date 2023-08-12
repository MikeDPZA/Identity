using System.Diagnostics.CodeAnalysis;
using Reception.Cognito.Models.Config;
using Reception.Shared.Clients;
using Reception.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reception.Cognito.Context;
using Reception.Cognito.Services;
using Reception.Shared.Constants;
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
        services.AddScoped<IOAuthTokenFacade, CognitoOAuthTokenFacade>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(AuthenticationSchemeConstants.UserCredentials,options =>
            {
                options.Authority = config.AuthorityUrl.ToString();
                options.Audience = config.UserCredentialsFlow.ClientId;
                options.RequireHttpsMetadata = false;
                
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidateLifetime = true
                };
                options.Events = GetBearerEvents(services);
            })
            .AddJwtBearer(AuthenticationSchemeConstants.ClientCredentials,options =>
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

                options.Events = GetBearerEvents(services);
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthenticationSchemeConstants.UserCredentials, p =>
            {
                p.AuthenticationSchemes.Add(AuthenticationSchemeConstants.UserCredentials);
                p.RequireAssertion(ctx => ValidateClient(ctx, config.UserCredentialsFlow.ClientId));
            });
            
            options.AddPolicy(AuthenticationSchemeConstants.ClientCredentials, p =>
            {
                p.AuthenticationSchemes.Add(AuthenticationSchemeConstants.ClientCredentials);
                p.RequireAssertion(ctx => ValidateClient(ctx, config.ClientCredentialsFlow.ClientId));
            });
        });
        
        return services;
    }

    private static bool ValidateClient(AuthorizationHandlerContext ctx, string clientId)
    {
        var claim = ctx.User.Claims.FirstOrDefault(_ => _.Type == "client_id");

        if (claim == null)
        {
            return false;
        }

        return claim.Value == clientId;
    }

    private static JwtBearerEvents GetBearerEvents(IServiceCollection services)
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
                var claims = ctx.Principal?.Claims?.ToDictionary(_ => _.Type, _ => _.Value);

                if (claims == null) return;
                
                // services.AddScoped<CurrentTokenContext>(_ => new (claims));
            }
        };
    }
}