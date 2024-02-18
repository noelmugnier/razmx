namespace Razmx.Core;

public class RedirectToIfAuthenticatedAttribute : Attribute
{
    public RedirectToIfAuthenticatedAttribute(string redirectTo)
    {
        RedirectTo = redirectTo;
    }

    public string RedirectTo { get; set; }
}