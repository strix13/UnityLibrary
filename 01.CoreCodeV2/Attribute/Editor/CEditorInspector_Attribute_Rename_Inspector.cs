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
    public override void OnGUI(Rect position,
                   SerializedProperty property, GUIContent label)
    {
        Rename_InspectorAttribute pAttributeTarget = (Rename_InspectorAttribute)attribute;
        label.text = pAttributeTarget.strInspectorName;

        GUI.enabled = pAttributeTarget.bIsEditPossibleInspector;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
