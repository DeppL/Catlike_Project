Shader "Hidden/EdgeDetectEffect"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _EdgeOnly ("EdgeOnly", Float) = 1.0
        _EdgeColor ("EdgeColor", Color) = (0, 0, 0, 0)
        _BackgroundColor ("BackgroundColor", Color) = (1, 1, 1, 1)
        _SampleDistance ("SampleDistance", Float) = 1.0
        _Sensitivity ("Sensitivity", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #include "UnityCG.cginc"
            
            #pragma target 3.0
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            
    
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _EdgeOnly;
            float4 _EdgeColor;
            float4 _BackgroundColor;
            float _SampleDistance;
            float4 _Sensitivity;
            sampler2D _CameraDepthNormalsTexture;
    
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv[5] : TEXCOORD0;
            };

            half CheckSame (float4 center, float4 sample)
            {
                float2 centerNormal = center.xy;
                float centerDepth = DecodeFloatRG(center.zw);
                float2 sampleNormal = sample.xy;
                float sampleDepth = DecodeFloatRG(sample.zw);

                float2 diffNormal = abs(centerNormal - sampleNormal) * _Sensitivity.x;
                int isSameNormal = (diffNormal.x + diffNormal.y) < 0.1;

                float diffDepth = abs(centerDepth - sampleDepth) * _Sensitivity.y;
                int isSameDepth = diffDepth < (0.1 * centerDepth);

                return isSameNormal * isSameDepth ? 1.0 : 0.0;
            }
            
            v2f MyVertexProgram (appdata_img i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                float2 uv = i.texcoord;
                o.uv[0] = uv;

            #if UNITY_UV_STARTS_AT_UP
                if (_MainTex_TexelSize.y < 0) {
                    uv.y = 1 - uv.y;
                }
            #endif

                // o.uv[0] = uv;
                o.uv[1] = uv + _MainTex_TexelSize.xy * float2( 1,  1) * _SampleDistance;
                o.uv[2] = uv + _MainTex_TexelSize.xy * float2(-1,  1) * _SampleDistance;
                o.uv[3] = uv + _MainTex_TexelSize.xy * float2(-1, -1) * _SampleDistance;
                o.uv[4] = uv + _MainTex_TexelSize.xy * float2( 1, -1) * _SampleDistance;

                return o;
            }
            float4 MyFragmentProgram (v2f i) : SV_TARGET
            {
                float4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
                float4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
                float4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
                float4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4]);

                float edge = 1.0f;
                edge *= CheckSame(sample1, sample3);
                edge *= CheckSame(sample2, sample4);

                float4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[0]), edge);
                float4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);

                return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
            }
            ENDCG
        }

    }
    FallBack Off
}
