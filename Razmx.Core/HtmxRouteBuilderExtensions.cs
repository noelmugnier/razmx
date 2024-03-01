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

    public static IEndpointRouteBuilder MapNotFoundPage<TComponent>(this IEndpointRouteBuilder endpoints) where TComponent : IComponent
    {
        var htmxComponent = typeof(TComponent);
        var endpoint = endpoints.MapFallback((HttpContext context) =>
        {
            var parameters = ExtractComponentParameters(context);
            return new RazorComponentResult(htmxComponent, parameters);
        }).WithName(htmxComponent.FullName!);

        var authorizeAttributes = htmxComponent.GetCustomAttributes(typeof(AuthorizeAttribute), true);
        var anonymousAttributes = htmxComponent.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);

        if (anonymousAttributes.Length == 0 && authorizeAttributes.Length != 0)
        {
            endpoint.RequireAuthorization();
        }

        return endpoints;
    }

    private static void RegisterEndpoint(IEndpointRouteBuilder endpoints, Type htmxComponent, string routeTemplate,
        string routeName)
    {
        var endpoint = endpoints.MapGet(routeTemplate, (HttpContext context) =>
        {
            var parameters = ExtractComponentParameters(context);
            var component = new RazorComponentResult(htmxComponent, parameters);

            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                return component;
            }

            var redirectToPageIfAuthenticated = htmxComponent.GetCustomAttribute(typeof(RedirectIfAuthenticatedAttribute<>));
            if (redirectToPageIfAuthenticated != null)
            {
                return context.NavigateToRoute(((RedirectIfAuthenticatedAttribute)redirectToPageIfAuthenticated).RedirectTo);
            }

            var redirectToIfAuthenticated = htmxComponent.GetCustomAttribute<RedirectIfAuthenticatedAttribute>();
            if (redirectToIfAuthenticated != null)
            {
                return context.NavigateToUrl(redirectToIfAuthenticated.RedirectTo);
            }

            return component;
        }).WithName(routeName);

        var authorizeAttributes = htmxComponent.GetCustomAttributes(typeof(AuthorizeAttribute), true);
        var anonymousAttributes = htmxComponent.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);

        if (!anonymousAttributes.Any() && authorizeAttributes.Any())
        {
            endpoint.RequireAuthorization();
        }
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