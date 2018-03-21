#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	작성자 : Strix
 *	
 *	기능 : 
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

static public class SCManagerGetComponent
{
    static public void DoUpdateGetComponentAttribute(MonoBehaviour pTarget)
    {
        System.Type pType = pTarget.GetType();
        FieldInfo[] arrFields = pType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        ProcUpdateComponentAttribute(pTarget, arrFields);

        arrFields = pType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        ProcUpdateComponentAttribute(pTarget, arrFields);

        arrFields = pType.GetFields(BindingFlags.Public | BindingFlags.Static);
        ProcUpdateComponentAttribute(pTarget, arrFields);

        arrFields = pType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
        ProcUpdateComponentAttribute(pTarget, arrFields);
    }

    static private void ProcUpdateComponentAttribute(MonoBehaviour pTarget, FieldInfo[] arrFields)
    {
        for (int i = 0; i < arrFields.Length; i++)
        {
            FieldInfo pField = arrFields[i];
            object[] arrCustomAttributes = pField.GetCustomAttributes(true);

            for (int j = 0; j < arrCustomAttributes.Length; j++)
            {
                GetComponentAttributeBase pGetcomponentAttribute = arrCustomAttributes[j] as GetComponentAttributeBase;
                if (pGetcomponentAttribute == null) continue;

                bool bIsRequireSetValue = true;
                System.Type pTypeField = pField.FieldType;
                object pComponent = null;

                if (pTypeField.IsArray)
                {
                    pComponent = pGetcomponentAttribute.GetComponent(pTarget, pTypeField.GetElementType());
                }
                else if (pTypeField.IsGenericType)
                {
                    GetComponentInChildrenAttribute pAttributeInChildren = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
                    System.Type pTypeField_Generic = pTypeField.GetGenericTypeDefinition();
                    if (pTypeField_Generic == typeof(List<>))
                    {
                        bIsRequireSetValue = false;
                        pComponent = pAttributeInChildren.GetComponent_List(pTarget, pTypeField.GetElementType(), pTypeField.GetGenericArguments()[0]);

                        pField.SetValue(pTarget, System.Activator.CreateInstance(pTypeField, pComponent));
                    }
                    //else if (pTypeField_Generic == typeof(Dictionary<,>))
                    //{
                    //    bIsRequireSetValue = false;
                    //    pComponent = pAttributeInChildren.GetComponent_Dictionary(pTarget, pTypeField.GetElementType(), pTypeField.GetGenericArguments()[0]);

                    //    pField.SetValue(pTarget, System.Activator.CreateInstance(pTypeField, pComponent));
                    //}
                }
                else
                {
                    if (pGetcomponentAttribute is GetComponentAttribute)
                        pComponent = pTarget.GetComponent(pTypeField);
                    else if (pGetcomponentAttribute is GetComponentInChildrenAttribute)
                    {
                        GetComponentInChildrenAttribute pAttributeInChildren = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
                        if (pAttributeInChildren.bSearch_By_ComponentName)
                            pComponent = pTarget.GetComponentInChildren(pAttributeInChildren.strComponentName, pTypeField);
                        else
                            pComponent = pTarget.GetComponentInChildren(pTypeField, true);
                    }
                    else if (pGetcomponentAttribute is GetComponentInParentAttribute)
                        pComponent = pTarget.GetComponentInParent(pTypeField);
                }

                if (pComponent == null && pGetcomponentAttribute.bIsPrint_OnNotFound)
                {
                    GetComponentInChildrenAttribute pAttribute = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
                    if (pAttribute != null && pAttribute.bSearch_By_ComponentName)
                        Debug.LogWarning(pTarget.name + string.Format(".{0}<{1}>({2}) Result == null", pGetcomponentAttribute.GetType().Name, pTypeField, pAttribute.strComponentName), pTarget);
                    else
                        Debug.LogWarning(pTarget.name + string.Format(".{0}<{1}> Result == null", pGetcomponentAttribute.GetType().Name, pTypeField), pTarget);
                    continue;
                }

                if (bIsRequireSetValue)
                    pField.SetValue(pTarget, pComponent);
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public abstract class GetComponentAttributeBase : PropertyAttribute
{
	public bool bIsPrint_OnNotFound;

    public abstract object GetComponent(MonoBehaviour behaviour, Type pElement);
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour behaviour, Type pElement)
    {
        MethodInfo getter = typeof(MonoBehaviour)
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

    public override object GetComponent(MonoBehaviour behaviour, Type elementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(elementType);
        return getter.Invoke(behaviour,
                new object[] { true });
    }

    public object GetComponent_List(MonoBehaviour behaviour, Type pFieldType, Type pFieldGenericType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(pFieldGenericType);
        return getter.Invoke(behaviour,
                new object[] { true });
    }

    //public object GetComponent_Dictionary(MonoBehaviour behaviour, Type pElement1, Type pElement2)
    //{
    //    MethodInfo getter = typeof(MonoBehaviour)
    //        .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
    //        .MakeGenericMethod(pElement1);

    //    object arrComponent = getter.Invoke(behaviour, new object[] { true });

    //    return arrComponent;
    //}
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentInParentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour behaviour, Type elementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
          .GetMethod("GetComponentsInParent", new Type[] { typeof(bool) })
          .MakeGenericMethod(elementType);
        return getter.Invoke(behaviour,
                new object[] { });
    }
}