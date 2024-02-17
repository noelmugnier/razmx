namespace Razmx.Core;

public static class HtmxRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHtmxPages(this IEndpointRouteBuilder endpoints, Type assemblyType, params Type[] typesFromAssemblies)
    {
        var types = new List<Type> { assemblyType }.Concat(typesFromAssemblies);
        foreach(var type in types)
        {
            var pages = type.Assembly.GetTypes().Where(type => type.GetCustomAttribute<RouteAttribute>() != null);
            foreach (var page in pages)
            {
                var routeAttribute = page.GetCustomAttribute<RouteAttribute>(true);
                if (routeAttribute is null)
                {
                    continue;
                }

                var routeTemplate = routeAttribute!.Template;
                var routeName = page.FullName!;

                RegisterEndpoint(endpoints, page, routeTemplate, routeName);
            }
        }

        return endpoints;
    }

    public static IEndpointRouteBuilder MapDefaultPage<TComponent>(this IEndpointRouteBuilder endpoints, string defaultRouteTemplate = "/", string defaultRouteName = "Home") where TComponent : IComponent
    {
        var htmxComponent = typeof(TComponent);
        RegisterEndpoint(endpoints, htmxComponent, defaultRouteTemplate, defaultRouteName);

        return endpoints;
    }

    public static IEndpointRouteBuilder MapNotFoundPage<TComponent>(this IEndpointRouteBuilder endpoints) where TComponent : IComponent
    {
        var htmxComponent = typeof(TComponent);
        var endpoint = endpoints.MapFallback((HttpContext context) =>
        {
            var parameters = ExtractComponentParameters(context);
            return new RazorComponentResult(htmxComponent, parameters);
        }).WithName("NotFound");

        var authorizeAttributes = htmxComponent.GetCustomAttributes(typeof(AuthorizeAttribute), true);
        if (authorizeAttributes.Any())
        {
            endpoint.RequireAuthorization();
        }

        return endpoints;
    }

    private static void RegisterEndpoint(IEndpointRouteBuilder endpoints, Type htmxComponent, string routeTemplate,
        string routeName)
    {
        var redirectToRouteIfAuthenticated = htmxComponent.GetCustomAttribute<RedirectToRouteIfAuthenticatedAttribute>();
        var redirectToUrlIfAuthenticated = htmxComponent.GetCustomAttribute<RedirectToUrlIfAuthenticatedAttribute>();
        var endpoint = endpoints.MapGet(routeTemplate, (HttpContext context) =>
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            if (isAuthenticated && redirectToRouteIfAuthenticated != null)
            {
                return HtmxResults.RedirectToRoute(context, redirectToRouteIfAuthenticated.RedirectTo);
            }
            if (isAuthenticated && redirectToUrlIfAuthenticated != null)
            {
                return HtmxResults.RedirectToUrl(context, redirectToUrlIfAuthenticated.RedirectTo);
            }

            var parameters = ExtractComponentParameters(context);
            return new RazorComponentResult(htmxComponent, parameters);
        }).WithName(routeName);

        var authorizeAttributes = htmxComponent.GetCustomAttributes(typeof(AuthorizeAttribute), true);
        if (authorizeAttributes.Any())
        {
            endpoint.RequireAuthorization();
        }

        var anonymousAttributes = htmxComponent.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
    }

    private static Dictionary<string, object?> ExtractComponentParameters(HttpContext context)
    {
        var parameters = new Dictionary<string, object?>();
        foreach (var key in context.Request.Query.Keys)
        {
            parameters.Add(key, context.Request.Query[key]);
        }

        foreach (var key in context.Request.RouteValues.Keys)
        {
            parameters.Add(key, context.Request.RouteValues[key]);
        }

        return parameters;
    }
}