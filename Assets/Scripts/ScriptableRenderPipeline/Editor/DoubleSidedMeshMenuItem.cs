﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DoubleSidedMeshMenuItem
{
    [MenuItem("Assets/Create/Double-sided Mesh")]
    static void MakeDoubleSideMeshAsset ()
    {
        var sourceMesh = Selection.activeObject as Mesh;
        if (sourceMesh == null) {
            Debug.Log("You must have a mesh asset selected.");
            return;
        }

        Mesh insideMesh = Object.Instantiate(sourceMesh);
        int[] triangles = insideMesh.triangles;
        System.Array.Reverse(triangles);
        insideMesh.triangles = triangles;

        Vector3[] normals = insideMesh.normals;
        for (int i = 0; i < normals.Length; i++) {
            normals[i] = -normals[i];
        }
        insideMesh.normals = normals;

        var combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(
            new CombineInstance[] {
                new CombineInstance { mesh = insideMesh},
                new CombineInstance { mesh = sourceMesh}
            },
            true, false, false
        );
        Object.DestroyImmediate(insideMesh);

        AssetDatabase.CreateAsset(
            combinedMesh,
            System.IO.Path.Combine(
                "Assets", sourceMesh.name + " Double-Sided.asset"
            )
        );
    }
}
