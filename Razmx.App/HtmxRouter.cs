using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Razmx.App;

public class HtmxRouter : IComponent
{
    private RenderHandle _renderHandle;

    /// <summary>
    /// Gets or sets the content to display.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public string? CssClass { get; set; }

    [Parameter]
    public string RouterId { get; set; } = "main-router";

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
        // In the middle goes the supplied content
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