namespace Razmx.Core;

public class HtmxValidationFor : HtmxComponent
{
    [CascadingParameter] private FormState Context { get; set; }
    [Parameter] public string For { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (Context == null)
        {
            throw new InvalidOperationException($"{nameof(Context)} requires a cascading " +
                                                $"parameter of type {nameof(FormState)}. For example, you can use {nameof(HtmxValidationFor)} " +
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
            var inputErrors = Context.Errors.Where(error => error.Key == For).Select(error => error.Value).ToList();
            if (inputErrors.Count <= 0)
            {
                return;
            }

            foreach (var error in inputErrors)
            {
                builder.OpenElement(1, "div");
                builder.AddAttribute(2, "class", "validation-message");
                builder.AddContent(3, error);
                builder.CloseElement();
            }
        };

        return fragment;
    }
}