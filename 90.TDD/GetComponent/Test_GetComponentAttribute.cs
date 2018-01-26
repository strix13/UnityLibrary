using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class Test_GetComponentAttribute
{
	[UnityTest]
	public IEnumerator Test_GetComponentAttributeWithEnumeratorPasses()
	{
		GameObject pObjectParents = new GameObject( "Test" );
		Test_ComponentParents pParents = pObjectParents.AddComponent<Test_ComponentParents>();

		// GetComponent 대상인 자식 추가
		for(int i = 0; i < (int)Test_ComponentParents.ETestChildObject.MAX; i++)
		{
			GameObject pObjectchild = new GameObject( ((Test_ComponentParents.ETestChildObject)i).ToString());
			pObjectchild.AddComponent<Test_ComponentChild>();
			pObjectchild.transform.SetParent( pObjectParents.transform );
		}

		pParents.EventOnAwake();

		yield return null;

		Assert.IsTrue( pParents.p_mapTest_KeyIsEnum.Count == 3 );
		Assert.IsTrue( pParents.p_mapTest_KeyIsString.Count == 3 );
		Assert.NotNull( pParents.p_pChildComponent_FindEnum );
		Assert.NotNull( pParents.p_pChildComponent_FindString );

		Assert.IsTrue( pParents.p_pChildComponent_FindString.name == Test_ComponentParents.ETestChildObject.TestObject_Other_FindString.ToString() );
		Assert.IsTrue( pParents.p_pChildComponent_FindEnum.name == Test_ComponentParents.ETestChildObject.TestObject_Other_FindEnum.ToString() );

		List<KeyValuePair<string, Test_ComponentChild>> listString = pParents.p_mapTest_KeyIsString.ToList();
		for(int i = 0; i < listString.Count; i++)
			Assert.IsTrue(listString[i].Key == listString[i].Value.name.ToString()) ;

		List<KeyValuePair<Test_ComponentParents.ETestChildObject, Test_ComponentChild>> listEnum = pParents.p_mapTest_KeyIsEnum.ToList();
		for (int i = 0; i < listString.Count; i++)
			Assert.IsTrue( listString[i].Key.ToString() == listString[i].Value.name.ToString() );
	}
}
