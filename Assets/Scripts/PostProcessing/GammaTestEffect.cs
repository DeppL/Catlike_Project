using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GammaTestEffect : PostEffectsBase
{
    
    [HideInInspector] public Shader testShader;
    private Material testMaterial;
    public Material material {
        get {
            testMaterial = CheckShaderAndCreateMaterial(testShader, testMaterial);
            return testMaterial;
        }
    }
    private void OnEnable() {
        
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, material);
    }
}
