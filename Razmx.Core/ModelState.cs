namespace Razmx.Core;

public abstract class ModelState(Dictionary<string, string>? errors = null)
{
    public static ModelState<T> Init<T>(T state) => new(state);
    public IDictionary<string, string> Errors { get; private set; } = errors ?? new Dictionary<string, string>();
    public abstract IDictionary<string, object?> ToExpando();
    public bool ModelHasErrorForField(string fieldName) => Errors.ContainsKey(fieldName);
}

public class ModelState<T>(T state, Dictionary<string, string>? errors = null) : ModelState(errors)
{
    public T Model { get; } = state;
    public override IDictionary<string, object?> ToExpando()
    {
        return Model.ToExpando();
    }
}

public interface IFormState<T>
{
    ModelState<T> State { get; }
}