using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV_To_Json : CObjectBase {

	public enum EProp
	{
		갈색나무통, 남색도자기, 노란나무통, 노란열쇠상자, 보석상자, 빨간상자, 주황도자기,
	}

	[System.Serializable]
	public class SDataProp : IDictionaryItem<EProp>, IRandomItem
	{
		public string str물건이름;
		public int i등장확률;

		public int i최소드랍골드;
		public int i최대드랍골드;

		public EProp eProp { get { return str물건이름.ConvertEnum<EProp>(); } }

		public EProp IDictionaryItem_GetKey()
		{
			return eProp;
		}

		public int IRandomItem_GetPercent()
		{
			return i등장확률;
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		Dictionary<EProp, SDataProp> _mapData_Prop = new Dictionary<EProp, SDataProp>();
		SCManagerParserJson pParser = SCManagerParserJson.DoMakeInstance( this, SCManagerParserJson.const_strFolderName, EResourcePath.Resources );
		pParser.DoReadJson_And_InitEnumerator( "인게임오브젝트", ref _mapData_Prop );

		var listTest = _mapData_Prop.ToList();
		for (int i = 0; i < listTest.Count; i++)
			Debug.Log( string.Format( "Key : {0} Value ( i등장확률 : {1} i최대드랍골드 : {2} )", listTest[i].Key, listTest[i].Value.i등장확률, listTest[i].Value.i최대드랍골드 ) );
	}
}
