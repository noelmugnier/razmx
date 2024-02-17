namespace Razmx.Core;

public abstract class HtmxRouter : HtmxComponent
{
    private string _identifier = default!;
    public string HtmlTag { get; internal set; } = "div";

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public string? CssClass { get; set; }

    protected void SetIdentifier(string identifier)
    {
        _identifier = identifier;
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, HtmlTag);
            builder.AddAttribute(2, "id", _identifier);
            builder.AddAttribute(3, "class", CssClass);
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        };

        return fragment;
    }
}