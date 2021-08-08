using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(VoxelDisplay))]
public class VoxelGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VoxelDisplay myTarget = (VoxelDisplay)target;

        myTarget.textureName = EditorGUILayout.TextField("Texture Name", myTarget.textureName);
        myTarget.saveLocation = EditorGUILayout.TextField("Save Location", myTarget.saveLocation);
        myTarget.pointContainer = EditorGUILayout.ObjectField("Point Collection", myTarget.pointContainer, typeof(Transform), true) as Transform;
        myTarget.texSize = EditorGUILayout.IntField("Texture Size : ", myTarget.texSize);
        myTarget.containerSize = EditorGUILayout.FloatField("Container Size : ", myTarget.containerSize);

        if(GUILayout.Button("Generate Texture"))
        {
            myTarget.CreateTexture();
        }
    }
}
