using System.Text.Json;
using Reception.Shared.Models.Builders;

namespace Reception.Shared.Builders;

public class HttpRequestBuilder
{
    private readonly HttpRequestData _data;
    
    private HttpRequestBuilder(HttpMethod method, params string[] path)
    {
        _data = new HttpRequestData()
        {
            Method = method,
            Path = string.Join("/", path),
            Headers = new(),
            QueryParams = new(),
            PathParams = new()
        };
    }
    
    public static HttpRequestBuilder Get(params string[] path) => new(HttpMethod.Get, path);
    public static HttpRequestBuilder Post(params string[] path) => new(HttpMethod.Post, path);
    public static HttpRequestBuilder Put(params string[] path) => new(HttpMethod.Put, path);
    public static HttpRequestBuilder Patch(params string[] path) => new(HttpMethod.Patch, path);
    public static HttpRequestBuilder Delete(params string[] path) => new(HttpMethod.Delete, path);
    
    public HttpRequestBuilder WithBody(string body)
    {
        _data.Body = body;
        return this;
    }

    public HttpRequestBuilder WithBody<T>(T body)
    {
        var bodyText = JsonSerializer.Serialize(body);
        return WithBody(bodyText);
    }
    
    public HttpRequestBuilder WithHeader(string key, string value)
    {
        _data.Headers.Add(key, value);
        return this;
    }
    
    public HttpRequestBuilder WithHeader<T>(string key, T value) => WithHeader(key, value.ToString());
    
    public HttpRequestBuilder WithQueryParameter(string key, string value)
    {
        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            _data.QueryParams.Add(key, value);
        }
        return this;
    }
    
    public HttpRequestBuilder WithQueryParameter<T>(string key, T value) => WithQueryParameter(key, value.ToString());

    
    public HttpRequestBuilder WithPathParameter(string key, string value)
    {
        _data.PathParams.Add(key, value);
        return this;
    }
    
    public HttpRequestBuilder WithPathParameter<T>(string key, T value) => WithPathParameter(key, value.ToString());
    
    public HttpRequestData Build() => _data;
}