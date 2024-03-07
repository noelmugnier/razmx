namespace Razmx.Core;

public class HtmxSubmit : HtmxComponent
{
    [CascadingParameter] private FormState? Context { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "type", "submit");

            // switch (Context.Method)
            // {
            //     case HttpVerb.GET:
            //         AdditionalAttributes["hx-get"] = Context.Action;
            //         break;
            //     case HttpVerb.POST:
            //         AdditionalAttributes["hx-post"] = Context.Action;
            //         break;
            //     case HttpVerb.PUT:
            //         AdditionalAttributes["hx-put"] = Context.Action;
            //         break;
            //     case HttpVerb.PATCH:
            //         AdditionalAttributes["hx-patch"] = Context.Action;
            //         break;
            //     case HttpVerb.DELETE:
            //         AdditionalAttributes["hx-delete"] = Context.Action;
            //         break;
            //     default:
            //         throw new InvalidOperationException("Invalid method");
            // }

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