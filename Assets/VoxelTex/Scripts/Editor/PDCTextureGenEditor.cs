using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PCDTextureGenerator))]
public class PDCTextureGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PCDTextureGenerator myTarget = (PCDTextureGenerator)target;

        myTarget.textureRes = EditorGUILayout.IntField("Texture Res", myTarget.textureRes);
        myTarget.colorRes = EditorGUILayout.IntField("Color Res", myTarget.colorRes);
        myTarget.textureSaveLocation = EditorGUILayout.TextField("PCD file name", myTarget.textureSaveLocation);
        myTarget.textureName = EditorGUILayout.TextField("PCD file name", myTarget.textureName);
        myTarget.pcdFolderLocation = EditorGUILayout.TextField("PCD file name", myTarget.pcdFolderLocation);
        myTarget.pcdFileName = EditorGUILayout.TextField("PCD file name", myTarget.pcdFileName);

        if (GUILayout.Button("Generate PCD Texture"))
        {
            myTarget.LoadAndGeneratePCDTex();
        }

    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}
