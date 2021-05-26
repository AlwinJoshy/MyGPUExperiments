using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(MiniGamePirateGoodsLib.TradeableItem))]
class EditorItemSelectorProperty : PropertyDrawer
{
    float totalWidth;
    Vector2 tempVec2;
    Vector2Int tempVec2Int = new Vector2Int();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PrefixLabel(position, label);
        Rect contentPos = EditorGUI.PrefixLabel(position, label);

        totalWidth = contentPos.width;

        EditorGUIUtility.labelWidth = 35f;
        contentPos.width = totalWidth * 0.20f;
        EditorGUI.indentLevel = 0;
        property.FindPropertyRelative("itemID").intValue =
       EditorGUI.Popup(contentPos, property.FindPropertyRelative("itemID").intValue,
       (property.serializedObject.targetObject as MiniGamePirateGoodsLib).GetItemNames());

        EditorGUIUtility.labelWidth = 60f;
        tempVec2 = contentPos.position;
        contentPos.x += contentPos.width + 5f;
        contentPos.width = totalWidth * 0.60f;
        //tempVec2.x += contentPos.width + spacing;
        EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("reciveQuantity"), new GUIContent("inputRange"));
        //    property.FindPropertyRelative("reciveQuantity").vector2IntValue
        EditorGUI.EndProperty();
    }

}

