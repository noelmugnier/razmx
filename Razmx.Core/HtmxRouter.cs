namespace Razmx.Core;

public abstract class HtmxRouter : HtmxComponent
{
    protected string HtmlTag { get; init; } = "div";

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, HtmlTag);

            foreach (var attribute in AdditionalAttributes)
            {
                builder.AddAttribute(2, attribute.Key, attribute.Value);
            }

            builder.AddContent(3, ChildContent);
            builder.CloseElement();
        };

        return fragment;
    }
}