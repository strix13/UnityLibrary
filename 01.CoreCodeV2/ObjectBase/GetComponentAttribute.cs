#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	작성자 : Strix
 *	
 *	기능 : 
 *	오리지널 소스코드의 경우 에디터 - Inspector에서 봐야만 갱신이 되었는데,
 *	현재는 SCManagerGetComponent.DoUpdateGetComponentAttribute 를 호출하면 갱신하도록 변경하였습니다.
 *	Awake에서 호출하시면 됩니다.
 *	
 *	Private 변수는 갱신되지 않습니다.
 *	
 *	오리지널 소스 링크
 *	https://openlevel.postype.com/post/683269
   ============================================ */
#endregion Header

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEngine.TestTools;
using NUnit.Framework;
#endif

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public abstract class GetComponentAttributeBase : UnityEngine.PropertyAttribute
{
    public bool bIsPrint_OnNotFound;

    public abstract object GetComponent(MonoBehaviour pTargetMono, Type pElementType);
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
                 .GetMethod("GetComponents", new Type[0])
                 .MakeGenericMethod(pElementType);
        return getter.Invoke(pTargetMono, null);
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentInChildrenAttribute : GetComponentAttributeBase
{
    public bool bSearch_By_ComponentName = false;
    public string strComponentName;

    public GetComponentInChildrenAttribute(System.Object pObject, bool bIsPrint_OnNotFound = true)
    {
        bSearch_By_ComponentName = true;
        this.strComponentName = pObject.ToString();
        this.bSearch_By_ComponentName = true;
        this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
    }

    public GetComponentInChildrenAttribute(bool bIsPrint_OnNotFound = true)
    {
        bSearch_By_ComponentName = false;
        this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
    }

    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(pElementType);
        return getter.Invoke(pTargetMono,
                new object[] { true });
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GetComponentInParentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
          .GetMethod("GetComponentsInParent", new Type[] { typeof(bool) })
          .MakeGenericMethod(pElementType);
        return getter.Invoke(pTargetMono,
                new object[] { });
    }
}


static public class SCManagerGetComponent
{
    static public Component GetComponentInChildren(this Component pTarget, string strObjectName, System.Type pComponentType)
    {
        Component[] arrComponentFind = pTarget.transform.GetComponentsInChildren(pComponentType);
        for (int i = 0; i < arrComponentFind.Length; i++)
        {
            if (arrComponentFind[i].name.Equals(strObjectName))
                return arrComponentFind[i];
        }
        return null;
    }

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

    static private void ProcUpdateComponentAttribute(MonoBehaviour pTargetMono, FieldInfo[] arrFields)
    {
        for (int i = 0; i < arrFields.Length; i++)
        {
            FieldInfo pField = arrFields[i];
            object[] arrCustomAttributes = pField.GetCustomAttributes(true);

            for (int j = 0; j < arrCustomAttributes.Length; j++)
            {
                GetComponentAttributeBase pGetcomponentAttribute = arrCustomAttributes[j] as GetComponentAttributeBase;
                if (pGetcomponentAttribute == null) continue;

                System.Type pTypeField = pField.FieldType;
                object pComponent = null;

                if (pTypeField.IsArray)
                {
                    pComponent = pGetcomponentAttribute.GetComponent(pTargetMono, pTypeField.GetElementType());
                }
                else if (pTypeField.IsGenericType)
                {
                    pComponent = ProcUpdateComponent_Generic(pTargetMono, pField, (GetComponentInChildrenAttribute)pGetcomponentAttribute, pTypeField);
                }
                else
                {
                    if (pGetcomponentAttribute is GetComponentAttribute)
                        pComponent = pTargetMono.GetComponent(pTypeField);
                    else if (pGetcomponentAttribute is GetComponentInChildrenAttribute)
                    {
                        GetComponentInChildrenAttribute pAttributeInChildren = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
                        if (pAttributeInChildren.bSearch_By_ComponentName)
                            pComponent = pTargetMono.GetComponentInChildren(pAttributeInChildren.strComponentName, pTypeField);
                        else
                            pComponent = pTargetMono.GetComponentInChildren(pTypeField, true);
                    }
                    else if (pGetcomponentAttribute is GetComponentInParentAttribute)
                        pComponent = pTargetMono.GetComponentInParent(pTypeField);
                }

                if (pComponent == null && pGetcomponentAttribute.bIsPrint_OnNotFound)
                {
                    GetComponentInChildrenAttribute pAttribute = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
                    if (pAttribute != null && pAttribute.bSearch_By_ComponentName)
                        Debug.LogWarning(pTargetMono.name + string.Format(".{0}<{1}>({2}) Result == null", pGetcomponentAttribute.GetType().Name, pTypeField, pAttribute.strComponentName), pTargetMono);
                    else
                        Debug.LogWarning(pTargetMono.name + string.Format(".{0}<{1}> Result == null", pGetcomponentAttribute.GetType().Name, pTypeField), pTargetMono);
                    continue;
                }

                if (pTypeField.IsGenericType == false)
                    pField.SetValue(pTargetMono, pComponent);
            }
        }
    }

