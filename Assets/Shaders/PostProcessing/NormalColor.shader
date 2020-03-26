Shader "Custom/NormalColor"
{
    SubShader
    {
        pass
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
            };
            struct Interpolators {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;
                // float3 color : TEXCOORD0;
            };

            Interpolators MyVertexProgram ( VertexData input )
            {
                Interpolators o;
                o.position = UnityObjectToClipPos(input.position);
                o.normal = UnityObjectToWorldNormal(input.normal);
                return o;
            }

            float4 MyFragmentProgram ( Interpolators input ) : SV_TARGET
            {
                float3 color = input.normal * 0.5 + 0.5;
                return float4(color, 1.0);
            }
            ENDCG
        }

        // Pass {
		// 	CGPROGRAM

		// 	#pragma vertex MyVertexProgram
		// 	#pragma fragment MyFragmentProgram

		// 	#include "UnityCG.cginc"

		// 	float4 _Tint;
		// 	sampler2D _MainTex;
		// 	float4 _MainTex_ST;

		// 	struct VertexData {
		// 		float4 position : POSITION;
        //         float3 normal : NORMAL;
		// 	};

		// 	struct Interpolators {
		// 		float4 position : SV_POSITION;
        //         float3 color : TEXCOORD1;
		// 	};

		// 	Interpolators MyVertexProgram (VertexData v) {
		// 		Interpolators i;
		// 		i.position = UnityObjectToClipPos(v.position);
        //         i.color = v.normal * 0.5 + float3(0.5, 0.5, 0.5);
		// 		return i;
		// 	}

		// 	float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
        //         return float4(i.color, 1.0);
		// 	}

		// 	ENDCG
		// }
	}
    // FallBack "Diffuse"
}
