using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace Razmx.Core;

public class HtmxForm<T> : HtmxComponent
{
    [CascadingParameter] private HttpContext? HttpContext { get; set; } = default!;

    [Parameter] public ModelState<T>? State { get; set; }
    [Parameter] public HttpVerb Method { get; set; } = HttpVerb.POST;
    [Parameter] public string? Retarget { get; set; }
    [Parameter] public bool UseAntiforgeryToken { get; set; } = true;
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
    [Parameter] public RenderFragment<ModelState<T>> ChildContent { get; set; } = default!;

    protected override RenderFragment GenerateFragmentToRender()
    {
        AdditionalAttributes.TryGetValue("action", out var actionValue);
        var action = actionValue?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(action) && !string.IsNullOrWhiteSpace(HttpContext?.Request.Path.Value))
        {
            action = $"{HttpContext.Request.Path.Value}{HttpContext.Request.QueryString}";
        }

        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, "form");

            switch (Method)
            {
                case HttpVerb.GET:
                    AdditionalAttributes["hx-get"] = action;
                    break;
                case HttpVerb.POST:
                    AdditionalAttributes["hx-post"] = action;
                    break;
                case HttpVerb.PUT:
                    AdditionalAttributes["hx-put"] = action;
                    break;
                case HttpVerb.PATCH:
                    AdditionalAttributes["hx-patch"] = action;
                    break;
                case HttpVerb.DELETE:
                    AdditionalAttributes["hx-delete"] = action;
                    break;
                default:
                    throw new InvalidOperationException("Invalid method");
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

            if (UseAntiforgeryToken)
            {
                builder.OpenComponent<AntiforgeryToken>(2);
                builder.CloseComponent();
            }

            builder.OpenComponent<CascadingValue<ModelState<T>>>(6);
            builder.AddComponentParameter(2, "IsFixed", true);
            builder.AddComponentParameter(3, "Value", State);
            builder.AddComponentParameter(4, "ChildContent", ChildContent?.Invoke(State));
            builder.CloseComponent();
            builder.CloseElement();
        };

        return fragment;
    }
}