namespace Razmx.Core;

public class Input : HtmxComponent
{
    [CascadingParameter] private ModelState? State { get; set; }
    [Parameter] public bool DisplayFieldErrors { get; set; } = true;
    [Parameter] public string Name { get; set; } = default!;
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Label { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    protected override void OnInitialized()
    {
        if (State == null)
        {
            throw new InvalidOperationException($"{nameof(State)} requires a cascading " +
                                                $"parameter of type {nameof(ModelState)}. For example, you can use {nameof(HtmxFieldErrors)} " +
                                                $"inside an HtmxForm.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"{nameof(Name)} is required on {nameof(Input)}");
        }

        base.OnInitialized();
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            var stateClass = string.Empty;
            if (State.Errors.Any())
            {
                stateClass = State.Errors.Keys.Contains(Name) ? " is-invalid" : " is-valid";
                if (AdditionalAttributes.TryGetValue("class", out var classes))
                {
                    AdditionalAttributes["class"] = classes + stateClass;
                }
                else
                {
                    AdditionalAttributes["class"] = stateClass;
                }
            }

            AdditionalAttributes["name"] = Name;
            AdditionalAttributes["id"] = Id ?? Name;

            if (!string.IsNullOrWhiteSpace(Label))
            {
                builder.OpenElement(1, "label");
                builder.AddAttribute(2, "for", Name);
                builder.AddAttribute(3, "class", stateClass);
                builder.AddContent(4, Label);
                builder.CloseElement();
            }

            builder.OpenElement(4, "input");

            foreach (var attribute in AdditionalAttributes)
            {
                builder.AddAttribute(5, attribute.Key, attribute.Value);
            }

            builder.CloseElement();

            if (DisplayFieldErrors)
            {
                builder.OpenComponent<HtmxFieldErrors>(6);
                builder.AddComponentParameter(7, "For", Name);
                builder.CloseComponent();
            }
        };

        return fragment;
    }
}