using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MiniGameTestDynamicNameDropdown))]
public class NameDropdownEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();

        MiniGameTestDynamicNameDropdown script = (MiniGameTestDynamicNameDropdown)target;
        GUILayoutOption[] guiOptions = new GUILayoutOption[] { GUILayout.MaxWidth(100) };
        GUILayout.BeginHorizontal();
        script.SetID(EditorGUILayout.Popup(script.selectedID, script.AllObjectNames(script.allTypeSelection), guiOptions));
        script.SetID(EditorGUILayout.Popup(script.selectedID, script.AllObjectNames(script.allTypeSelection), guiOptions));
        //  EditorGUILayout.DropdownButton(objectNameList, script.selectedID, );
        GUILayout.EndHorizontal();
    }
}
