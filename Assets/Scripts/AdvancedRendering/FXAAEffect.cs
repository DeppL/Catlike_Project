using UnityEngine;
using System;
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FXAAEffect : MonoBehaviour
{
    const int LUMINANCE_PASS = 0;
    const int FXAA_PASS = 1;
    
    [HideInInspector]
    public Shader fxaaShader;
    [NonSerialized]
    Material fxaaMaterial;
    public LuminanceMode luminanceSource;
    [Range(0.0312f, 0.0833f)]
    public float contrastThreshold = 0.0312f;
    [Range(0.063f, 0.333f)]
    public float relativeThreshold = 0.063f;
    [Range(0f, 1f)]
    public float subpixelBlending = 1f;
    public bool lowQuality;
    public bool gammaBlending;

    public enum LuminanceMode {
        Alpha, Green, Calculate
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if (fxaaMaterial == null)
        {
            fxaaMaterial = new Material(fxaaShader);
            fxaaMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        fxaaMaterial.SetFloat("_ContrastThreshold", contrastThreshold);
        fxaaMaterial.SetFloat("_RelativeThreshold", relativeThreshold);
        fxaaMaterial.SetFloat("_SubpixelBlending", subpixelBlending);

        if (lowQuality) {
            fxaaMaterial.EnableKeyword("LOW_QUALITY");
        }
        else {
            fxaaMaterial.DisableKeyword("LOW_QUALITY");
        }
        if (gammaBlending) {
            fxaaMaterial.EnableKeyword("GAMMA_BLENDING");
        }
        else {
            fxaaMaterial.DisableKeyword("GAMMA_BLENDING");
        }
        
        if (luminanceSource == LuminanceMode.Calculate) {
            fxaaMaterial.DisableKeyword("LUMINANCE_GREEN");
            RenderTexture luminanceTex = RenderTexture.GetTemporary(
                src.width, src.height, 0, src.format
            );
            Graphics.Blit(src, luminanceTex, fxaaMaterial, LUMINANCE_PASS);
            Graphics.Blit(luminanceTex, dest, fxaaMaterial, FXAA_PASS);
            RenderTexture.ReleaseTemporary(luminanceTex);
        }
        else {
            if (luminanceSource == LuminanceMode.Green) {
                fxaaMaterial.EnableKeyword("LUMINANCE_GREEN");
            }
            else {
                fxaaMaterial.DisableKeyword("LUMINANCE_GREEN");
            }
            Graphics.Blit(src, dest, fxaaMaterial, FXAA_PASS);
        }
        
    }
    
}
