Shader "Custom/PP_ConvexGlass"
{
    Properties
    {
        intensity ("Intensity", Range(0, 200)) = 50
        _MainTex ("MainTex", 2D) = "black" {}
        _InputTex ("MyRenderTex", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Off
        ZTest Always
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct Interpolators {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _InputTex;
            float intensity;
			

            Interpolators MyVertexProgram (VertexData input)
            {
                Interpolators o;
                o.position = UnityObjectToClipPos(input.position);
                o.uv = input.uv;
                return o;
            }

            float4 MyFragmentProgram(Interpolators input) : SV_TARGET
            {
            #if UNITY_UV_STARTS_AT_TOP
                // DirectX
                float2 uv = float2(input.uv.x, 1 - input.uv.y);
            #else
                // OpenGL
                float2 uv = input.uv;
            #endif

                float2 offset = tex2D(_InputTex, uv).xy;
                // float2 offset = LinearToGammaSpace(tex2D(_InputTex, uv).xyz).xy;
                // float2 offset = GammaToLinearSpace(tex2D(_InputTex, uv).xyz).xy;

                // return float4( LinearToGammaSpace(tex2D(_InputTex, uv)), 1.0);
                // return float4( GammaToLinearSpace(tex2D(_InputTex, uv)), 1.0);
                // return float4( offset, 0, 1 );
                
                offset = pow(offset, 1 / 2.2f);
                offset = (offset.xy * 2) - 1;
                // return float4( offset, 0, 1 );
            #if UNITY_UV_STARTS_AT_TOP
                offset.y = -offset.y;
            #endif

                offset *= _MainTex_TexelSize.xy;
                offset *= intensity;

                // return float4(offset, 0, 1);
                return tex2D(_MainTex, uv + offset);
            }
            
            ENDCG
        }
    }
}
