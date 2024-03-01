using Microsoft.AspNetCore.Components.Web;
using RouteData = Microsoft.AspNetCore.Components.RouteData;

namespace Razmx.Core;

public class HtmxPageTitle : ComponentBase
{
    [CascadingParameter] public HttpContext? HttpContext { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (HttpContext?.Request.RequireHtmxPartialResponse() == true)
        {
            builder.OpenElement(0, "title");
            builder.AddAttribute(1, "hx-swap-oob", "true");
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
        else
        {
            builder.OpenComponent(0, typeof(PageTitle));
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(
                pageTitleBuilder => { pageTitleBuilder.AddContent(2, ChildContent); }));

            builder.CloseComponent();
        }

        base.BuildRenderTree(builder);
    }
}