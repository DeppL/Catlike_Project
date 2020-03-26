Shader "Custom/PP_GrayScale"
{
    Properties 
    {
        _MainTex("MainTex", 2D) = "black" {}
        intensity("Intensity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Off
        ZTest Always
        ZWrite Off
        
        Pass
        {
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            sampler2D _MainTex;
            float intensity;

            struct VertexData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct Interpolators {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Interpolators MyVertexProgram (VertexData i)
            {
                Interpolators o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                return o;
            }

            float4 grayScale(float4 c)
            {
                float g = c.r * 0.299 + c.g * 0.587 + c.b * 0.114;
                return float4(g, g, g, c.a);
            }
            float4 MyFragmentProgram(Interpolators i) : SV_TARGET
            {
                float4 t = tex2D(_MainTex, i.uv);
                return lerp(t, grayScale(t), intensity);
            }

            ENDCG
        }
    }
}
