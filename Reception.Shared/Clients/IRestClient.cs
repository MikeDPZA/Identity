using Reception.Shared.Models.Builders;

namespace Reception.Shared.Models.Clients;

public interface IRestClient
{
    Task<TResult> ExecuteAsync<TResult>(HttpRequestData requestData);
}