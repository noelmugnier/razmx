namespace Razmx.Core;

public class HtmxMainRouter : HtmxRouter
{
    private const string _identifier = "main";

    public HtmxMainRouter()
    {
        HtmlTag = _identifier;
    }

    public static string Id => _identifier;
}