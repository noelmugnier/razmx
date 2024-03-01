using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Razmx.Core;

public static class HtmxResults
{
    public static IResult NavigateTo<TPage>(this HttpContext context, object? routeValues = null, bool replaceUrl = false)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName(context, routeName, routeValues);

        if (!context.Request.RequireHtmxPartialResponse())
            return TypedResults.Redirect(routeName);

        if (replaceUrl)
        {
            context.Response.Headers.TryAdd("HX-Replace-Url", "true");
        }

        context.Response.Headers.TryAdd("HX-Location", context.BuildHtmxLocationHeader(path));
        return TypedResults.Ok();
    }

    public static IResult RedirectTo<TPage>(this HttpContext context, object? routeValues = null, bool replaceUrl = false)
        where TPage : IComponent
    {
        var routeName = typeof(TPage).FullName!;
        var path = GeneratePathForRouteName(context, routeName, routeValues);
        if (!context.Request.RequireHtmxPartialResponse())
            return TypedResults.Redirect(path);

        if (replaceUrl)
        {
            context.Response.Headers.TryAdd("HX-Replace-Url", "true");
        }

        context.Response.Headers.TryAdd("HX-Redirect", path);
        return TypedResults.Ok();
    }

    public static IResult RedirectToUrl(this HttpContext context, string url, bool replaceUrl = false)
    {
        if (!context.Request.RequireHtmxPartialResponse())
            return Results.Redirect(url);

        if (replaceUrl)
        {
            context.Response.Headers.TryAdd("HX-Replace-Url", "true");
        }

        context.Response.Headers.TryAdd("HX-Redirect", url);
        return Results.Ok();
    }

    public static IResult Refresh(this HttpContext context)
    {
        if (!context.Request.RequireHtmxPartialResponse())
            return Results.LocalRedirect(context.Request.Path.Value);

        context.Response.Headers.TryAdd("HX-Refresh", "true");
        return Results.Ok();
    }

    public static IResult NavigateToUrl(this HttpContext context, string url, bool replaceUrl = false)
    {
        if (!context.Request.RequireHtmxPartialResponse())
            return Results.Redirect(url);

        if (replaceUrl)
        {
            context.Response.Headers.TryAdd("HX-Replace-Url", "true");
        }

        context.Response.Headers.TryAdd("HX-Location", context.BuildHtmxLocationHeader(url));
        return Results.Ok();
    }

    public static IResult NavigateToRoute(this HttpContext context, string routeName, object? routeValues = null,
        bool replaceUrl = false)
    {
        var url = GeneratePathForRouteName(context, routeName, routeValues);
        if (!context.Request.RequireHtmxPartialResponse())
            return Results.RedirectToRoute(routeName);

        if (replaceUrl)
        {
            context.Response.Headers.TryAdd("HX-Replace-Url", "true");
        }

        var path = context.BuildHtmxLocationHeader(url);
        context.Response.Headers.TryAdd("HX-Location", path);
        return Results.Ok();
    }

    public static IResult NoContent(this HttpContext context)
    {
        if (!context.Request.RequireHtmxPartialResponse())
            return TypedResults.NoContent();

        return TypedResults.Ok();
    }

    public static string BuildHtmxLocationHeader(this HttpContext context, string path)
    {
        var retarget = context.Request.Headers["HX-Retarget"];
        return StringValues.IsNullOrEmpty(retarget)
            ? path
            : $"{{\"path\":\"{path}\", \"target\":\"{retarget}\"}}";
    }

    private static string? GeneratePathForRouteName(HttpContext context, string routeName, object? routeValues)
    {
        var generator = context.RequestServices.GetRequiredService<LinkGenerator>();
        return generator.GetPathByName(routeName, routeValues);
    }
}