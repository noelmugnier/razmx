using Microsoft.AspNetCore.Components.Web;

namespace Razmx.Core;

//code adapted from LayoutView component
public class HtmxPage : IComponent
{
    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private RenderHandle _renderHandle;

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public Type Layout { get; set; } = default!;

    [Parameter]
    public string Title { get; set; } = default!;

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        Render();
        return Task.CompletedTask;
    }

    private void Render()
    {
        var customFragment = ChildContent;
        var isHtmxRequest = HttpContextAccessor.HttpContext?.Request.IsHtmxRequest() ?? false;
        if (isHtmxRequest)
        {
            customFragment = builder =>
            {
                builder.OpenElement(0, "title");
                builder.AddAttribute(1, "hx-swap-oob", "true");
                builder.AddContent(2, Title);
                builder.CloseElement();
                builder.AddContent(3, ChildContent);
            };

            HttpContextAccessor.HttpContext?.Response.Headers.TryAdd("Vary", "HX-Request");
            _renderHandle.Render(customFragment);
            return;
        }

        customFragment = builder =>
        {
            // builder.OpenComponent(0, typeof(PageTitle));
            // builder.AddAttribute(1, "ChildContent", (RenderFragment)(
            //     (pageTitleBuilder) =>
            //     {
            //         pageTitleBuilder.AddContent(2, Title);
            //     }));
            //
            // builder.CloseComponent();
            builder.AddContent(3, ChildContent);
        };

        var layoutType = Layout;
        while (layoutType != null)
        {
            customFragment = WrapInLayout(layoutType, customFragment);
            layoutType = GetParentLayoutType(layoutType);
        }

        _renderHandle.Render(customFragment);
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