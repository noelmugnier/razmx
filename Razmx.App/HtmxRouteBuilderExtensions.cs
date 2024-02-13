using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Razmx.App.Pages.Shared;

namespace Razmx.App;

public static class HtmxRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHtmxRoutes(this IEndpointRouteBuilder endpoints)
    {
        var assemblies = typeof(MainLayout).Assembly.GetTypes();
        var htmxComponents = assemblies.Where(assembly => typeof(HtmxComponent).IsAssignableFrom(assembly));
        foreach (var htmxComponent in htmxComponents)
        {
            var authorizeAttributes = htmxComponent.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            var routesAttributes = htmxComponent.GetCustomAttributes(typeof(RouteAttribute), true);
            foreach (var routeAttribute in routesAttributes)
            {
                var endpoint = endpoints.MapGet(((RouteAttribute)routeAttribute).Template, (HttpContext context) =>
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
                });

                if (authorizeAttributes.Any())
                {
                    endpoint.RequireAuthorization();
                }
            }
        }

        return endpoints;
    }
}