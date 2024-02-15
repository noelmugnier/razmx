using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Razmx.Core;

public static class HtmxResults
{
    public static IResult Location<TPage>(HttpContext context, object? routeValues = null)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName<TPage>(context, routeName, routeValues);

        var location = path;

        var retarget = context.Request.Headers["HX-Retarget"];
        if (!StringValues.IsNullOrEmpty(retarget))
        {
            location = $"{{\"path\":\"{path}\", \"target\":\"{retarget}\"}}";
        }

        context.Response.Headers.TryAdd("HX-Location", location);

        return TypedResults.CreatedAtRoute(routeName, routeValues);
    }

    public static IResult Redirect<TPage>(HttpContext context, object? routeValues = null)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName<TPage>(context, routeName, routeValues);

        context.Response.Headers.TryAdd("HX-Redirect", path);

        return TypedResults.CreatedAtRoute(routeName, routeValues);
    }

    private static string? GeneratePathForRouteName<TPage>(HttpContext context, string routeName, object? routeValues)
        where TPage : IComponent
    {
        var generator = context.RequestServices.GetRequiredService<LinkGenerator>();
        return generator.GetPathByName(routeName, routeValues);
    }
}