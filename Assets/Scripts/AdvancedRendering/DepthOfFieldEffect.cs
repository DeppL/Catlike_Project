using UnityEngine;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class DepthOfFieldEffect : MonoBehaviour
{
    const int CIRCLE_OF_CONFUSION_PASS = 0;
    const int PRE_FILTER_PASS = 1;
    const int BOKEH_PASS = 2;
    const int POST_FILTER_PASS = 3;
    const int COMBINE_PASS = 4;
    
    
    [HideInInspector]
    public Shader dofShader;
    [NonSerialized]
    Material dofMaterial;
    [Range(0.1f, 100f)]
    public float focusDistance = 10f;
    [Range(0.1f, 10f)]
    public float focusRange = 3f;
    [Range(1f, 10f)]
    public float bokehRadius = 4f;

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (dofMaterial == null) {
            dofMaterial = new Material(dofShader);
            dofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        dofMaterial.SetFloat("_BokehRadius", bokehRadius);
        dofMaterial.SetFloat("_FocusDistance", focusDistance);
        dofMaterial.SetFloat("_FocusRange", focusRange);

        
        int width = src.width / 2;
        int height = src.height / 2;
        RenderTextureFormat format = src.format;
        RenderTexture coc = RenderTexture.GetTemporary(
            src.width, src.height, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );
        RenderTexture dof0 = RenderTexture.GetTemporary(width, height, 0, format);
        RenderTexture dof1 = RenderTexture.GetTemporary(width, height, 0, format);

        dofMaterial.SetTexture("_CoCTex", coc);
        dofMaterial.SetTexture("_DoFTex", dof0);

        Graphics.Blit(src, coc, dofMaterial, CIRCLE_OF_CONFUSION_PASS);
        Graphics.Blit(src, dof0, dofMaterial, PRE_FILTER_PASS);
        Graphics.Blit(dof0, dof1, dofMaterial, BOKEH_PASS);
        Graphics.Blit(dof1, dof0, dofMaterial, POST_FILTER_PASS);
        Graphics.Blit(src, dest, dofMaterial, COMBINE_PASS);

        // Graphics.Blit(coc, dest);
        RenderTexture.ReleaseTemporary(coc);
        RenderTexture.ReleaseTemporary(dof0);
        RenderTexture.ReleaseTemporary(dof1);
    }
}

