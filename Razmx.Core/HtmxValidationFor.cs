namespace Razmx.Core;

public class HtmxValidationFor : HtmxComponent
{
    [CascadingParameter] private ModelState State { get; set; }
    [Parameter] public string For { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (State == null)
        {
            throw new InvalidOperationException($"{nameof(State)} requires a cascading " +
                                                $"parameter of type {nameof(ModelState)}. For example, you can use {nameof(HtmxValidationFor)} " +
                                                $"inside an HtmxForm.");
        }

        if (string.IsNullOrWhiteSpace(For))
        {
            throw new InvalidOperationException($"{nameof(For)} attribute is required on {nameof(HtmxValidationFor)}");
        }

        base.OnInitialized();
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            var inputErrors = State.Errors.Where(error => error.Key == For).Select(error => error.Value).ToList();
            if (inputErrors.Count <= 0)
            {
                return;
            }

            foreach (var error in inputErrors)
            {
                builder.OpenElement(2, "span");
                builder.AddAttribute(3, "class", "error-message");
                builder.AddContent(3, error);
                builder.CloseElement();
            }
        };

        return fragment;
    }
}