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
	public static TSource[] ToArray<TSource>( this IEnumerable<TSource> source )
	{
		int iCapacity = 0;
		IEnumerator<TSource> pIter = source.GetEnumerator();
		while (pIter.MoveNext())
		{
			iCapacity++;
		}

		TSource[] arrReturn = new TSource[iCapacity];
		int iIndex = 0;
		pIter.Reset();
		while (pIter.MoveNext())
		{
			arrReturn[iIndex++] = pIter.Current;
		}

		return arrReturn;
	}


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


	static public void DoAddItem<TKey, TSource>( this Dictionary<TKey, TSource> mapDataTable, IEnumerable<TSource> source, bool bIsClear = true )
		where TSource : IDictionaryItem<TKey>
	{
		if(bIsClear)
			mapDataTable.Clear();

		IEnumerator<TSource> pIter = source.GetEnumerator();
		while (pIter.MoveNext())
		{
			TSource pCurrent = pIter.Current;
			TKey hDataID = pCurrent.IDictionaryItem_GetKey();
			if (mapDataTable.ContainsKey( hDataID ))
				Debug.LogWarning( "에러, 데이터 테이블에 공통된 키값을 가진 데이터가 존재!!" + typeof( TSource ) + " : " + hDataID );
			else
				mapDataTable.Add( hDataID, pCurrent );
		}
	}

	static public void Add<TKey, TSource>( this Dictionary<TKey, List<TSource>> mapDataTable, TSource pAddSource )
		where TSource : IDictionaryItem<TKey>
	{
		TKey hDataID = pAddSource.IDictionaryItem_GetKey();
		if(mapDataTable.ContainsKey( hDataID ) == false)
			mapDataTable.Add(hDataID, new List<TSource>());

		mapDataTable[hDataID].Add( pAddSource );
	}
	

	static public void DoAddItem<TSource, TKey>( this Dictionary<TKey, TSource> mapDataTable, TSource[] arrDataTable, UnityEngine.Object pObjectForDebug = null )
		where TSource : IDictionaryItem<TKey>
	{
		mapDataTable.Clear();
		if (arrDataTable == null) return;

		for (int i = 0; i < arrDataTable.Length; i++)
		{
			TKey hDataID = arrDataTable[i].IDictionaryItem_GetKey();
			if (mapDataTable.ContainsKey( hDataID ))
				Debug.LogWarning( "에러, 데이터 테이블에 공통된 키값을 가진 데이터가 존재!!" + typeof( TSource ) + " : " + hDataID, pObjectForDebug );
			else
				mapDataTable.Add( hDataID, arrDataTable[i] );
		}
	}

	static public void DoAddItem<TSource, TKey>( this Dictionary<TKey, List<TSource>> mapDataTable, IEnumerable<TSource> source, bool bIsClear = true )
		where TSource : IDictionaryItem<TKey>
	{
		if (bIsClear)
			mapDataTable.Clear();

		IEnumerator<TSource> pIter = source.GetEnumerator();
		while (pIter.MoveNext())
		{
			TSource pCurrent = pIter.Current;
			TKey hDataID = pCurrent.IDictionaryItem_GetKey();
			if (mapDataTable.ContainsKey( hDataID ) == false)
				mapDataTable.Add( hDataID, new List<TSource>() );
			
			mapDataTable[hDataID].Add( pCurrent );
		}
	}
	
	static public void DoAddItem<TSource, TKey>( this Dictionary<TKey, List<TSource>> mapDataTable, TSource[] arrDataTable )
		where TSource : IDictionaryItem<TKey>
	{
		mapDataTable.Clear();
		if (arrDataTable == null) return;

		for (int i = 0; i < arrDataTable.Length; i++)
		{
			TKey hDataID = arrDataTable[i].IDictionaryItem_GetKey();

			if (mapDataTable.ContainsKey( hDataID ) == false)
				mapDataTable.Add( hDataID, new List<TSource>() );

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

	static public bool Contains_PrintOnError<T>( this List<T> list, T CheckData, bool bContains = false )
	{
		bool bIsContain = list.Contains( CheckData );
		if (bIsContain == bContains)
			Debug.Log( "List.Contains 에 실패했습니다 - " + CheckData, null );

		return bIsContain;
	}

	static public bool TryGetValue<T>( this List<T> list, int iIndex, out T outData )
		where T : new()
	{
		bool bIsContain = iIndex < list.Count;
		if (bIsContain == false)
		{
			outData = new T();
			Debug.LogWarning( "List.TryGetValue 에 실패했습니다 - Index :  " + iIndex, null );
		}
		else
			outData = list[iIndex];

		return bIsContain;
	}

	static public bool ContainsKey_PrintOnError<TKey, TValue>( this Dictionary<TKey, TValue> map, TKey CheckKey, Object pObjectForDebug = null )
	{
		bool bIsContain = map.ContainsKey( CheckKey );
		if (bIsContain == false)
		{
			string strKeyName = typeof( TKey ).Name;
			string strValueName = typeof( TValue ).Name;
			Debug.LogWarning( string.Format( "Dictionary<{0}, {1}>.ContainsKey 에 실패했습니다 - {2}", strKeyName, strValueName, CheckKey ), pObjectForDebug );
		}

		return bIsContain;
	}

	static System.Text.StringBuilder pStringBuilder = new System.Text.StringBuilder();
	static public string ToString_DataList<T>( this List<T> list )
		where T : MonoBehaviour
	{
		pStringBuilder.Length = 0;
		for (int i = 0; i < list.Count; i++)
		{
			pStringBuilder.Append( list[i].name );
			if (i < list.Count - 1)
				pStringBuilder.Append( ", " );
		}

		return pStringBuilder.ToString();
	}

	static public T GetRandomItem<T>( this List<T> list )
	{
		if (list.Count == 0)
			return default( T );

		int iRandomIndex = Random.Range( 0, list.Count );
		return list[iRandomIndex];
	}

	static public T GetRandomItem<T>( this IEnumerable<T> source )
	{
		int iTotalCount = 0;
		IEnumerator<T> pIter = source.GetEnumerator();
		while (pIter.MoveNext())
		{
			iTotalCount++;
		}

		if (iTotalCount == 0)
			return default( T );

		int iRandomIndex = Random.Range( 0, iTotalCount ) + 1;
		pIter = source.GetEnumerator();
		T pDataReturn = default( T );

		while (pIter.MoveNext() && iRandomIndex-- > 0)
		{
			pDataReturn = pIter.Current;
		}

		return pDataReturn;
	}

	static public void AddRange_First<T>( this LinkedList<T> source, IEnumerable<T> arrItem )
	{
		IEnumerator<T> pEnumerator = arrItem.GetEnumerator();
		while (pEnumerator.MoveNext())
		{
			source.AddFirst( pEnumerator.Current );
		}
	}

	static public void AddRange_Last<T>( this LinkedList<T> source, IEnumerable<T> arrItem )
	{
		IEnumerator<T> pEnumerator = arrItem.GetEnumerator();
		while (pEnumerator.MoveNext())
		{
			source.AddLast( pEnumerator.Current );
		}
	}
}
