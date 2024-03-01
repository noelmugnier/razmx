namespace Razmx.Core;

public class HtmxLink : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "a");

        foreach (var attribute in AdditionalAttributes)
        {
            builder.AddAttribute(1, attribute.Key, attribute.Value);
        }

        builder.AddContent(2, ChildContent);
        builder.CloseElement();

        base.BuildRenderTree(builder);
    }
}