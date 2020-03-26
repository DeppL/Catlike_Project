using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderingPipelineSwitch : MonoBehaviour
{

    [SerializeField]
    UnityEngine.Experimental.Rendering.RenderPipelineAsset myAsset;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            GraphicsSettings.renderPipelineAsset = GraphicsSettings.renderPipelineAsset ? null : myAsset;
        }
    }
}