    static private object ProcUpdateComponent_Generic(MonoBehaviour pTargetMono, FieldInfo pField, GetComponentInChildrenAttribute pGetComponentAttribute, System.Type pTypeField)
    {
        object pComponent = null;
        System.Type pTypeField_Generic = pTypeField.GetGenericTypeDefinition();
        Type[] arrArgumentsType = pTypeField.GetGenericArguments();

        if (pTypeField_Generic == typeof(List<>))
        {
            // List의 경우 GetComponentsInChildren 을 통해 Array를 얻은 뒤
            pComponent = pGetComponentAttribute.GetComponent(pTargetMono, arrArgumentsType[0]);
            // List 생성자에 Array를 집어넣는다.
            pField.SetValue(pTargetMono, System.Activator.CreateInstance(pTypeField, pComponent));
        }
        else if (pTypeField_Generic == typeof(Dictionary<,>))
        {
            pComponent = ProcUpdateComponent_Dictionary(pTargetMono, pField, pTypeField, pGetComponentAttribute, arrArgumentsType[0], arrArgumentsType[1]);
        }

        return pComponent;
    }

    static private object ProcUpdateComponent_Dictionary(MonoBehaviour pTargetMono, FieldInfo pField, System.Type pTypeField, GetComponentInChildrenAttribute pAttributeInChildren, Type pType_DictionaryKey, Type pType_DictionaryValue)
    {
        object pComponent = pAttributeInChildren.GetComponent(pTargetMono, pType_DictionaryValue);
        Array arrComponent = pComponent as Array;

        if (arrComponent.Length == 0)
        {
            return null;
        }

        var Method_Add = pTypeField.GetMethod("Add", new[] {
                                pType_DictionaryKey, pType_DictionaryValue });

        // Reflection의 메소드는 Instance에서만 호출할수 있다.
        var pInstanceDictionary = System.Activator.CreateInstance(pTypeField);

        if (pType_DictionaryKey == typeof(string))
        {
            for (int k = 0; k < arrComponent.Length; k++)
            {
                Component pComponentChild = arrComponent.GetValue(k) as Component;
                Method_Add.Invoke(pInstanceDictionary, new object[] {
                                    pComponentChild.name,
                                    pComponentChild });
            }
        }
        else if (pType_DictionaryKey.IsEnum)
        {
            bool bIsDerived_DictionaryItem = false;
            Type[] arrInterfaces = pType_DictionaryValue.GetInterfaces();
            string strTypeName = typeof(IDictionaryItem<>).Name;
            for (int i = 0; i < arrInterfaces.Length; i++)
            {
                if (arrInterfaces[i].Name.Equals(strTypeName))
                {
                    bIsDerived_DictionaryItem = true;
                    break;
                }
            }
            if (bIsDerived_DictionaryItem)
            {
                var method = pType_DictionaryValue.GetMethod("IDictionaryItem_GetKey");
                for (int k = 0; k < arrComponent.Length; k++)
                {
                    Component pComponentChild = arrComponent.GetValue(k) as Component;
                    Method_Add.Invoke(pInstanceDictionary, new object[] {
                                    method.Invoke(pComponentChild, null),
                                    pComponentChild });
                }
            }
            else
            {
                for (int k = 0; k < arrComponent.Length; k++)
                {
                    Component pComponentChild = arrComponent.GetValue(k) as Component;
                    Method_Add.Invoke(pInstanceDictionary, new object[] {
                                    System.Enum.Parse(pType_DictionaryKey, pComponentChild.name),
                                    pComponentChild });
                }
            }
        }
        else
        {
            Debug.LogError("GetComponentAttribute Error Dictionary Key 타입은 string, enum입니다. Key Type : " + pType_DictionaryKey.Name);
            return null;
        }

        pField.SetValue(pTargetMono, pInstanceDictionary);
        return pComponent;
    }

    #region Test
#if UNITY_EDITOR

