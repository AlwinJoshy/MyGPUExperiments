using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MiniGameNotorietyManager))]
public class EditorNotorietyManager : Editor
{
    bool editLocations;
    int lastSelectedSpawnPointID = 0;
    public static MiniGameNotorietyManager script;

    public override void OnInspectorGUI()
    {

        script = target as MiniGameNotorietyManager;
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayoutOption[] buttonStyle = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.20f) };

        GUILayout.BeginHorizontal();

        GUILayout.Label("Spawn Pts", buttonStyle);

        if (GUILayout.Button(editLocations ? "Hide" : "Edit Positions", buttonStyle))
        {
            editLocations = !editLocations;
            GUIUtility.hotControl = GUIUtility.hotControl > 0 ? 0 : 1;
            SceneView.RepaintAll();
        }
        else if (GUILayout.Button("Add Point", buttonStyle))
        {
            script.AddNewLocationPoint();
            lastSelectedSpawnPointID = script.spawnLocations.Length - 1;
            SceneView.RepaintAll();
        }

        else if (GUILayout.Button("Remove Point", buttonStyle))
        {
            script.RemoveSelectedLocationPoint(lastSelectedSpawnPointID);
            if (script.spawnLocations != null) lastSelectedSpawnPointID = script.spawnLocations.Length - 1;
            SceneView.RepaintAll();
        }

        GUILayout.EndHorizontal();

        //  DrawDefaultInspector();
    }

    void OnSceneGUI()
    {

        script = target as MiniGameNotorietyManager;

        Event guiEvent = Event.current;

        Draw();
        /*
                if (guiEvent.type == EventType.Layout)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                }
                */
    }

    void Draw()
    {
        Vector3 deltaPos;
        if (editLocations)
        {
            for (int i = 0; i < script.spawnLocations.Length; i++)
            {
                if (i == lastSelectedSpawnPointID)
                {
                    Handles.color = Color.red;
                }
                else
                {
                    Handles.color = Color.white;
                }
                deltaPos = Handles.FreeMoveHandle(script.spawnLocations[i], Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereHandleCap);

                if (GUI.changed && Vector3.SqrMagnitude(deltaPos - script.spawnLocations[i]) > 0.000001f)
                {
                    lastSelectedSpawnPointID = i;
                    Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    float downScale = mouseRay.origin.y - script.transform.position.y;
                    float downProj = Vector3.Project(mouseRay.direction, Vector3.down).magnitude;
                    script.spawnLocations[i] = mouseRay.origin + (mouseRay.direction * (downScale / downProj));
                }
                //script.spawnLocations[i].y = script.transform.position.y;
            }
        }
    }

}

[CustomPropertyDrawer(typeof(MiniGameNotorietyManager.NotorietyLevelControl))]
public class NotorietyLevelControlDrawer : PropertyDrawer
{

    Vector2 tempVec2;
    const float spacing = 0.1f;
    float totalWidth;
    MiniGameNotorietyManager script;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

       // script = property.objectReferenceValue.name;

       // Debug.Log(property.serializedObject.FindProperty("attackLevelTires").arrayElementType);

        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PrefixLabel(position, label);
        Rect contentPos = EditorGUI.PrefixLabel(position, label);

        totalWidth = contentPos.width;

        EditorGUIUtility.labelWidth = 35f;
        contentPos.width = totalWidth * 0.35f;
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("stageName"), new GUIContent("name"));

        EditorGUIUtility.labelWidth = 30f;
        tempVec2 = contentPos.position;
        contentPos.x += contentPos.width + 2f;
        contentPos.width = totalWidth * 0.20f;
        //tempVec2.x += contentPos.width + spacing;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("maxValue"), new GUIContent("max"));

        EditorGUIUtility.labelWidth = 20f;
        tempVec2 = contentPos.position;
        contentPos.x += contentPos.width + 2f;
        contentPos.width = totalWidth * 0.20f;
        //tempVec2.x += contentPos.width + spacing;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("offecncePower"), new GUIContent("hit"));

        EditorGUIUtility.labelWidth = 20f;
        tempVec2 = contentPos.position;
        contentPos.x += contentPos.width + 2f;
        contentPos.width = totalWidth * 0.20f;
        //tempVec2.x += contentPos.width + spacing;
        //EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("offeceCatagory"), new GUIContent("tire"));

  //      MiniGameNotorietyManager.MalitiaTire extractedMilitiaArray = property.serializedObject.FindProperty("attackLevelTires") as MiniGameNotorietyManager.MalitiaTire;

        property.FindPropertyRelative("offeceCatagory").intValue = 
        EditorGUI.Popup(contentPos, property.FindPropertyRelative("offeceCatagory").intValue, 
        CustomEditorUtilityClass.GetNameFromArray(property.serializedObject.FindProperty("attackLevelTires"), "tireName"));


        EditorGUI.EndProperty();
    }
}

public static class CustomEditorUtilityClass
{
    static dynamic dynObject;

    static public string[] GetNameList<T>(T[] arrayObj)
    {
        string[] nameArray = new string[arrayObj.Length];

        //  MiniGameNotorietyManager.MatitiaTire temp = arrayObj[0] as MiniGameNotorietyManager.MatitiaTire;
        for (int i = 0; i < arrayObj.Length; i++)
        {
            dynamic obj = System.Convert.ChangeType(arrayObj[i], typeof(T));
            nameArray[i] = obj.name;
        }
        return nameArray;
    }

    static public string[] GetNameFromArray(SerializedProperty arrayObj, string namePropertyName)
    {
        string[] nameArray = new string[arrayObj.arraySize];
              
        for (int i = 0; i < arrayObj.arraySize; i++)
        {
            SerializedProperty obj = arrayObj.GetArrayElementAtIndex(i);
            nameArray[i] = obj.FindPropertyRelative(namePropertyName).stringValue;
        }
        return nameArray;
    }

}
