using UnityEngine;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 열거자 관련 확장 메서드용 클래스
   Version	   :
   ============================================ */

public interface IDictionaryItem<TKeyType>
{
	TKeyType IDictionaryItem_GetKey();
}

public interface IDictionaryItem_ContainData<TKeyType, TDataType> : IDictionaryItem<TKeyType>
{
	TKeyType IDictionaryItem_ContainData_GetKey();
	void IDictionaryItem_ContainData_SetData( TDataType sData );
}

public interface IListItem_HasField<TFieldType>
{
	TFieldType IListItem_HasField_GetField();
}

public static class SCEnumeratorHelper
{
	public static List<TSource> ToList<TSource>( this IEnumerable<TSource> source )
	{
		List<TSource> listOut = new List<TSource>();
		IEnumerator<TSource> pIter = source.GetEnumerator();
		while (pIter.MoveNext())
		{
			listOut.Add( pIter.Current );
		}

		return listOut;
	}

	public static void ToList<TSource>( this IEnumerable<TSource> source, List<TSource> listOut )
	{
		listOut.Clear();
		IEnumerator<TSource> pIter = source.GetEnumerator();
		while (pIter.MoveNext())
		{
			listOut.Add( pIter.Current );
		}
	}

	public static Dictionary<TKey, TSource> ToDictionary<TKey, TSource>( this IEnumerable<TSource> source )
		where TSource : IDictionaryItem<TKey>
	{
		Dictionary<TKey, TSource> mapReturn = new Dictionary<TKey, TSource>();
		mapReturn.DoAddItem( source );

		return mapReturn;
	}


	static public void DoAddItem<TKey, TSource>( this Dictionary<TKey, TSource> mapDataTable, IEnumerable<TSource> source )
		where TSource : IDictionaryItem<TKey>
	{
		mapDataTable.Clear();
		IEnumerator<TSource> pIter = source.GetEnumerator();
		while (pIter.MoveNext())
		{
			TSource pCurrent = pIter.Current;
			TKey hDataID = pCurrent.IDictionaryItem_GetKey();
			if (mapDataTable.ContainsKey( hDataID ))
				Debug.LogWarning( "에러, 데이터 테이블에 공통된 키값을 가진 데이터가 존재!!" + typeof( TSource ) + hDataID );
			else
				mapDataTable.Add( hDataID, pCurrent );
		}
	}


	static public void DoAddItem<Struct, TKey>( this Dictionary<TKey, Struct> mapDataTable, Struct[] arrDataTable )
	where Struct : IDictionaryItem<TKey>
	{
		mapDataTable.Clear();
		if (arrDataTable == null) return;

		for (int i = 0; i < arrDataTable.Length; i++)
		{
			TKey hDataID = arrDataTable[i].IDictionaryItem_GetKey();
			if (mapDataTable.ContainsKey( hDataID ))
				Debug.LogWarning( "에러, 데이터 테이블에 공통된 키값을 가진 데이터가 존재!!" + typeof( Struct ) + hDataID );
			else
				mapDataTable.Add( hDataID, arrDataTable[i] );
		}
	}

	static public void DoAddItem<Struct, TKey>( this Dictionary<TKey, List<Struct>> mapDataTable, Struct[] arrDataTable )
	where Struct : IDictionaryItem<TKey>
	{
		mapDataTable.Clear();
		if (arrDataTable == null) return;

		for (int i = 0; i < arrDataTable.Length; i++)
		{
			TKey hDataID = arrDataTable[i].IDictionaryItem_GetKey();

			if (mapDataTable.ContainsKey( hDataID ) == false)
				mapDataTable.Add( hDataID, new List<Struct>() );

			mapDataTable[hDataID].Add( arrDataTable[i] );
		}
	}
	static public void DoLinkOwnerToData<TOwner, TKeyData, TData>( this Dictionary<TKeyData, TData> mapData, List<TOwner> listDataOwner )
		where TOwner : class, IDictionaryItem_ContainData<TKeyData, TData>
		where TData : IDictionaryItem<TKeyData>
	{
		for (int i = 0; i < listDataOwner.Count; i++)
		{
			TOwner pOwner = listDataOwner[i];
			TKeyData pKey = pOwner.IDictionaryItem_GetKey();
			if (mapData.ContainsKey( pKey ))
				pOwner.IDictionaryItem_ContainData_SetData( mapData[pKey] );
			else
				Debug.LogWarning( string.Format( "{0}이 {1}에 없습니다)", pKey.ToString(), mapData.ToString() ) );
		}
	}

	static public void DoExtractItem<T, TFieldType>( this List<T> list, List<TFieldType> listOut )
		where T : IListItem_HasField<TFieldType>
	{
		listOut.Clear();
		for (int i = 0; i < list.Count; i++)
			listOut.Add( list[i].IListItem_HasField_GetField() );
	}

	static public void DoExtractItemList<T, TFieldType>( this List<T> list, List<TFieldType> listOut )
		where T : IListItem_HasField<List<TFieldType>>
	{
		listOut.Clear();
		for (int i = 0; i < list.Count; i++)
			listOut.AddRange( list[i].IListItem_HasField_GetField() );
	}
}
