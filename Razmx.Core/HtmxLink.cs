namespace Razmx.Core;

public class HtmxLink : HtmxComponent
{
    [Parameter] public string? CssClass { get; set; }
    [Parameter] public string Route { get; set; }
    [Parameter] public string HxTarget { get; set; } = HtmxMainRouter.Id;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, "a");
            builder.AddAttribute(2, "href", Route);
            builder.AddAttribute(2, "hx-target", HxTarget);
            builder.AddAttribute(3, "class", CssClass);
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        };

        return fragment;
    }
}