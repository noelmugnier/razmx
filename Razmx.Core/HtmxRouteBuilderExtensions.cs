namespace Razmx.Core;

public static class HtmxRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHtmxRoutes(this IEndpointRouteBuilder endpoints, Type assemblyType, params Type[] typesFromAssemblies)
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

    public static IEndpointRouteBuilder WithRootComponent<TComponent>(this IEndpointRouteBuilder endpoints, string defaultRouteTemplate = "/", string defaultRouteName = "Home") where TComponent : IComponent
    {
        var htmxComponent = typeof(TComponent);
        RegisterEndpoint(endpoints, htmxComponent, defaultRouteTemplate, defaultRouteName);

        return endpoints;
    }

    private static void RegisterEndpoint(IEndpointRouteBuilder endpoints, Type htmxComponent, string routeTemplate,
        string routeName)
    {
        var endpoint = endpoints.MapGet(routeTemplate, (HttpContext context) =>
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

            return new RazorComponentResult(htmxComponent, parameters);
        }).WithName(routeName);

        var authorizeAttributes = htmxComponent.GetCustomAttributes(typeof(AuthorizeAttribute), true);
        if (authorizeAttributes.Any())
        {
            endpoint.RequireAuthorization();
        }

        var anonymousAttributes = htmxComponent.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
    }
}