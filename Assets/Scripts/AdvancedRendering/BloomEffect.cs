using UnityEngine;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class BloomEffect : MonoBehaviour
{
    public Shader bloomShader;
    [Range(0, 10)]
    public float intensity = 1;
    [Range(1, 16)]
    public int iterations = 1;
    [Range(0, 10)]
    public float threshold = 1;
    [Range(0, 1)]
    public float softThreshold = 0.5f;
    public bool debug;
    RenderTexture[] textures = new RenderTexture[16];
    [NonSerialized]
    Material bloom;


    const int BOX_DOWN_PREFILTER_PASS = 0;
    const int BOX_DOWN_PASS = 1;
    const int BOX_UP_PASS = 2;
    const int APPLY_BLOOM_PASS = 3;
    const int DEBUG_BLOOM_PASS = 4;
    
    

    private void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if (bloom == null) {
            bloom = new Material(bloomShader);
            bloom.hideFlags = HideFlags.HideAndDontSave;
        }
        // bloom.SetFloat("_Threshold", threshold);
        // bloom.SetFloat("_SoftThreshold", softThreshold);
        float knee = threshold * softThreshold;
		Vector4 filter;
		filter.x = threshold;
		filter.y = filter.x - knee;
		filter.z = 2f * knee;
		filter.w = 0.25f / (knee + 0.00001f);
		bloom.SetVector("_Filter", filter);
        bloom.SetFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));
        int width = src.width / 2;
        int height = src.height / 2;
        RenderTextureFormat format = src.format;

        RenderTexture currentDestination = textures[0] =
            RenderTexture.GetTemporary(width, height, 0, format);
        Graphics.Blit(src, currentDestination, bloom, BOX_DOWN_PREFILTER_PASS);
        RenderTexture currentSource = currentDestination;
        
        int i = 1;
        for (; i < iterations; i++) {
            width /= 2;
            height /= 2;
            if (height < 2) { break; }
            currentDestination = textures[i] =
                RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(currentSource, currentDestination, bloom, BOX_DOWN_PASS);
            currentSource = currentDestination;
        }
        for (i -= 2; i >= 0; i--) {
            currentDestination = textures[i];
            textures[i] = null;
            Graphics.Blit(currentSource, currentDestination, bloom, BOX_UP_PASS);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }
        // Graphics.Blit(currentSource, dest, bloom, BOX_UP_PASS);
        if (debug) {
            Graphics.Blit(currentSource, dest, bloom, DEBUG_BLOOM_PASS);
        }
        else {
            bloom.SetTexture("_SourceTex", src);
            Graphics.Blit(currentSource, dest, bloom, APPLY_BLOOM_PASS);
        }
        RenderTexture.ReleaseTemporary(currentSource);
    }
}
