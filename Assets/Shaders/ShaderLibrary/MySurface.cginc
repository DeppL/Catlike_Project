#ifndef MY_SURFACE_INCLUDED
#define MY_SURFACE_INCLUDED

struct SurfaceData {
	float3 albedo;
	float3 emission;
	float3 normal;
	float alpha;
	float metallic;
	float occlusion;
	float smoothness;
};

struct SurfaceParameters {
	float3 normal, position;
	float4 uv;
};

#endif // MY_SURFACE_INCLUDED