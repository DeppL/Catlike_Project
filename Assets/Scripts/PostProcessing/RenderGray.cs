using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderGray : MonoBehaviour
{
    [SerializeField]
    Material mat;
    private void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if (mat != null) {
            Graphics.Blit(src, dest, mat);
        }
    }
}
