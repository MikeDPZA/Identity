using Reception.Shared.Models.Builders;
using RestSharp;
using IRestClient = Reception.Shared.Clients.IRestClient;

namespace Reception.Shared.Clients;

public class ReceptionClient : IRestClient
{
    private readonly RestClient _client;
    
    public ReceptionClient(Uri baseUri)
    {
        _client = new RestClient(baseUri);
    }
    
    public async Task<TResult> ExecuteAsync<TResult>(HttpRequestData requestData)
    {
        var request = BuildRequest(requestData);
        var response = await _client.ExecuteAsync<TResult>(request);

        if (!response.IsSuccessful)
        {
            throw new HttpRequestException(response.Content);
        }
        
        return response.Data!;
    }
    
    public async Task ExecuteAsync(HttpRequestData requestData)
    {
        var request = BuildRequest(requestData);
        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new HttpRequestException(response.Content);
        }
    }

    private static RestRequest BuildRequest(HttpRequestData requestData)
    {
        var request = new RestRequest(requestData.Path);
        AddMethod(request, requestData.Method);
        AddHeaders(request, requestData.Headers);
        AddQueryParams(request, requestData.QueryParams);
        AddPathParams(request,requestData.PathParams);

        if (!string.IsNullOrEmpty(requestData.Body))
        {
            request.AddBody(requestData.Body);
        }

        return request;
    }

    private static void AddMethod(RestRequest request, HttpMethod method)
    {
        if (method == HttpMethod.Get)
        {
            request.Method = Method.Get;
            return;
        }
        
        if (method == HttpMethod.Post)
        {
            request.Method = Method.Post;
            return;
        }
        
        if (method == HttpMethod.Put)
        {
            request.Method = Method.Put;
            return;
        }
        
        if (method == HttpMethod.Patch)
        {
            request.Method = Method.Patch;
            return;
        }
        
        if (method == HttpMethod.Delete)
        {
            request.Method = Method.Delete;
            return;
        }
    }
    
    private static void AddHeaders(RestRequest request, Dictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            request.AddHeader(header.Key, header.Value);
        }
    }

    private static void AddQueryParams(RestRequest request, Dictionary<string, string> queryParams)
    {
        foreach (var formParam in queryParams)
        {
            request.AddQueryParameter(formParam.Key, formParam.Value);
        }
    }
    
    private static void AddPathParams(RestRequest request, Dictionary<string, string> pathParams)
    {
        foreach (var formParam in pathParams)
        {
            request.AddUrlSegment(formParam.Key, formParam.Value);
        }
    }
}