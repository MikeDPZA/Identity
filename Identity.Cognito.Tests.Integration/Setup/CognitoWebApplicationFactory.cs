using System;
using System.Linq;
using Identity.Shared.Clients;
using Identity.Shared.Models.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace Identity.Cognito.Tests.Integration.Setup;

public class CognitoWebApplicationFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Cognito");
        builder.ConfigureServices(services =>
        {
            var mockServer = WireMockServer.Start();
            var mockServerUrl = mockServer.Urls.First();
            
            var restClientDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IRestClient));
            services.Remove(restClientDescriptor);

            services.AddScoped<IRestClient, IdentityClient>(_ => new(new Uri(mockServerUrl)));
            services.AddSingleton(mockServer);
        });
        
    }
}