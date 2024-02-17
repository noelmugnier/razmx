using System.Text.Json;

namespace Razmx.Core;

public class HtmxAction : HtmxComponent
{
    [CascadingParameter] private HttpContext? HttpContext { get; set; }

    [Parameter] public string? CssClass { get; set; }
    [Parameter] public string Url { get; set; } = default!;
    [Parameter] public string HtmlType { get; set; } = "button";
    [Parameter] public HttpVerb Method { get; set; }
    [Parameter] public string HxTarget { get; set; } = HtmxMainRouter.Id;
    [Parameter] public string? HxTrigger { get; set; }
    [Parameter] public HxSwap? HxSwap { get; set; }
    [Parameter] public string? HxIndicator { get; set; }
    [Parameter] public string? HxConfirm { get; set; }
    [Parameter] public string? HxRetarget { get; set; }
    [Parameter] public IDictionary<string, object> HxHeaders { get; set; } = new Dictionary<string, object>();
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

            if(HtmlType == "button")
            {
                builder.AddAttribute(0, "type", "button");
            }

            switch (Method)
            {
                case HttpVerb.GET:
                    builder.AddAttribute(1, "hx-get", Url);
                    break;
                case HttpVerb.POST:
                    builder.AddAttribute(1, "hx-post", Url);
                    break;
                case HttpVerb.PUT:
                    builder.AddAttribute(1, "hx-put", Url);
                    break;
                case HttpVerb.PATCH:
                    builder.AddAttribute(1, "hx-patch", Url);
                    break;
                case HttpVerb.DELETE:
                    builder.AddAttribute(1, "hx-delete", Url);
                    break;
                default:
                    throw new InvalidOperationException("Invalid method");
            }

            builder.AddAttribute(2, "hx-target", HxTarget);
            builder.AddAttribute(3, "class", CssClass);

            if(HxSwap != null)
            {
                builder.AddAttribute(4, "hx-swap", HxSwap.ToString());
            }

            if(!string.IsNullOrWhiteSpace(HxIndicator))
            {
                builder.AddAttribute(5, "hx-indicator", HxIndicator);
            }

            if(!string.IsNullOrWhiteSpace(HxTrigger))
            {
                builder.AddAttribute(6, "hx-trigger", HxTrigger);
            }

            if(!string.IsNullOrWhiteSpace(HxConfirm))
            {
                builder.AddAttribute(7, "hx-confirm", HxConfirm);
            }

            if(!string.IsNullOrWhiteSpace(HxRetarget))
            {
                if(HxHeaders.ContainsKey("hx-retarget"))
                {
                    HxHeaders["hx-retarget"] = HxRetarget;
                }
                else
                {
                    HxHeaders.Add("hx-retarget", HxRetarget);
                }
            }

            if (HxHeaders.Count != 0)
            {
                builder.AddAttribute(8, "hx-headers", JsonSerializer.Serialize(HxHeaders));
            }

            if(HtmlType != "input")
            {
                builder.AddContent(9, ChildContent);
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