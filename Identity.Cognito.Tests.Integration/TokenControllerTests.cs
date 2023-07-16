using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Cognito.Tests.Integration.Setup;
using Identity.Shared.Models.OAuth;
using Microsoft.Extensions.DependencyInjection;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using Xunit.Abstractions;

namespace Identity.Cognito.Tests.Integration;

public class TokenControllerTests : BaseIntegrationTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TokenControllerTests(
        CognitoWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper) : base(factory)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetLoginForm_WhenCalled_ReturnsSignInUrl()
    {
        var result = await Client.GetAsync("/v1/Token/Login");
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        content.Should()
            .Be(
                "https://some-domain.auth.some-region.amazoncognito.com/login?" +
                "response_type=code" +
                "&client_id=some-uc-client-id" +
                "&scope=some-scope" +
                "&redirect_uri=https%3a%2f%2fsome-host%2fv1%2fToken%2fAuthorization");
    }

    [Fact]
    public async Task GetAuthorizationToken_WhenCalled_ReturnsToken()
    {
        MockServer.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/oauth2/token")
                .WithParam("client_id", "some-uc-client-id")
                .WithParam("client_secret", "some-uc-client-secret")
                .WithParam("grant_type", "authorization_code")
                .WithParam("scope", "some-scope")
                .WithParam("code", "some-code")
                .WithParam("redirect_uri", new Uri("https://some-host/v1/Token/Authorization").ToString())
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithHeader("Authorization",
                    $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("some-uc-client-id:some-uc-client-secret"))}")
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyFromFile(Path.Join(Directory.GetCurrentDirectory(), "Stubs", "GetAuthorizationToken",
                    "Success.json"))
        );


        var result = await Client.GetAsync("v1/Token/Authorization?code=some-code");

        result.EnsureSuccessStatusCode();
        var deserialized = await ParseResponse<OAuthResponse>(result);

        deserialized.Should().BeEquivalentTo(new OAuthResponse()
        {
            AccessToken = "ey..",
            ExpiresIn = 3600,
            RefreshToken = "eyr..",
            TokenType = "Bearer"
        });
    }

    [Fact]
    public async Task GetAuthorizationToken_WhenInvalidCodeCalled_ReturnsErrorMessage()
    {
        MockServer.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/oauth2/token")
                .WithParam("code", "some-invalid-code")
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithHeader("Authorization",
                    $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("some-uc-client-id:some-uc-client-secret"))}")
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithBodyFromFile(Path.Join(Directory.GetCurrentDirectory(), "Stubs", "GetAuthorizationToken",
                    "InvalidGrantType.json"))
        );


        var result = await Client.GetAsync("v1/Token/Authorization?code=some-invalid-code");

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await result.Content.ReadAsStringAsync();
        content.Should().BeEquivalentTo("{\"error\": \"invalid_grant\"}");
    }
}