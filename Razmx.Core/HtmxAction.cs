using System.Text.Json;

namespace Razmx.Core;

public abstract class HtmxAction : ComponentBase
{
    [CascadingParameter] protected HttpContext? HttpContext { get; set; }

    protected HttpVerb Method { get; set; } = HttpVerb.GET;

    [Parameter] public string Url { get; set; } = string.Empty;
    [Parameter] public string HtmlType { get; set; } = "button";
    [Parameter] public string? Retarget { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected HtmxAction()
    {
        if (string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(HttpContext?.Request.Path.Value))
        {
            Url = $"{HttpContext.Request.Path.Value}{HttpContext.Request.QueryString}";
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
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

        if (HtmlType == "button")
        {
            AdditionalAttributes.TryAdd("type", "button");
        }

        AdditionalAttributes.TryAdd("hx-target", HtmxMainRouter.Id);

        var hxHeaders = new Dictionary<string, string>();
        if (AdditionalAttributes.TryGetValue("hx-headers", out var additionalAttribute))
        {
            hxHeaders = additionalAttribute as Dictionary<string, string> ?? new Dictionary<string, string>();
        }

        if (!string.IsNullOrWhiteSpace(Retarget))
        {
            hxHeaders["hx-retarget"] = Retarget;
        }

        if (hxHeaders.Count != 0)
        {
            AdditionalAttributes["hx-headers"] = JsonSerializer.Serialize(hxHeaders);
        }

        foreach (var attribute in AdditionalAttributes)
        {
            builder.AddAttribute(1, attribute.Key, attribute.Value);
        }

        if (HtmlType != "input")
        {
            builder.AddContent(3, ChildContent);
        }

        builder.CloseElement();

        base.BuildRenderTree(builder);
    }
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