    public class Test_ComponentChild : MonoBehaviour { }
    public class Test_ComponentChild_DerivedDictionaryItem : MonoBehaviour, IDictionaryItem<Test_ComponentParents.ETestChildObject>
    {
        public Test_ComponentParents.ETestChildObject IDictionaryItem_GetKey()
        {
            return name.ConvertEnum<Test_ComponentParents.ETestChildObject>();
        }
    }

    public class Test_ComponentParents : MonoBehaviour
    {
        public enum ETestChildObject
        {
            TestObject_1,
            TestObject_2,
            TestObject_3,

            TestObject_Other_FindString,
            TestObject_Other_FindEnum,

            MAX,
        }


        [GetComponentInChildren]
        public List<Test_ComponentChild> p_listTest = new List<Test_ComponentChild>();

        [GetComponentInChildren]
        public Dictionary<string, Test_ComponentChild> p_mapTest_KeyIsString = new Dictionary<string, Test_ComponentChild>();
        [GetComponentInChildren]
        public Dictionary<ETestChildObject, Test_ComponentChild> p_mapTest_KeyIsEnum = new Dictionary<ETestChildObject, Test_ComponentChild>();

        [GetComponentInChildren("TestObject_Other_FindString")]
        public Test_ComponentChild p_pChildComponent_FindString;
        [GetComponentInChildren(ETestChildObject.TestObject_Other_FindEnum)]
        public Test_ComponentChild p_pChildComponent_FindEnum;

        public void Awake()
        {
            SCManagerGetComponent.DoUpdateGetComponentAttribute(this);
        }
    }

    [UnityTest]
    [Category("StrixLibrary")]

    static public IEnumerator Test_GetComponentAttribute()
    {
        GameObject pObjectParents = new GameObject("Test");

        // GetComponent 대상인 자식 추가
        for (int i = 0; i < (int)Test_ComponentParents.ETestChildObject.MAX; i++)
        {
            GameObject pObjectChild = new GameObject(((Test_ComponentParents.ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild>();
        }

        // 자식을 전부 추가한 뒤에 페런츠에 추가한다.
        // 추가하자마자 Awake로 자식을 찾기 때문
        Test_ComponentParents pParents = pObjectParents.AddComponent<Test_ComponentParents>();
        pParents.Awake();

        yield return null;

        // Getcomponent Attribute가 잘 작동했는지 체크 시작!!
        Assert.NotNull(pParents.p_pChildComponent_FindEnum);
        Assert.NotNull(pParents.p_pChildComponent_FindString);

        Assert.IsTrue(pParents.p_pChildComponent_FindString.name == Test_ComponentParents.ETestChildObject.TestObject_Other_FindString.ToString());
        Assert.IsTrue(pParents.p_pChildComponent_FindEnum.name == Test_ComponentParents.ETestChildObject.TestObject_Other_FindEnum.ToString());

        Assert.IsTrue(pParents.p_listTest.Count == (int)Test_ComponentParents.ETestChildObject.MAX);

        Assert.IsTrue(pParents.p_mapTest_KeyIsEnum.Count == 5);
        Assert.IsTrue(pParents.p_mapTest_KeyIsString.Count == 5);

        var pIterString = pParents.p_mapTest_KeyIsString.GetEnumerator();
        while (pIterString.MoveNext())
        {
            Assert.IsTrue(pIterString.Current.Key == pIterString.Current.Value.name.ToString());
        }

        var pIterEnum = pParents.p_mapTest_KeyIsEnum.GetEnumerator();
        while (pIterEnum.MoveNext())
        {
            Assert.IsTrue(pIterEnum.Current.Key.ToString() == pIterEnum.Current.Value.name.ToString());
        }
    }

    [UnityTest]
    [Category("StrixLibrary")]

    static public IEnumerator Test_GetComponentInChildren_DeriveEnum()
    {
        GameObject pObjectParents = new GameObject("Test");

        // GetComponent 대상인 자식 추가
        for (int i = 0; i < (int)Test_ComponentParents.ETestChildObject.MAX; i++)
        {
            GameObject pObjectChild = new GameObject(((Test_ComponentParents.ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild_DerivedDictionaryItem>();
        }

        // 자식을 전부 추가한 뒤에 페런츠에 추가한다.
        // 추가하자마자 Awake로 자식을 찾기 때문
        Test_ComponentParents pParents = pObjectParents.AddComponent<Test_ComponentParents>();
        pParents.Awake();

        yield return null;

        var pIterEnum = pParents.p_mapTest_KeyIsEnum.GetEnumerator();
        while (pIterEnum.MoveNext())
        {
            Assert.IsTrue(pIterEnum.Current.Key.ToString() == pIterEnum.Current.Value.name.ToString());
        }
    }

#endif
    #endregion
}