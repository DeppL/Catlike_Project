using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EdgeDetectEffect : PostEffectsBase
{
    [HideInInspector] public Shader edgeDetecShader;
    private Material edgeDetecMaterial;

    public Material material
    {
        get
        {
            edgeDetecMaterial = CheckShaderAndCreateMaterial(edgeDetecShader, edgeDetecMaterial);
            return edgeDetecMaterial;
        }
    }

    [Range(0, 1.0f)]
    public float edgesOnly = 0.0f;
    public Color edgeColor = Color.black;
    public Color backgroundColor = Color.white;
    public float sampleDistance = 1.0f;
    public float sensitivityDepth = 1.0f;
    public float sensitivityNormals = 1.0f;

    private void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material == null)
        {
            Graphics.Blit(src, dest);
        }
        else
        {
            edgeDetecMaterial.SetFloat("_EdgeOnly", edgesOnly);
            edgeDetecMaterial.SetColor("_EdgeColor", edgeColor);
            edgeDetecMaterial.SetColor("_BackgroundColor", backgroundColor);
            edgeDetecMaterial.SetFloat("_SampleDistance", sampleDistance);
            edgeDetecMaterial.SetVector("_Sensitivity", new Vector4(sensitivityNormals, sensitivityDepth, 0.0f, 0.0f));
            
            Graphics.Blit(src, dest, material);
        }
    }
}
