namespace Identity.Shared.Models.Builders;

public class HttpRequestData
{
    public HttpMethod Method { get; set; }
    public string Path { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> QueryParams { get; set; }
    public Dictionary<string, string> PathParams { get; set; }
    
}