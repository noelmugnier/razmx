namespace Razmx.Core;

public class HtmxRouter : IComponent
{
    private RenderHandle _renderHandle;

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public string? CssClass { get; set; }

    [Parameter]
    public string RouterId { get; set; } = "main-router";

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
        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, "main");
            builder.AddAttribute(2, "id", RouterId);
            builder.AddAttribute(3, "class", CssClass);
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        };

        _renderHandle.Render(fragment);
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