using Reception.Shared.Models.Builders;

namespace Reception.Shared.Clients;

public interface IRestClient
{
    Task<TResult> ExecuteAsync<TResult>(HttpRequestData requestData);
    Task ExecuteAsync(HttpRequestData requestData);
}