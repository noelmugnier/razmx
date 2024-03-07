namespace Razmx.Core;

public abstract class FormState(Dictionary<string, string>? errors = null)
{
    public string Action { get; set; } = string.Empty;
    public HttpVerb Method { get; set; } = HttpVerb.GET;
    public static FormState<T> Init<T>(T state, Dictionary<string, string>? errors = null) => new(state, errors);
    public IDictionary<string, string> Errors { get; private set; } = errors ?? new Dictionary<string, string>();
    public abstract IDictionary<string, object?> ToExpando();
    public bool HasFieldError(string fieldName) => Errors.ContainsKey(fieldName);
}

public class FormState<T>(T state, Dictionary<string, string>? errors = null) : FormState(errors)
{
    public T Model { get; } = state;
    public override IDictionary<string, object?> ToExpando()
    {
        return Model.ToExpando();
    }
}