Shader "Custom/ProtanopiaFull"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            // valores vindos do script
            float _Fade;
            float _Intensity;

            float _Exposure;
            float _Contrast;
            float _Saturation;
            float _Gamma;
            float4 _Tint;

            float _VStrength;
            float _VSmooth;

            float _Blur;

            // ---------- PROTANOPIA ----------
            fixed3 Protanopia(fixed3 c)
            {
                float3x3 M =
                float3x3(
                    0.567f, 0.433f, 0.0f,
                    0.558f, 0.442f, 0.0f,
                    0.0f,   0.242f, 0.758f
                );

                return mul(M, c);
            }

            float CalcLuma(float3 c)
            {
                return dot(c, float3(0.299, 0.587, 0.114));
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float3 col = tex2D(_MainTex, uv).rgb;

                // ---------- DESFOQUE SUAVE NAS BORDAS ----------
                if (_Blur > 0.001)
                {
                    float2 dir = uv - 0.5;
                    float dist = length(dir) * 2.0;
                    float blurAmount = saturate(dist * _Blur);

                    float3 blurred =
                        tex2D(_MainTex, uv + dir * 0.002).rgb +
                        tex2D(_MainTex, uv - dir * 0.002).rgb +
                        tex2D(_MainTex, uv + dir * 0.004).rgb +
                        tex2D(_MainTex, uv - dir * 0.004).rgb;

                    blurred /= 4.0;

                    col = lerp(col, blurred, blurAmount);
                }

                // backup para fade
                float3 original = col;

                // ---------- PROTANOPIA ----------
                float3 prot = Protanopia(col);

                // ---------- PÓS-PROCESSO ----------
                prot *= _Exposure;
                prot = ((prot - 0.5) * _Contrast) + 0.5;

                float lum = CalcLuma(prot);
                prot = lerp(lum.xxx, prot, _Saturation);

                prot = pow(prot, 1.0 / _Gamma);
                prot *= _Tint.rgb;

                // ---------- INTENSIDADE DO EFEITO ----------
                prot = lerp(original, prot, _Intensity);

                // ---------- FADE SUAVE ----------
                col = lerp(original, prot, _Fade);

                // ---------- VINHETA ----------
                float2 p = uv * 2 - 1;
                float d = length(p);
                float vign = smoothstep(_VStrength, _VStrength + _VSmooth, d);
                col *= (1 - vign);

                return float4(col, 1);
            }

            ENDCG
        }
    }
}
