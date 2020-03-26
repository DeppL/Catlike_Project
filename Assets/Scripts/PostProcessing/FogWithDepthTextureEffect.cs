using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogWithDepthTextureEffect : PostEffectsBase
{
    [HideInInspector]
    public Shader fogShader;
    private Material fogMaterial;
    public Material material {
        get {
            fogMaterial = CheckShaderAndCreateMaterial(fogShader, fogMaterial);
            return fogMaterial;
        }
    }
    private Camera myCamera;
    public Camera camera {
        get {
            if(myCamera == null) {
                myCamera = GetComponent<Camera>();
            }
            return myCamera;
        }
    }
    private Transform myCameraTransform;
    public Transform cameraTransform {
        get {
            if (myCameraTransform == null) {
                myCameraTransform = camera.transform;
            }
            return myCameraTransform;
        }
    }

    [Range(0, 3.0f)]
    public float _FogDensity = 1.0f;
    public Color _FogColor = Color.white;
    public float _FogStart = 0f;
    public float _FogEnd = 2.0f;
    private void OnEnable() {
        camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (material == null) {
            Graphics.Blit(src, dest);
        }
        else {
            float near = camera.nearClipPlane;
            float far = camera.farClipPlane;
            float fov = camera.fieldOfView;
            float aspect = camera.aspect;

            float halfHeight = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * near;
            Vector3 toRight = cameraTransform.right * halfHeight * aspect;
            Vector3 toTop = cameraTransform.up * halfHeight;

            Vector3 topLeft = cameraTransform.forward * near + toTop - toRight;
            float scale = topLeft.magnitude / near;
            
            topLeft.Normalize();
            topLeft *= scale;

            Vector3 topRight = cameraTransform.forward * near + toTop + toRight;
            topRight.Normalize();
            topRight *= scale;

            Vector3 bottomLeft = cameraTransform.forward * near - toTop - toRight;
            bottomLeft.Normalize();
            bottomLeft *= scale;

            Vector3 bottomRight = cameraTransform.forward * near - toTop + toRight;
            bottomRight.Normalize();
            bottomRight *= scale;

            Matrix4x4 frustumCorners = Matrix4x4.identity;
            frustumCorners.SetRow(0, topRight);
            frustumCorners.SetRow(1, topLeft);
            frustumCorners.SetRow(2, bottomLeft);
            frustumCorners.SetRow(3, bottomRight);

            material.SetMatrix("_FrustumCornersRay", frustumCorners);
            material.SetMatrix("_VP_I_Matrix", (camera.projectionMatrix * camera.worldToCameraMatrix).inverse);

            material.SetFloat("_FogDensity", _FogDensity);
            material.SetFloat("_FogStart", _FogStart);
            material.SetFloat("_FogEnd", _FogEnd);
            material.SetColor("_FogColor", _FogColor);

            Graphics.Blit(src, dest, material);
        }
    }
}
