namespace Razmx.Core;

public static class HttpExtensions
{
    public static bool IsHtmxRequest(this HttpRequest request)
    {
        return request.Headers["HX-request"].FirstOrDefault() == "true";
    }
    public static bool IsHtmxBackHistoryRequest(this HttpRequest request)
    {
        return request.Headers["Hx-History-Restore-Request"].FirstOrDefault() == "true";
    }
    public static bool RequireHtmxPartialResponse(this HttpRequest request)
    {
        return request.IsHtmxRequest() && !request.IsHtmxBackHistoryRequest();
    }
}