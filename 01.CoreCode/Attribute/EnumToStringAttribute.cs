#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[AttributeUsage( AttributeTargets.Enum, Inherited = true, AllowMultiple = false )]
public class RegistEnumAttribute : PropertyAttribute
{
	public string strGroupName;

	public RegistEnumAttribute( string strGroupName = "None" ) { this.strGroupName = strGroupName; }
	public RegistEnumAttribute( System.Type pGroupType ) { this.strGroupName = pGroupType.Name; }
}

[AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = false )]
public class EnumToStringAttribute : PropertyAttribute
{
	public System.Type pTypeEnum;
	public int iSelectIndex_EnumValue;
	public int iSelectIndex_EnumType;

	public string strSelectEnumGroup = "None";
	public int iSelectIndex_EnumGroup;
}

[AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = false )]
public class Rename_InspectorAttribute : PropertyAttribute
{
	public string strInspectorName;
	public bool bIsEditPossibleInspector;

	/// <summary>
	/// 기본적으로 인스펙터에 노출시킵니다.
	/// </summary>
	/// <param name="strInpectorName">인스펙터에 노출시킬 이름</param>
	/// <param name="bIsEditPossibleInspector">에디터에서 수정가능 유무</param>
	public Rename_InspectorAttribute(string strInpectorName, bool bIsEditPossibleInspector = true)
	{
		this.strInspectorName = strInpectorName;
		this.bIsEditPossibleInspector = bIsEditPossibleInspector;
	}
}