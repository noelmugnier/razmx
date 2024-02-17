using System.Dynamic;

namespace Razmx.Core;

public class HtmxInputErrors : HtmxComponent
{
    [CascadingParameter] private ModelState State { get; set; }
    [Parameter] public string For { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (State == null)
        {
            throw new InvalidOperationException($"{nameof(State)} requires a cascading " +
                                                $"parameter of type {nameof(ModelState)}. For example, you can use {nameof(HtmxInputErrors)} " +
                                                $"inside an HtmxForm.");
        }

        base.OnInitialized();
    }

    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            var inputErrors = State.Errors.Where(error => error.Key == For).Select(error => error.Value).ToList();
            if (inputErrors.Count > 0)
            {
                builder.OpenElement(0, "div");
                foreach (var error in inputErrors)
                {
                    builder.OpenElement(2, "span");
                    builder.AddContent(3, error);
                    builder.CloseElement();
                }
                builder.CloseElement();
            }
        };

        return fragment;
    }
}

public static class ObjectExtensions
{
    public static ExpandoObject ToExpando<T>(this T? obj)
    {
        if (obj is null)
            return new ExpandoObject();

        var expando = new ExpandoObject();

        var type = obj.GetType();

        foreach (var propertyInfo in type.GetProperties())
        {
            var currentValue = propertyInfo.GetValue(obj);
            expando.TryAdd(propertyInfo.Name, currentValue);
        }

        return expando;
    }

    public static IDictionary<string, object?> ToDictionary(this object? obj)
    {
        return obj as IDictionary<string, object?> ?? new Dictionary<string, object?>();
    }
}