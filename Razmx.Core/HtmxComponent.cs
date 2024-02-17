namespace Razmx.Core;

public abstract class HtmxComponent: ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var fragment = GenerateFragmentToRender();
        builder.AddContent(0, fragment);

        base.BuildRenderTree(builder);
    }

    protected abstract RenderFragment GenerateFragmentToRender();
}