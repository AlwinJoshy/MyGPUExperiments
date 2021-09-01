using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PointCloudGenerator))]
public class PointGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PointCloudGenerator myTarget = (PointCloudGenerator)target;

        DrawUILine(Color.gray, 1, 10);
        EditorGUILayout.LabelField("Canvas Options", EditorStyles.boldLabel);

        myTarget.scanStarterPoint = EditorGUILayout.ObjectField("Point Collection", myTarget.scanStarterPoint, typeof(Transform), true) as Transform;
        myTarget.checkCount = EditorGUILayout.IntField("Check Count", myTarget.checkCount);
        myTarget.colorRes = EditorGUILayout.IntField("Color Res", myTarget.colorRes);
        myTarget.surfaceTexture = EditorGUILayout.ObjectField("Point Collection", myTarget.surfaceTexture, typeof(Texture2D), true) as Texture2D;
        myTarget.fileSaveLocation = EditorGUILayout.TextField("PCD Folder Path", myTarget.fileSaveLocation);
        myTarget.fileName = EditorGUILayout.TextField("PCD file name", myTarget.fileName);

        if (GUILayout.Button("Generate PCD"))
        {
            myTarget.GeneratePCD();
        }

        else if (GUILayout.Button("Generate PCD File"))
        {
            myTarget.GeneratePCDFile();
        }

        /*
         myTarget.textureName = EditorGUILayout.TextField("Texture Name", myTarget.textureName);
         myTarget.saveLocation = EditorGUILayout.TextField("Save Location", myTarget.saveLocation);
         myTarget.pointContainer = EditorGUILayout.ObjectField("Point Collection", myTarget.pointContainer, typeof(Transform), true) as Transform;
         myTarget.texSize = EditorGUILayout.IntField("Texture Size : ", myTarget.texSize);
         myTarget.containerSize = EditorGUILayout.FloatField("Container Size : ", myTarget.containerSize);

         if(GUILayout.Button("Generate Texture"))
         {
             myTarget.CreateTexture();
         }
         */
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
