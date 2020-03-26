// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/MotionBlurEffect"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _BlurSize ("BlurSize", float) = 0.5
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

            #pragma vertex MyVertexPrograme
            #pragma fragment MyFragmentPrograme

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;
            float4x4 _PreviousVP_Matrix;
            float4x4 _CurrentVP_I_Matrix;
            float _BlurSize;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct Interpolators {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
            };


            Interpolators MyVertexPrograme (VertexInput input)
            {
                Interpolators output;
                output.pos = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                output.uv_depth = input.uv;
            #if UNITY_UV_START_UP
                if (_MainTex_ST.y < 0) {
                    output.uv_depth.y = 1 - output.uv_depth.y;
                }
            #endif
                return output;
            }

            float4 MyFragmentPrograme (Interpolators i) : SV_TARGET
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth);
                float4 H = float4(i.uv * 2 - 1, depth * 2 - 1, 1);
                float4 D = mul(_CurrentVP_I_Matrix, H);
                float4 worldPos = D / D.w;

                float4 currentPos = H;
                float4 previouPos = mul(_PreviousVP_Matrix, worldPos);
                previouPos /= previouPos.w;

                float2 velocity = (currentPos.xy - previouPos.xy) * 0.5 * _BlurSize;
                float2 uv = i.uv;
                float3 c;
                for (int i = 0; i < 10; i++) {
                    c += tex2D(_MainTex, uv);
                    uv = saturate(uv + velocity);
                }
                c /= 10;
                // return float4(uv, 0, 1);
                return float4(c.rgb, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
