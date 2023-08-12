using System;
using System.Linq;
using Reception.Shared.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace Reception.Cognito.Tests.Integration.Setup;

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

            services.AddScoped<IRestClient, ReceptionClient>(_ => new(new Uri(mockServerUrl)));
            services.AddSingleton(mockServer);
        });
        
    }
}