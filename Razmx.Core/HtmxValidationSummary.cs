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

            builder.OpenElement(1, "ol");
            builder.AddAttribute(2, "class", "validation-errors");
            var model = State.ToExpando();

            foreach (var error in State.Errors)
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