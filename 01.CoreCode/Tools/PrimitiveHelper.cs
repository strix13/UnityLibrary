using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-27 오전 8:00:42
   Description : 
   Edit Log    : 
   ============================================ */

// 성능 측정
//System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
//stopWatch.Start();
//      for (int i = 0; i< 1000000; i++)
//         transform.position = Vector3.zero;

//      stopWatch.Stop();
//      Debug.Log("transform - Normal" + stopWatch.Elapsed);

//      stopWatch = new System.Diagnostics.Stopwatch();
//      stopWatch.Start();

//      for (int i = 0; i< 1000000; i++)
//         p_pTransCached.position = Vector3.zero;

//      stopWatch.Stop();
//      Debug.Log("transform - Cached" + stopWatch.Elapsed);

public static class PrimitiveHelper
{
	static public Vector3 Inverse( this Vector3 vecTarget )
	{
		return vecTarget * -1;
	}

	static public Vector3 RandomRange( Vector3 vecMinRange, Vector3 vecMaxRange )
	{
		float fRandX = Random.Range( vecMinRange.x, vecMaxRange.x );
		float fRandY = Random.Range( vecMinRange.y, vecMaxRange.y );
		float fRandZ = Random.Range( vecMinRange.z, vecMaxRange.z );

		return new Vector3( fRandX, fRandY, fRandZ );
	}

	static public Vector3 AddFloat( this Vector3 vecOrigin, float fAddValue )
	{
		vecOrigin.x += fAddValue;
		vecOrigin.y += fAddValue;
		vecOrigin.z += fAddValue;

		return vecOrigin;
	}

	static public string CutString( this string strText, char chCut, out string strCut )
	{
		strCut = null;
		int iLength = strText.Length;
		for (int i = 0; i < iLength; i++)
		{
			if (strText[i].Equals( chCut ))
			{
				strCut = strText.Substring( 0, i );
				strText = strText.Substring( i + 1 );
				break;
			}
		}

		return strText;
	}

	static public string CutString( this string strText, char chCut, out int iValue )
	{
		iValue = -1;

		string strTemp;
		strText = strText.CutString( chCut, out strTemp );

		if (strTemp != null)
			int.TryParse( strTemp, out iValue );

		return strText;
	}

	public static string CommaString( this string strText )
	{
		if (strText.Equals( "0" )) return "0";

		int iResult = 0;
		bool bSuccess = int.TryParse( strText, out iResult );
		if (bSuccess == false)
		{
			Debug.LogWarning( "int 파싱 에러 : " + strText );

			return "Int Parse Error!";
		}

		return string.Format( "{0:#,###}", iResult );
	}


	static public ENUM[] DoGetEnumArray<ENUM>( string strEnumName, int iIndexStart, int iIndexEnd )
	{
		int iLoopIndex = iIndexEnd - iIndexStart;
		if (iIndexStart == 0)
			iLoopIndex += 1;

		ENUM[] arrEnumArray = new ENUM[iLoopIndex];

		for (int i = 0; i < iLoopIndex; i++)
		{
			try
			{
				arrEnumArray[i] = (ENUM)System.Enum.Parse( typeof( ENUM ), string.Format( "{0}{1}", strEnumName, i ) );
			}
			catch
			{
				Debug.LogWarning( typeof( ENUM ).ToString() + " 에 " + string.Format( "{0}{1}", strEnumName, i ) + "이 존재하지 않습니다." );
				break;
			}
		}

		return arrEnumArray;
	}

	static public T[] DoGetEnumType<T>()
		where T : System.IConvertible, System.IComparable
	{
		if (typeof( T ).IsEnum == false)
			throw new System.ArgumentException( "GetValues<T> can only be called for types derived from System.Enum", "T" );

		return (T[])System.Enum.GetValues( typeof( T ) );
	}

	static public void DoShuffleList<Compo>( this List<Compo> list )
	{
		if (list == null)
		{
			Debug.LogWarning( "Shuffle List에서 Shuffle할게 없다.." + typeof( Compo ).Name );
			return;
		}

		for (int i = 0; i < list.Count; i++)
		{
			int RandomIndex = Random.Range( i, list.Count );
			Compo temp = list[RandomIndex];
			list[RandomIndex] = list[i];
			list[i] = temp;
		}
	}

	static public void DoResetTransform( this Transform pTrans )
	{
		pTrans.localPosition = Vector3.zero;
		pTrans.localRotation = Quaternion.identity;
		pTrans.localScale = Vector3.one;
	}

	public static int GetEnumLength<TENUM>()
		where TENUM : System.IConvertible, System.IComparable
	{
		System.Array pArray = DoGetEnumType<TENUM>();

		return pArray.Length;
	}

	public static bool CheckIsValidString( string strTarget )
	{
		return strTarget != null && strTarget.Length != 0;
	}

	public static void UpdateInputLabel( this UIInput pInput, UIInput.InputType eInputType )
	{
		pInput.inputType = eInputType;
		pInput.UpdateLabel();
	}


	static Dictionary<System.Type, CDictionary_ForEnumKey<System.Enum, string>> g_mapEnumToString = new Dictionary<System.Type, CDictionary_ForEnumKey<System.Enum, string>>();
	public static string ToString_GarbageSafe( this System.Enum eEnum )
	{
		System.Type pType = eEnum.GetType();
		if (g_mapEnumToString.ContainsKey( pType ) == false)
			g_mapEnumToString.Add( pType, new CDictionary_ForEnumKey<System.Enum, string>() );

		CDictionary_ForEnumKey<System.Enum, string> mapEnumToString = g_mapEnumToString[pType];
		if (mapEnumToString.ContainsKey( eEnum ) == false)
			mapEnumToString.Add( eEnum, System.Enum.GetName( pType, eEnum ) );

		return mapEnumToString[eEnum];
	}

	static public void Sort_ObjectSibilingIndex( this List<GameObject> listpObject )
	{
		listpObject.Sort( Comparer_Object );
	}

	static public void Sort_ObjectSibilingIndex<TComponent>( this List<TComponent> listpObject )
		where TComponent : UnityEngine.Component
	{
		listpObject.Sort( new CPrimitiveHelper().Comparer_Object );
	}

	static public int Comparer_Object( GameObject pObjectX, GameObject pObjectY )
	{
		int iSiblingIndexX = pObjectX.transform.GetSiblingIndex();
		int iSiblingIndexY = pObjectY.transform.GetSiblingIndex();

		if (iSiblingIndexX < iSiblingIndexY)
			return -1;
		else if (iSiblingIndexX > iSiblingIndexY)
			return 1;
		else
			return 0;
	}

	static public bool GetComponent<COMPONENT>( this UnityEngine.Component pTarget, COMPONENT pComponent )
	where COMPONENT : UnityEngine.Component
	{
		pComponent = pTarget.GetComponent<COMPONENT>();

		return pComponent != null;
	}
}

public struct CPrimitiveHelper
{
	public int Comparer_Object( Component pObjectX, Component pObjectY )
	{
		int iSiblingIndexX = pObjectX.transform.GetSiblingIndex();
		int iSiblingIndexY = pObjectY.transform.GetSiblingIndex();

		if (iSiblingIndexX < iSiblingIndexY)
			return -1;
		else if (iSiblingIndexX > iSiblingIndexY)
			return 1;
		else
			return 0;
	}
}
