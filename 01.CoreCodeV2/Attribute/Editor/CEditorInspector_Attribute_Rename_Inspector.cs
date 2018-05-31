#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-28 오후 2:49:00
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(Rename_InspectorAttribute))]
public class CEditorInspector_Attribute_Rename : PropertyDrawer
{
    Rename_InspectorAttribute pAttributeTarget;

    public override void OnGUI(Rect position,
                   SerializedProperty property, GUIContent label)
    {
        pAttributeTarget = (Rename_InspectorAttribute)attribute;
        Vector2 vecSize = CalculateSize(pAttributeTarget);

        // Todo - 인스펙터 이름이 길어지면 재조정
        label.text = pAttributeTarget.strInspectorName;

        GUI.enabled = pAttributeTarget.bIsEditPossibleInspector;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        pAttributeTarget = (Rename_InspectorAttribute)attribute;
        Vector2 vecSize = CalculateSize(pAttributeTarget);

        return vecSize.y;
    }

    Vector2 CalculateSize(Rename_InspectorAttribute attribute)
    {
        return GUI.skin.label.CalcSize(new GUIContent(attribute.strInspectorName));
    }
}
