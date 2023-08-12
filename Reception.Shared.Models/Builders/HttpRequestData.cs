using System.Diagnostics.CodeAnalysis;

namespace Reception.Shared.Models.Builders;

[ExcludeFromCodeCoverage]
public class HttpRequestData
{
    public HttpMethod Method { get; set; } = null!;
    public string Path { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, string> QueryParams { get; set; } = new();
    public Dictionary<string, string> PathParams { get; set; } = new();

}