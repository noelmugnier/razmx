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
                                                $"parameter of type {nameof(ModelState)}. For example, you can use {nameof(HtmxValidationFor)} " +
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
            var classes = string.Empty;
            if (AdditionalAttributes.TryGetValue("class", out var existingClasses))
            {
                classes = existingClasses + " ";
            }

            if (State.Errors.Any() && State.Errors.ContainsKey(Name))
            {
                classes += " invalid";
                AdditionalAttributes["aria-invalid"] = "true";
            }
            else
            {
                classes += " valid";
            }

            AdditionalAttributes["class"] = classes;

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