Shader "Hidden/FogEffect"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _FogColor ("FogColor", Color) = (1, 1, 1, 1)
        _FogDensity ("FogDensity", float) = 1.0
        _FogStart ("FogStart", float) = 0
        _FogEnd ("FogEnd", float) = 1.0
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            
            #pragma target 3.0
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float _FogDensity;
            float3 _FogColor;
            float _FogStart;
            float _FogEnd;

            sampler2D _CameraDepthTexture;
            float4x4 _VP_I_Matrix;
            float4x4 _FrustumCornersRay;


            struct VertexData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
                float4 interpolaterRay : TEXCOORD2;
            };

            v2f MyVertexProgram (VertexData i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                o.uv_depth = i.uv;
            #if UNITY_UV_STARTS_TOP
                if (_MainTex_TexelSize.y < 0) {
                    o.uv_Depth.y = 1 - o.uv_Depth.y;
                }
            #endif

                int index = 0;
                if (o.uv_depth.x > 0.5 && o.uv_depth.y > 0.5) {
                    index = 0;
                }
                else if (o.uv_depth.x < 0.5 && o.uv_depth.y > 0.5) {
                    index = 1;
                }
                else if (o.uv_depth.x < 0.5 && o.uv_depth.y < 0.5) {
                    index = 2;
                }
                else if (o.uv_depth.x > 0.5 && o.uv_depth.y < 0.5) {
                    index = 3;
                }
                o.interpolaterRay = _FrustumCornersRay[index];
                return o;
            }

            float4 MyFragmentProgram (v2f i) : SV_TARGET
            {
                float linearDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth));
                float3 worldPos = _WorldSpaceCameraPos + linearDepth * i.interpolaterRay.xyz;

                float density = (_FogEnd - worldPos.y ) / (_FogEnd - _FogStart);
                density = saturate(density * _FogDensity);

                float4 color = tex2D(_MainTex, i.uv);
                color.rgb = lerp(color.rgb, _FogColor.rgb, density);

                return color;
            }
            ENDCG
        }

    }
    FallBack Off
}
