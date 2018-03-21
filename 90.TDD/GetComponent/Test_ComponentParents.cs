using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ComponentParents : CObjectBase {

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

    // [GetComponentInChildren]
	//public Dictionary<string, Test_ComponentChild> p_mapTest_KeyIsString = new Dictionary<string, Test_ComponentChild>();
	//[GetComponentInChildren]
	//public Dictionary<ETestChildObject, Test_ComponentChild> p_mapTest_KeyIsEnum = new Dictionary<ETestChildObject, Test_ComponentChild>();

	[GetComponentInChildren( "TestObject_Other_FindString" )]
	public Test_ComponentChild p_pChildComponent_FindString;
	[GetComponentInChildren( ETestChildObject.TestObject_Other_FindEnum )]
	public Test_ComponentChild p_pChildComponent_FindEnum;
        
}
