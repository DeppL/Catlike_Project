using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.Rendering;

[CustomEditor(typeof(MyPipelineAsset))]
public class MyPipelineAssetEditor : Editor
{
    SerializedProperty shadowCascades;
    SerializedProperty towCascadesSplit;
    SerializedProperty fourCascadesSplit;

    private void OnEnable() {
        shadowCascades = serializedObject.FindProperty("shadowCascades");
        towCascadesSplit = serializedObject.FindProperty("twoCascadesSplit");
        fourCascadesSplit = serializedObject.FindProperty("fourCascadesSplit");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        switch (shadowCascades.enumValueIndex)
        {
            case 0 : return;
            case 1 : 
                CoreEditorUtils.DrawCascadeSplitGUI<float>(ref towCascadesSplit);
                break;
            case 2 : 
                CoreEditorUtils.DrawCascadeSplitGUI<Vector3>(ref fourCascadesSplit);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
