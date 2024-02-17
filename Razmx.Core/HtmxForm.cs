using System.Dynamic;

namespace Razmx.Core;

public class HtmxForm<T> : HtmxComponent
{
    [CascadingParameter] private HttpContext? HttpContext { get; set; } = default!;

    [Parameter] public ModelState<T> State { get; set; }
    [Parameter] public string Action { get; set; }
    [Parameter] public HttpVerb Method { get; set; } = HttpVerb.POST;
    [Parameter] public RenderFragment<ModelState<T>> ChildContent { get; set; } = default!;

    protected override RenderFragment GenerateFragmentToRender()
    {
        if (string.IsNullOrWhiteSpace(Action) && !string.IsNullOrWhiteSpace(HttpContext?.Request.Path.Value))
        {
            Action = $"{HttpContext.Request.Path.Value}{HttpContext.Request.QueryString}";
        }

        RenderFragment fragment = builder =>
        {
            builder.OpenElement(0, "form");

            switch (Method)
            {
                case HttpVerb.GET:
                    builder.AddAttribute(1, "hx-get", Action);
                    break;
                case HttpVerb.POST:
                    builder.AddAttribute(1, "hx-post", Action);
                    break;
                case HttpVerb.PUT:
                    builder.AddAttribute(1, "hx-put", Action);
                    break;
                case HttpVerb.PATCH:
                    builder.AddAttribute(1, "hx-patch", Action);
                    break;
                case HttpVerb.DELETE:
                    builder.AddAttribute(1, "hx-delete", Action);
                    break;
                default:
                    throw new InvalidOperationException("Invalid method");
            }

            builder.OpenComponent<CascadingValue<ModelState<T>>>(6);
            builder.AddComponentParameter(7, "IsFixed", true);
            builder.AddComponentParameter(8, "Value", State);
            builder.AddComponentParameter(9, "ChildContent", ChildContent?.Invoke(State));
            builder.CloseComponent();
            builder.CloseElement();
        };

        return fragment;
    }
}

public class ModelState<T>(T state, Dictionary<string, string>? errors = null) : ModelState(errors)
{
    public T Model { get; set; } = state;
    public override ExpandoObject GetModel()
    {
        return Model.ToExpando();
    }
}

public abstract class ModelState
{
    public ModelState(Dictionary<string, string>? errors = null)
    {
        Errors = errors ?? new Dictionary<string, string>();
    }

    public static ModelState<T> Init<T>(T state) => new(state);
    public IDictionary<string, string> Errors { get; private set; }
    public abstract ExpandoObject GetModel();
}