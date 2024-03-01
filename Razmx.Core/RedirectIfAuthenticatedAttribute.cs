namespace Razmx.Core;

public class RedirectIfAuthenticatedAttribute<T>() : RedirectIfAuthenticatedAttribute(typeof(T).FullName)
    where T : IComponent;

public class RedirectIfAuthenticatedAttribute(string redirectTo) : Attribute
{
    public string RedirectTo { get; set; } = redirectTo;
}