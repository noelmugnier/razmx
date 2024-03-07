namespace Razmx.Core;

public class HtmxFormComponentResult<TPage, TFormModel> : RazorComponentResult
{
    public HtmxFormComponentResult() : this(default(TFormModel)!)
    {
    }

    public HtmxFormComponentResult(TFormModel formState, Dictionary<string, string>? errors = null) : base(typeof(TPage),
        new Dictionary<string, object?>
        {
            ["State"] = FormState.Init(formState, errors)
        })
    {
    }
}