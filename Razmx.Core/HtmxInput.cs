namespace Razmx.Core;

public class HtmxInput : HtmxComponent
{
    [CascadingParameter] private ModelState State { get; set; }
    [Parameter] public bool ShowErrors { get; set; } = true;
    [Parameter] public string Name { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
    protected override void OnInitialized()
    {
        if (State == null)
        {
            throw new InvalidOperationException($"{nameof(State)} requires a cascading " +
                                                $"parameter of type {nameof(ModelState)}. For example, you can use {nameof(HtmxInputErrors)} " +
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
            var classes = AdditionalAttributes.Where(attr => attr.Key == "class").Select(attr => attr.Value.ToString()).ToList();

            if (State.Errors.Any())
            {
                classes.Add(State.Errors.Keys.Contains(Name) ? "invalid" : "valid");
            }

            var model = State.GetModel().ToDictionary();

            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "name", Name);
            builder.AddAttribute(2, "value", model[Name]);
            builder.AddAttribute(3, "class", string.Join(" ", classes));
            builder.CloseElement();

            if (ShowErrors)
            {
                builder.OpenComponent<HtmxInputErrors>(4);
                builder.AddComponentParameter(5, "For", Name);
                builder.CloseComponent();
            }
        };

        return fragment;
    }
}