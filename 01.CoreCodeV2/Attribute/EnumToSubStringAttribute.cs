#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-15 오후 5:39:53
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class EnumToSubStringAttribute : UnityEngine.PropertyAttribute
{
    public string strSubString;

    public EnumToSubStringAttribute(string strSubString)
    {
        this.strSubString = strSubString;
    }
}

public static class SCEnumToSubStringHelper
{
    static public string ToStringSub(this System.Enum eEnum)
    {
        string strString = eEnum.ToString();
        Type pType = eEnum.GetType();
        FieldInfo pFieldInfo = pType.GetField(strString);
        EnumToSubStringAttribute[] arrAttribute = pFieldInfo.GetCustomAttributes(typeof(EnumToSubStringAttribute), false) as EnumToSubStringAttribute[];
        if (arrAttribute.Length > 0)
            strString = arrAttribute[0].strSubString;

        return strString;
    }
}

[Category("StrixLibrary")]
public class EnumToSubStringAttribute_Test
{
    public enum ETest
    {
        [EnumToSubString("Test11")]
        Test1,
        Test2,
    }

    [Test]
    public void 이넘_투_서브스트링_테스트()
    {
        Assert.AreEqual(ETest.Test1.ToStringSub(), "Test11");
        Assert.AreEqual(ETest.Test2.ToStringSub(), "Test2");
    }
}
