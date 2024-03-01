namespace Razmx.Core;

public class HtmxValidationSummary : HtmxComponent
{
    [CascadingParameter] private ModelState? State { get; set; }
    [Parameter] public string Message { get; set; } = "The following errors occured";
    [Parameter] public bool ShowModelPropertyErrors { get; set; }
    [Parameter] public bool ExpandSummary { get; set; } = true;

    protected override void OnInitialized()
    {
        if (State == null)
        {
            throw new InvalidOperationException($"{nameof(State)} requires a cascading " +
                                                $"parameter of type {nameof(ModelState)}. For example, you can use {nameof(HtmxValidationSummary)} " +
                                                $"inside an HtmxForm.");
        }

        base.OnInitialized();
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            if(State!.Errors.Count == 0)
            {
                return;
            }

            if ((!ShowModelPropertyErrors || State.Errors.Count <= 0) && (ShowModelPropertyErrors || State.Errors.All(x => State
                    .ToExpando()
                    .ContainsKey(x.Key))))
            {
                return;
            }

            builder.OpenElement(0, "details");
            builder.AddAttribute(1, "class", "errors-summary");
            if(ExpandSummary)
            {
                builder.AddAttribute(2, "open", "true");
            }

            builder.AddAttribute(1, "class", "errors-summary");
            builder.OpenElement(1, "summary");
            builder.AddContent(2, Message);
            builder.CloseElement();

            builder.OpenElement(3, "ol");
            var model = State.ToExpando();

            foreach (var error in State.Errors)
            {
                if (!ShowModelPropertyErrors && model.ContainsKey(error.Key))
                {
                    continue;
                }

                builder.OpenElement(4, "li");
                builder.AddContent(5, error.Value);
                builder.CloseElement();
            }
            builder.CloseElement();
            builder.CloseElement();
        };

        return fragment;
    }
}