namespace Razmx.Core;

//code adapted from LayoutView component
public abstract class HtmxPage : HtmxComponent
{
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public Type Layout { get; set; } = default!;
    [Parameter] public bool SensitiveData { get; set; }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment customFragment = builder =>
        {
            if (SensitiveData)
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "hx-history", "false");
                builder.CloseElement();
            }

            builder.AddContent(2, ChildContent);
        };

        if (HttpContextAccessor.HttpContext?.Request.RequireHtmxPartialResponse() == true)
        {
            HttpContextAccessor.HttpContext?.Response.Headers.TryAdd("Vary", "HX-Request");
            return customFragment;
        }

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