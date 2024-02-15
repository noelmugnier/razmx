namespace Razmx.Core;

public static class HttpExtensions
{
    public static bool IsHtmxRequest(this HttpRequest request)
    {
        return request.Headers["HX-request"].FirstOrDefault() == "true";
    }
}