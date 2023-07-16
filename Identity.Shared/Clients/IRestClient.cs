using Identity.Shared.Models.Builders;

namespace Identity.Shared.Models.Clients;

public interface IRestClient
{
    Task<TResult> ExecuteAsync<TResult>(HttpRequestData requestData);
}