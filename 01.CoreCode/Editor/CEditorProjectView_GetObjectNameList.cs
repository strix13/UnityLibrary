using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CEditorProjectView_GetObjectNameList : Editor
{
	[MenuItem("Assets/StrixTool/Get Object Name List Like Enum", false, 0)]
	[MenuItem( "GameObject/StrixTool/Get Object Name List Like Enum", false, 15 )]
	static public void DoGetObjectNameList()
	{
		if (Selection.objects == null)
			return;
		else
		{
			StringBuilder pStrBuilder = new StringBuilder();
			List<UnityEngine.Object> listObject = new List<UnityEngine.Object>();
			listObject.AddRange(Selection.objects);
			listObject.Sort(CompareObject_ByName);
			for (int i = 0; i < listObject.Count; i++)
			{
				pStrBuilder.Append(listObject[i].name);

				if(i != listObject.Count - 1)
					pStrBuilder.Append(",");
			}

			UnityEngine.Debug.Log(pStrBuilder.ToString());
		}
	}

	static public int CompareObject_ByName(UnityEngine.Object pObj1, UnityEngine.Object pObj2)
	{
		string strObjName1 = pObj1.name;
		string strObjName2 = pObj2.name;

		return(strObjName1.CompareTo(strObjName2));
	}
}
