namespace Razmx.Core;

public class Hidden : Input
{
    protected override RenderFragment GenerateFragmentToRender()
    {
        RenderFragment fragment = builder =>
        {
            AdditionalAttributes["type"] = "hidden";
            builder.OpenElement(1, "input");

            foreach (var attribute in AdditionalAttributes)
            {
                builder.AddAttribute(2, attribute.Key, attribute.Value);
            }

            builder.CloseElement();
        };

        return fragment;
    }
}