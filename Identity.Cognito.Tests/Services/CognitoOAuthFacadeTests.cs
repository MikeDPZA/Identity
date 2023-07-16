using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Cognito.Models.Config;
using Identity.Cognito.Services;
using Identity.Shared.Models.Builders;
using Identity.Shared.Models.Clients;
using Identity.Shared.Models.OAuth;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using System.Linq;
using System.Net.Http;
using System.Text;
using FluentAssertions;

namespace Identity.Cognito.Tests.Services;

public class CognitoOAuthFacadeTests
{
    private readonly CognitoOAuthFacade _sut;
    private readonly IOptions<CognitoConfiguration> _optionsMock;
    private readonly Mock<IRestClient> _restMock;

    public CognitoOAuthFacadeTests()
    {
        _optionsMock = Options.Create(new CognitoConfiguration()
        {
            Domain = "some-domain",
            Region = "some-region",
            UserPoolId = "some-user-pool-id",
            ClientCredentialsFlow = new()
            {
                Scopes = new[] { "some-cc-scope" },
                ClientId = "some-cc-client-id",
                ClientSecret = "some-cc-client-secret"
            },
            UserCredentialsFlow = new()
            {
                Scopes = new[] { "some-uc-scope" },
                ClientId = "some-uc-client-id",
                ClientSecret = "some-uc-client-secret",
                RedirectUri = new("https://some-uc-redirect-uri")
            }
        });

        _restMock = new Mock<IRestClient>();

        _sut = new CognitoOAuthFacade(
            _optionsMock,
            _restMock.Object
        );
    }

    [Fact]
    public async Task GetAccessTokenAsync_OnSuccess_ReturnsToken()
    {
        var auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"some-uc-client-id:some-uc-client-secret"))}";
        var code = Uri.EscapeDataString("some-code");
        _restMock.Setup(_ => _.ExecuteAsync<OAuthResponse>(It.Is<HttpRequestData>(__ =>
                __.QueryParams.Contains(new("code", code)) &&
                __.QueryParams.Contains(new("redirect_uri", "https://some-uc-redirect-uri/")) &&
                __.QueryParams.Contains(new("grant_type", "authorization_code")) &&
                __.QueryParams.Contains(new("client_id", "some-uc-client-id")) &&
                __.QueryParams.Contains(new("client_secret", "some-uc-client-secret")) &&
                __.QueryParams.Contains(new("scope", "some-uc-scope")) &&
                __.Headers.Contains(new("Content-Type", "application/x-www-form-urlencoded")) &&
                __.Headers.Contains(new("Authorization", auth)) &&
                __.Path == "oauth2/token" &&
                __.Method == HttpMethod.Post
            )))
            .ReturnsAsync(new OAuthResponse()
            {
                AccessToken = "ey...",
                ExpiresIn = 100,
                RefreshToken = "eyr...",
                TokenType = "bearer"
            });
        var result = await _sut.GetAccessTokenAsync("some-code");

        result.Should().BeEquivalentTo(new OAuthResponse()
        {
            AccessToken = "ey...",
            ExpiresIn = 100,
            RefreshToken = "eyr...",
            TokenType = "bearer"
        });
        
        _restMock.Verify(_ => _.ExecuteAsync<OAuthResponse>(
            It.Is<HttpRequestData>(__ =>
                __.QueryParams.Contains(new("code", "some-code")) &&
                __.QueryParams.Contains(new("redirect_uri", "https://some-uc-redirect-uri/")) &&
                __.QueryParams.Contains(new("grant_type", "authorization_code")) &&
                __.QueryParams.Contains(new("client_id", "some-uc-client-id")) &&
                __.QueryParams.Contains(new("client_secret", "some-uc-client-secret")) &&
                __.QueryParams.Contains(new("scope", "some-uc-scope")) &&
                __.Headers.Contains(new("Content-Type", "application/x-www-form-urlencoded")) &&
                __.Headers.Contains(new("Authorization", auth)) &&
                __.Path == "oauth2/token" &&
                __.Method == HttpMethod.Post
            )), Times.Once);
    }

    [Fact]
    public async Task GetClientCredentialsTokenAsync_OnSuccess_ReturnsToken()
    {
        var auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"some-cc-client-id:some-cc-client-secret"))}";
        _restMock.Setup(_ => _.ExecuteAsync<OAuthResponse>(It.Is<HttpRequestData>(__ =>
                __.QueryParams.Contains(new("grant_type", "client_credentials")) &&
                __.QueryParams.Contains(new("client_id", "some-cc-client-id")) &&
                __.QueryParams.Contains(new("client_secret", "some-cc-client-secret")) &&
                __.QueryParams.Contains(new("scope", "some-cc-scope")) &&
                __.Headers.Contains(new("Content-Type", "application/x-www-form-urlencoded")) &&
                __.Headers.Contains(new("Authorization", auth)) &&
                __.Path == "oauth2/token" &&
                __.Method == HttpMethod.Post
            )))
            .ReturnsAsync(new OAuthResponse()
            {
                AccessToken = "ey...",
                ExpiresIn = 100,
                RefreshToken = "",
                TokenType = "bearer"
            });
        var result = await _sut.GetClientCredentialsTokenAsync();

        _restMock.Verify(_ => _.ExecuteAsync<OAuthResponse>(
            It.Is<HttpRequestData>(__ =>
                __.QueryParams.Contains(new("grant_type", "client_credentials")) &&
                __.QueryParams.Contains(new("client_id", "some-cc-client-id")) &&
                __.QueryParams.Contains(new("client_secret", "some-cc-client-secret")) &&
                __.QueryParams.Contains(new("scope", "some-cc-scope")) &&
                __.Headers.Contains(new("Content-Type", "application/x-www-form-urlencoded")) &&
                __.Headers.Contains(new("Authorization", auth)) &&
                __.Path == "oauth2/token" &&
                __.Method == HttpMethod.Post
            )), Times.Once);
    }

    [Fact]
    public void GetLoginForm()
    {

        var result = _sut.GetLoginForm();

        result.Should().Be(new Uri("https://some-domain.auth.some-region.amazoncognito.com/login?" +
                                               "response_type=code" +
                                               "&client_id=some-uc-client-id" +
                                               "&scope=some-uc-scope" +
                                               "&redirect_uri=https%3a%2f%2fsome-uc-redirect-uri%2f"
                                               ));
        
    }
}