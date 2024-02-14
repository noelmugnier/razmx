using Microsoft.Extensions.Primitives;
using Razmx.App;

public static partial class Results
{
    public static IResult HtmxLocation<TPage>(HttpContext context, object? routeValues = null)
        where TPage : HtmxPage
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

    public static IResult HtmxRedirect<TPage>(HttpContext context, object? routeValues = null)
        where TPage : HtmxPage
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName<TPage>(context, routeName, routeValues);

        context.Response.Headers.TryAdd("HX-Redirect", path);

        return TypedResults.CreatedAtRoute(routeName, routeValues);
    }

    private static string? GeneratePathForRouteName<TPage>(HttpContext context, string routeName, object? routeValues)
        where TPage : HtmxPage
    {
        var generator = context.RequestServices.GetRequiredService<LinkGenerator>();
        return generator.GetPathByName(routeName, routeValues);
    }
}