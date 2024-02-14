using Microsoft.AspNetCore.Components;
using Razmx.App.Components;

namespace Razmx.App;

public class HtmxPage : HtmxComponent
{
    public string Title { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> QueryParams { get; set; }
}