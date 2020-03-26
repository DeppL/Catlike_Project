using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderToTexture : MonoBehaviour
{
    [SerializeField]
    Material mat;
    [SerializeField]
    RenderTexture renderTex;
    [SerializeField]
    Camera renderTexCamera;
    private void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if (!mat || !renderTex) { return; }
        mat.SetTexture("_InputTex", renderTex);
        Graphics.Blit(src, dest, mat);
    }
    private void Update() {
        if (renderTex == null || 
            renderTex.width != Screen.width || 
            renderTex.height != Screen.height) {
            
            renderTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            // renderTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            renderTex.name = "MyRenderTex";
            renderTex.filterMode = FilterMode.Point;
        }
        if (renderTexCamera) {
            renderTexCamera.targetTexture = renderTex;
        }
        if (mat){
            mat.SetTexture("_MainTex", renderTex);
        }
    }
    private void OnDestroy() {
        // DestroyImmediate(renderTex);
    }
}
