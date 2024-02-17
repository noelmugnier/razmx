namespace Razmx.Core;

public class HtmxMainRouter : HtmxRouter
{
    private const string FixedIdentifier = "main-router";

    public HtmxMainRouter()
    {
        SetIdentifier(FixedIdentifier);
        HtmlTag = "main";
    }

    public static string Id => $"#{FixedIdentifier}";
}