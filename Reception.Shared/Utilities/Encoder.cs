using System.Text;

namespace Reception.Shared.Utilities;

public static class Encoder
{
    public static string Encode64(string? value)
        => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
}