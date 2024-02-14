using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Razmx.App.Pages;

namespace Razmx.App;

public static class HtmxRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHtmxRoutes(this IEndpointRouteBuilder endpoints)
    {
        var assemblies = typeof(Main).Assembly.GetTypes();
        var htmxPages = assemblies.Where(assembly => typeof(HtmxPage).IsAssignableFrom(assembly));
        foreach (var htmxPage in htmxPages)
        {
            var routeAttribute = htmxPage.GetCustomAttribute<RouteAttribute>(true);
            if (routeAttribute is null)
            {
                continue;
            }

            var routeTemplate = routeAttribute!.Template;
            var routeName = htmxPage.FullName!;

            RegisterEndpoint(endpoints, htmxPage, routeTemplate, routeName);
        }

        return endpoints;
    }

    public static IEndpointRouteBuilder MapRootComponent<TComponent>(this IEndpointRouteBuilder endpoints, string defaultRouteTemplate = "/", string defaultRouteName = "Home") where TComponent : HtmxPage
    {
        var htmxComponent = typeof(TComponent);
        RegisterEndpoint(endpoints, htmxComponent, defaultRouteTemplate, defaultRouteName);

        return endpoints;
    }

    private static void RegisterEndpoint(IEndpointRouteBuilder endpoints, Type htmxComponent, string routeTemplate,
        string routeName)
    {
        var authorizeAttributes = htmxComponent.GetCustomAttributes(typeof(AuthorizeAttribute), true);
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

        if (authorizeAttributes.Any())
        {
            endpoint.RequireAuthorization();
        }
    }
}