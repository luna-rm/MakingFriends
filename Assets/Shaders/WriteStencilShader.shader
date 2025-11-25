Shader "Custom/WriteStencil"
{
    // This shader's job is to do nothing *except* write to the stencil buffer.
    // It's invisible and doesn't affect depth.
    
    Properties
    {
        _StencilRef ("Stencil Reference", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend Zero One
            ZWrite Off  // 2. Don't write to the depth buffer

            // 3. This is all we do:
            Stencil
            {
                Ref [_StencilRef]
                Comp Always    // Always run this pass
                Pass Replace   // If it passes, replace the stencil value with our Ref (1)
            }
        }
    }
}