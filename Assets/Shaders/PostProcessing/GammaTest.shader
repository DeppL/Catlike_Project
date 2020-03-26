Shader "Unlit/GammaTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags { "RenderType"="Opaque" }
            ZTest Always
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            #pragma target 3.0

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD;
            };
            struct Interpolators {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD;
            };

            Interpolators MyVertexProgram ( VertexData input )
            {
                Interpolators o;
                o.position = UnityObjectToClipPos(input.position);
                o.normal = UnityObjectToWorldNormal(input.normal);
                o.uv = input.uv;
                return o;
            }

            float4 MyFragmentProgram ( Interpolators input ) : SV_TARGET
            {
                // float3 color = input.normal * 0.5 + 0.5;
                float c = input.uv.x;
                c = pow(c, 2.2);
                return float4(c, c, c, 1.0);
            }
            ENDCG
        }
    }
}
