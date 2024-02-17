namespace Razmx.Core;

public class HtmxValidationErrors : HtmxComponent
{
    [CascadingParameter] private ModelState State { get; set; }
    [Parameter] public string Message { get; set; } = "Please correct the following errors:";
    [Parameter] public bool ShowModelErrors { get; set; }

    protected override void OnInitialized()
    {
        if (State == null)
        {
            throw new InvalidOperationException($"{nameof(State)} requires a cascading " +
                                                $"parameter of type {nameof(ModelState)}. For example, you can use {nameof(HtmxValidationErrors)} " +
                                                $"inside an HtmxForm.");
        }

        base.OnInitialized();
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            if(State.Errors == null || State.Errors.Count == 0)
            {
                return;
            }

            if (ShowModelErrors && State.Errors.Count > 0 || !ShowModelErrors && State.Errors.Any(x => !State.GetModel().ToDictionary().ContainsKey(x.Key)))
            {
                builder.OpenElement(0, "div");
                builder.AddContent(2, Message);

                builder.OpenElement(3, "ol");
                var model = State.GetModel().ToDictionary();

                foreach (var error in State.Errors)
                {
                    if (!ShowModelErrors && model.ContainsKey(error.Key))
                    {
                        continue;
                    }

                    builder.OpenElement(4, "li");
                    builder.AddContent(5, error.Value);
                    builder.CloseElement();
                }
                builder.CloseElement();
                builder.CloseElement();
            }
        };

        return fragment;
    }
}