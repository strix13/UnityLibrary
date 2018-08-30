﻿#region Header
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
using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
#pragma warning disable CS0672 // 멤버가 사용되지 않는 멤버를 재정의합니다.

[OdinDrawer]
[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
public class CEditorInspector_Attribute_Rename : OdinAttributeDrawer<Rename_InspectorAttribute>
{
    /// <summary>
    /// Draws the attribute.
    /// </summary>
    protected override void DrawPropertyLayout(InspectorProperty property, Rename_InspectorAttribute attribute, GUIContent label)
    {
        GUI.enabled = attribute.bIsEditPossibleInspector;

        var context = property.Context.Get<StringMemberHelper>(this, "StringContext", (StringMemberHelper)null);
        if (context.Value == null)
            context.Value = new StringMemberHelper(property.ParentType, attribute.strInspectorName);

        if (context.Value.ErrorMessage != null)
            SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);

        if (label == null)
            property.Label = null;
        else
        {
            property.Label = label;
            property.Label.text = context.Value.GetString(property);
        }
        GUI.enabled = true;

        this.CallNextDrawer(property, property.Label);
    }
}

#else
[CustomPropertyDrawer(typeof(Rename_InspectorAttribute))]
public class CEditorInspector_Attribute_Rename : PropertyDrawer
{
    Rename_InspectorAttribute pAttributeTarget;

    public override void OnGUI(Rect position,
                   SerializedProperty property, GUIContent label)
    {
        pAttributeTarget = (Rename_InspectorAttribute)attribute;
        // Vector2 vecSize = CalculateSize(pAttributeTarget);

        // Todo - 인스펙터 이름이 길어지면 재조정
        // Todo - Array 등 복수형 자료에서도 적용해야 함
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
#endif

#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
#pragma warning restore CS0672 // 멤버가 사용되지 않는 멤버를 재정의합니다.
