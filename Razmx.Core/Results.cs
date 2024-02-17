using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Razmx.Core;

public static class HtmxResults
{
    public static IResult RedirectToCreated<TPage>(HttpContext context, object? routeValues = null)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName(context, routeName, routeValues);

        var retarget = context.Request.Headers["HX-Retarget"];
        if (!StringValues.IsNullOrEmpty(retarget))
        {
            path = $"{{\"path\":\"{path}\", \"target\":\"{retarget}\"}}";
        }

        context.Response.Headers.TryAdd("HX-Location", path);
        if (context.Request.CanRedirectWithHtmx())
        {
            return TypedResults.CreatedAtRoute(routeName, routeValues);
        }

        return TypedResults.RedirectToRoute(routeName, routeValues);
    }

    public static IResult Redirect<TPage>(HttpContext context, object? routeValues = null)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName(context, routeName, routeValues);
        path = AddRetargetToLocationPathIfRequired(context, path);

        context.Response.Headers.TryAdd("HX-Location", path);
        if (context.Request.CanRedirectWithHtmx())
        {
            return TypedResults.Ok();
        }

        return TypedResults.RedirectToRoute(routeName, routeValues);
    }

    private static string? AddRetargetToLocationPathIfRequired(HttpContext context, string? path)
    {
        var retarget = context.Request.Headers["HX-Retarget"];
        if (!StringValues.IsNullOrEmpty(retarget))
        {
            path = $"{{\"path\":\"{path}\", \"target\":\"{retarget}\"}}";
        }

        return path;
    }

    public static IResult FullPageRedirectToCreated<TPage>(HttpContext context, object? routeValues = null)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName(context, routeName, routeValues);

        context.Response.Headers.TryAdd("HX-Redirect", path);
        if (context.Request.CanRedirectWithHtmx())
        {
            return TypedResults.CreatedAtRoute(routeName, routeValues);
        }

        return TypedResults.RedirectToRoute(routeName, routeValues);
    }

    public static IResult FullPageRedirect<TPage>(HttpContext context, object? routeValues = null)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName(context, routeName, routeValues);

        context.Response.Headers.TryAdd("HX-Redirect", path);
        if (context.Request.CanRedirectWithHtmx())
        {
            return Results.Ok();
        }

        return Results.Redirect(path);
    }

    public static IResult FullPageRedirect(HttpContext context, string url)
    {
        context.Response.Headers.TryAdd("HX-Redirect", url);
        if (context.Request.CanRedirectWithHtmx())
        {
            return Results.Ok();
        }

        return Results.Redirect(url);
    }

    public static IResult RedirectToUrl(HttpContext context, string url)
    {
        var path = AddRetargetToLocationPathIfRequired(context, url);
        if (context.Request.CanRedirectWithHtmx())
        {
            context.Response.Headers.TryAdd("HX-Location", path);
            return Results.Ok();
        }

        return Results.Redirect(url);
    }

    public static IResult RedirectToRoute(HttpContext context, string routeName)
    {
        var url = GeneratePathForRouteName(context, routeName, null);
        var path = AddRetargetToLocationPathIfRequired(context, url);
        context.Response.Headers.TryAdd("HX-Location", path);
        if (context.Request.CanRedirectWithHtmx())
        {
            return Results.Ok();
        }

        return Results.RedirectToRoute(routeName);
    }

    private static string? GeneratePathForRouteName(HttpContext context, string routeName, object? routeValues)
    {
        var generator = context.RequestServices.GetRequiredService<LinkGenerator>();
        return generator.GetPathByName(routeName, routeValues);
    }
}