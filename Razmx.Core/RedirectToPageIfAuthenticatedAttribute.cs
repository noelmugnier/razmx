namespace Razmx.Core;

public class RedirectToPageIfAuthenticatedAttribute<T> : RedirectToIfAuthenticatedAttribute
    where T: IComponent
{
    public RedirectToPageIfAuthenticatedAttribute() : base(typeof(T).FullName)
    {
    }
}