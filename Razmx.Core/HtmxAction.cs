using System.Text.Json;

namespace Razmx.Core;

public class HtmxAction : HtmxComponent
{
    [CascadingParameter] private HttpContext? HttpContext { get; set; }

    [Parameter] public string Url { get; set; } = default!;
    [Parameter] public string HtmlType { get; set; } = "button";
    [Parameter] public HttpVerb Method { get; set; }
    [Parameter] public HxSwap? HxSwap { get; set; }
    [Parameter] public string? HxRetarget { get; set; }
    [Parameter] public IDictionary<string, object> HxHeaders { get; set; } = new Dictionary<string, object>();
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;

    protected override RenderFragment GenerateFragmentToRender()
    {
        if (string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(HttpContext?.Request.Path.Value))
        {
            Url = $"{HttpContext.Request.Path.Value}{HttpContext.Request.QueryString}";
        }

        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, HtmlType);

            switch (Method)
            {
                case HttpVerb.GET:
                    AdditionalAttributes["hx-get"] = Url;
                    break;
                case HttpVerb.POST:
                    AdditionalAttributes["hx-post"] = Url;
                    break;
                case HttpVerb.PUT:
                    AdditionalAttributes["hx-put"] = Url;
                    break;
                case HttpVerb.PATCH:
                    AdditionalAttributes["hx-patch"] = Url;
                    break;
                case HttpVerb.DELETE:
                    AdditionalAttributes["hx-delete"] = Url;
                    break;
                default:
                    throw new InvalidOperationException("Invalid method");
            }

            if(HtmlType == "button")
            {
                AdditionalAttributes.TryAdd("type", "button");
            }

            if (HxSwap != null)
            {
                AdditionalAttributes["hx-swap"] = HxSwap.ToString();
            }

            if(!string.IsNullOrWhiteSpace(HxRetarget))
            {
                HxHeaders["hx-retarget"] = HxRetarget;
            }

            if (HxHeaders.Count != 0)
            {
                AdditionalAttributes["hx-headers"] = JsonSerializer.Serialize(HxHeaders);
            }

            foreach (var attribute in AdditionalAttributes)
            {
                builder.AddAttribute(1, attribute.Key, attribute.Value);
            }

            if(HtmlType != "input")
            {
                builder.AddContent(2, ChildContent);
            }

            builder.CloseElement();
        };

        return fragment;
    }
}

public enum HxSwap
{
    innerHTML,
    outerHTML,
    afterbegin,
    beforebegin,
    beforeend,
    afterend,
    delete,
    none
}

public class HtmxGet : HtmxAction
{
    public HtmxGet()
    {
        Method = HttpVerb.GET;
    }
}

public class HtmxPost : HtmxAction
{
    public HtmxPost()
    {
        Method = HttpVerb.POST;
    }
}

public class HtmxPut : HtmxAction
{
    public HtmxPut()
    {
        Method = HttpVerb.PUT;
    }
}

public class HtmxDelete : HtmxAction
{
    public HtmxDelete()
    {
        Method = HttpVerb.DELETE;
    }
}

public class HtmxPatch : HtmxAction
{
    public HtmxPatch()
    {
        Method = HttpVerb.PATCH;
    }
}