using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EditorLocationLite))]
public class EditorLocationProperty : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PrefixLabel(position, label);
        Rect contentPos = EditorGUI.PrefixLabel(position, label);
        contentPos.width = 100;
        if (GUI.Button(contentPos, "Edit Position"))
        {
            Object targetObject = property.serializedObject.targetObject;
            (targetObject as SelectPositionForEdit).FindElementWithID(property.propertyPath);

            /*
            Object targetObject = property.serializedObject.targetObject;
            int genId = MiniGameGlobalRef.GenerateRandomID();
            property.FindPropertyRelative("objID").intValue = genId;
            property.serializedObject.ApplyModifiedProperties();
            (targetObject as SelectPositionForEdit).FindElementWithID(genId, 0);
            */
            // Object objProp = property.propertyPath;
            //     EditorLocationLite locLite = objProp as EditorLocationLite;
            // System.Type parentType = property.serializedObject.targetObject.GetType();
            // System.Reflection.FieldInfo objectInfo = parentType.GetField(property.propertyPath);

            // Debug.Log(parentType.GetField("conversations"));

        }
        // contentPos.x += 100;
        // contentPos.width = 100;
        // EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("objID"), GUIContent.none);
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(MiniGameConversation.TalkText))]
public class EditorTalkText : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //       base.OnGUI(position, property, label);
        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PrefixLabel(position, label);
        Rect contentPos = EditorGUI.PrefixLabel(position, GUIContent.none);
        float totalWidth = contentPos.width;
        contentPos.width = totalWidth * 0.20f;
        Object srcClassObj = property.serializedObject.targetObject;
        MiniGameConversator srcClass = srcClassObj as MiniGameConversator;
        property.FindPropertyRelative("entityID").intValue = EditorGUI.Popup(contentPos, property.FindPropertyRelative("entityID").intValue, srcClass.GetEntityNames(property.propertyPath));
        contentPos.x += contentPos.width;
        contentPos.width = totalWidth * 0.80f;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("talkText"), GUIContent.none);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18;
    }

}

[CustomEditor(typeof(MiniGameConversator))]
public class EditorManager : Editor
{
    MiniGameConversator script;

    public override void OnInspectorGUI()
    {
        script = target as MiniGameConversator;
        base.OnInspectorGUI();
        serializedObject.Update();
    }

    void OnSceneGUI()
    {
        script = target as MiniGameConversator;
        Event guiEvent = Event.current;
        Draw();
    }

    void Draw()
    {
        if (script.selectedConversationLocation != null) script.selectedConversationLocation.position = Handles.DoPositionHandle(script.selectedConversationLocation.position, Quaternion.identity);
    }
}

