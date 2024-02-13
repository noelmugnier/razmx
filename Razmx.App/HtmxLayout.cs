using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Razmx.App;

public class HtmxLayout : IComponent
{
    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private static readonly RenderFragment EmptyRenderFragment = builder =>
    {
        builder.OpenElement(0, "title");
        builder.AddAttribute(1, "hx-swap-oob", "true");
        builder.AddContent(2, "This is title");
        builder.CloseElement();
    };

    private RenderHandle _renderHandle;

    /// <summary>
    /// Gets or sets the content to display.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the type of the layout in which to display the content.
    /// The type must implement <see cref="IComponent"/> and accept a parameter named <see cref="LayoutComponentBase.Body"/>.
    /// </summary>
    [Parameter]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public Type Layout { get; set; } = default!;

    /// <inheritdoc />
    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
    }

    /// <inheritdoc />
    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        Render();
        return Task.CompletedTask;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2072",
        Justification =
            "Layout components are preserved because the LayoutAttribute constructor parameter is correctly annotated.")]
    private void Render()
    {
        var isHtmxRequest = HttpContextAccessor.HttpContext?.Request.Headers["HX-request"].FirstOrDefault() == "true";

        RenderFragment customFragment = ChildContent;
        if (ChildContent.Target is HtmxPage page && isHtmxRequest)
        {
            customFragment = builder =>
            {
                builder.OpenElement(0, "title");
                builder.AddAttribute(1, "hx-swap-oob", "true");
                builder.AddContent(2, page.Title);
                builder.CloseElement();
                builder.AddContent(3, ChildContent);
            };
        }

        // Then repeatedly wrap that in each layer of nested layout until we get
        // to a layout that has no parent
        var layoutType = isHtmxRequest ? null : Layout;
        while (layoutType != null)
        {
            customFragment = WrapInLayout(layoutType, customFragment);
            layoutType = GetParentLayoutType(layoutType);
        }

        _renderHandle.Render(customFragment);
    }

    private static RenderFragment WrapInLayout(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type layoutType, RenderFragment bodyParam)
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

public class HtmxPage : HtmxComponent
{
    public string Title { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> QueryParams { get; set; }
}

public class HtmxComponent : ComponentBase
{
}