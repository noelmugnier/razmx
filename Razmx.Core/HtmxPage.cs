using Microsoft.AspNetCore.Components.Web;

namespace Razmx.Core;

//code adapted from LayoutView component
public class HtmxPage : HtmxComponent
{
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public Type Layout { get; set; } = default!;
    [Parameter] public string PageName { get; set; } = default!;
    [Parameter] public bool SensitiveData { get; set; }

    protected override RenderFragment GenerateFragmentToRender()
    {
        var customFragment = ChildContent;
        var isHtmxRequest = HttpContextAccessor.HttpContext?.Request.IsHtmxRequest() ?? false;
        var isBackHistoryRequest = HttpContextAccessor.HttpContext?.Request.IsHtmxBackHistoryRequest() ?? false;
        if (isHtmxRequest && !isBackHistoryRequest)
        {
            customFragment = builder =>
            {
                builder.OpenElement(0, "title");
                builder.AddAttribute(1, "hx-swap-oob", "true");
                builder.AddContent(2, PageName);
                builder.CloseElement();
                if (SensitiveData)
                {
                    builder.OpenElement(3, "span");
                    builder.AddAttribute(4, "hx-history", "false");
                    builder.CloseElement();
                }
                builder.AddContent(5, ChildContent);
            };

            HttpContextAccessor.HttpContext?.Response.Headers.TryAdd("Vary", "HX-Request");
            return customFragment;
        }

        customFragment = builder =>
        {
            builder.OpenComponent(0, typeof(PageTitle));
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(
                (pageTitleBuilder) => { pageTitleBuilder.AddContent(2, PageName); }));

            builder.CloseComponent();

            if (SensitiveData)
            {
                builder.OpenElement(3, "span");
                builder.AddAttribute(4, "hx-history", "false");
                builder.CloseElement();
            }

            builder.AddContent(5, ChildContent);
        };

        var layoutType = Layout;
        while (layoutType != null)
        {
            customFragment = WrapInLayout(layoutType, customFragment);
            layoutType = GetParentLayoutType(layoutType);
        }

        return customFragment;
    }

    private static RenderFragment WrapInLayout(Type layoutType, RenderFragment bodyParam)
    {
        void Render(RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, layoutType);
            builder.AddComponentParameter(1, nameof(LayoutComponentBase.Body), bodyParam);
            builder.CloseComponent();
        }

        return Render;
    }

    private static Type? GetParentLayoutType(Type type)
        => type.GetCustomAttribute<LayoutAttribute>()?.LayoutType;
}