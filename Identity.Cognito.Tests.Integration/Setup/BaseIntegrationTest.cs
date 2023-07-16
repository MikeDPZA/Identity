using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;
using Xunit;

namespace Identity.Cognito.Tests.Integration.Setup;

public abstract class BaseIntegrationTest: IClassFixture<CognitoWebApplicationFactory>
{
    private readonly CognitoWebApplicationFactory _factory;
    protected HttpClient Client { get; set; }
    protected WireMockServer MockServer { get; set; }

    protected BaseIntegrationTest(CognitoWebApplicationFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
        MockServer = factory.Services.GetRequiredService<WireMockServer>();
    }

    protected async Task<T?> ParseResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content);
    }
    
}