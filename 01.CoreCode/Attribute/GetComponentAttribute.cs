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
 *	
 *	기존에는 에디터 - Inspector에서 봐야만 갱신이 되었는데, 현재는 Awake에 두었다.
 *	오리지널 소스 링크
 *	https://openlevel.postype.com/post/683269
   ============================================ */
#endregion Header

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public abstract class GetComponentAttributeBase : PropertyAttribute
{
	public bool bIsPrint_OnNotFound;

    public abstract object GetComponent(CObjectBase behaviour, Type pElement);
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(CObjectBase behaviour, Type pElement)
    {
        MethodInfo getter = typeof(CObjectBase)
                 .GetMethod("GetComponents", new Type[0])
                 .MakeGenericMethod(pElement);
        return getter.Invoke(behaviour, null);
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentInChildrenAttribute : GetComponentAttributeBase
{
    public bool bSearch_By_ComponentName = false;
	public string strComponentName;

	public GetComponentInChildrenAttribute( System.Object pObject, bool bIsPrint_OnNotFound = true )
	{
		bSearch_By_ComponentName = true;
		this.strComponentName = pObject.ToString();
		this.bSearch_By_ComponentName = true;
		this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
	}

    public GetComponentInChildrenAttribute(bool bIsPrint_OnNotFound = true )
    {
        bSearch_By_ComponentName = false;
		this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
	}

    public override object GetComponent(CObjectBase behaviour, Type elementType)
    {
        MethodInfo getter = typeof(CObjectBase)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(elementType);
        return getter.Invoke(behaviour,
                new object[] { true });
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentInParentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(CObjectBase behaviour, Type elementType)
    {
        MethodInfo getter = typeof(CObjectBase)
          .GetMethod("GetComponentsInParent", new Type[] { typeof(bool) })
          .MakeGenericMethod(elementType);
        return getter.Invoke(behaviour,
                new object[] { });
    }
}