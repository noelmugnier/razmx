namespace Razmx.Core;

public class HtmxFormComponentResult<TPage, TFormModel> : RazorComponentResult
    where TPage : IFormState<TFormModel>
{
    public HtmxFormComponentResult() : base(typeof(TPage),
        new ModelState<TFormModel>(default, new Dictionary<string, string>()))
    {
    }

    public HtmxFormComponentResult(ModelState<TFormModel> formState) : base(typeof(TPage),
        new Dictionary<string, object?>
        {
            ["State"] = formState
        })
    {
    }
}