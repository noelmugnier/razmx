namespace Razmx.Core;

public class HtmxValidationSummary : HtmxComponent
{
    [CascadingParameter] private FormState? Context { get; set; }
    [Parameter] public string Message { get; set; } = "The following errors occured";
    [Parameter] public bool ShowModelPropertyErrors { get; set; }
    [Parameter] public bool ExpandSummary { get; set; } = true;

    protected override void OnInitialized()
    {
        if (Context == null)
        {
            throw new InvalidOperationException($"{nameof(Context)} requires a cascading " +
                                                $"parameter of type {nameof(FormState)}. For example, you can use {nameof(HtmxValidationSummary)} " +
                                                $"inside an HtmxForm.");
        }

        base.OnInitialized();
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            if(Context!.Errors.Count == 0)
            {
                return;
            }

            if ((!ShowModelPropertyErrors || Context.Errors.Count <= 0) && (ShowModelPropertyErrors || Context.Errors.All(x => Context
                    .ToExpando()
                    .ContainsKey(x.Key))))
            {
                return;
            }

            builder.OpenElement(1, "ol");
            builder.AddAttribute(2, "class", "validation-errors");
            var model = Context.ToExpando();

            foreach (var error in Context.Errors)
            {
                if (!ShowModelPropertyErrors && model.ContainsKey(error.Key))
                {
                    continue;
                }

                builder.OpenElement(3, "li");
                builder.AddAttribute(4, "class", "validation-message");
                builder.AddContent(5, error.Value);
                builder.CloseElement();
            }

            builder.CloseElement();
        };

        return fragment;
    }
}