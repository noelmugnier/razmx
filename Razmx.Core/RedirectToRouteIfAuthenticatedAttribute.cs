namespace Razmx.Core;

public class RedirectToRouteIfAuthenticatedAttribute : Attribute
{
    public RedirectToRouteIfAuthenticatedAttribute(string redirectTo)
    {
        RedirectTo = redirectTo;
    }

    public string RedirectTo { get; set; }
}