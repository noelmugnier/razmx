namespace Razmx.Core;

public class HtmxInput : HtmxComponent
{
    [CascadingParameter] private FormState? Context { get; set; }
    [Parameter] public bool DisplayFieldErrors { get; set; } = true;
    [Parameter] public string Name { get; set; } = default!;
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Label { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    protected override void OnInitialized()
    {
        if (Context == null)
        {
            throw new InvalidOperationException($"{nameof(Context)} requires a cascading " +
                                                $"parameter of type {nameof(FormState)}. For example, you can use {nameof(HtmxValidationFor)} " +
                                                $"inside an HtmxForm.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"{nameof(Name)} is required on {nameof(HtmxInput)}");
        }

        base.OnInitialized();
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            var classes = string.Empty;
            if (AdditionalAttributes.TryGetValue("class", out var existingClasses))
            {
                classes = existingClasses + " ";
            }

            if (Context.Errors.Any() && Context.Errors.ContainsKey(Name))
            {
                classes += " invalid";
                AdditionalAttributes["aria-invalid"] = "true";
            }
            else
            {
                classes += " valid";
            }

            AdditionalAttributes["class"] = classes;

            AdditionalAttributes["hx-validate"] = "true";
            AdditionalAttributes["name"] = Name;
            AdditionalAttributes["id"] = Id ?? Name;

            if (!string.IsNullOrWhiteSpace(Label))
            {
                builder.OpenElement(1, "label");
                builder.AddAttribute(2, "for", Name);
                builder.AddAttribute(3, "class", classes);
                builder.AddContent(4, Label);
                builder.CloseElement();
            }

            builder.OpenElement(5, "input");

            foreach (var attribute in AdditionalAttributes)
            {
                builder.AddAttribute(6, attribute.Key, attribute.Value);
            }

            builder.CloseElement();

            if (DisplayFieldErrors)
            {
                builder.OpenComponent<HtmxValidationFor>(7);
                builder.AddComponentParameter(8, "For", Name);
                builder.CloseComponent();
            }
        };

        return fragment;
    }
}