namespace Razmx.Core;

public class RedirectToUrlIfAuthenticatedAttribute : Attribute
{
    public RedirectToUrlIfAuthenticatedAttribute(string redirectTo)
    {
        RedirectTo = redirectTo;
    }

    public string RedirectTo { get; set; }
}