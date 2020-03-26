using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlurWithDepthTextureEffect : PostEffectsBase
{
    [HideInInspector]
    public Shader motionBlurShader;
    private Material motionBlurMaterial = null;
    public Material material {
        get {
            motionBlurMaterial = CheckShaderAndCreateMaterial(motionBlurShader, motionBlurMaterial);
            return motionBlurMaterial;
        }
    }
    private Camera myCamera;
    public Camera camera {
        get {
            if (myCamera == null) {
                myCamera = GetComponent<Camera>();
            }
            return myCamera;
        }
    }

    [Range(0, 10)]
    public float blurSize = 0;
    private Matrix4x4 previousVP_Matrix;
    private void OnEnable() {
        camera.depthTextureMode |= DepthTextureMode.Depth;
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (material != null)
        {
            material.SetFloat("_BlurSize", blurSize);
            material.SetMatrix("_PreviousVP_Matrix", previousVP_Matrix);
            Matrix4x4 currentVP_Matrix = camera.projectionMatrix * camera.worldToCameraMatrix;
            Matrix4x4 currentVP_I_Matrix = currentVP_Matrix.inverse;
            material.SetMatrix("_CurrentVP_I_Matrix", currentVP_I_Matrix);
            previousVP_Matrix = currentVP_Matrix;
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
