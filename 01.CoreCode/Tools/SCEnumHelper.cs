using System.Collections.Generic;
using UnityEngine;

public static class SCEnumHelper
{
	static Dictionary<System.Type, CDictionary_ForEnumKey<int, string>> g_mapEnumToString_ForGeneric = new Dictionary<System.Type, CDictionary_ForEnumKey<int, string>>();
	public static string ToString_GarbageSafe<ENUM>(ENUM eEnum)
		where ENUM : struct
	{
		int iHashCode = eEnum.GetHashCode();
		System.Type pType = eEnum.GetType();
		if (g_mapEnumToString_ForGeneric.ContainsKey(pType) == false)
			g_mapEnumToString_ForGeneric.Add(pType, new CDictionary_ForEnumKey<int, string>());

		CDictionary_ForEnumKey<int, string> mapEnumToString = g_mapEnumToString_ForGeneric[pType];
		if (mapEnumToString.ContainsKey(iHashCode) == false)
			mapEnumToString.Add(iHashCode, System.Enum.GetName(pType, eEnum));

		return mapEnumToString[iHashCode];
	}

	public static List<TEnum> ConvertEnumList<TEnum>(this List<int> listInt)
	where TEnum : System.IConvertible, System.IComparable
	{
		List<TEnum> listEnum = new List<TEnum>();
		for (int i = 0; i < listInt.Count; i++)
			listEnum.Add((TEnum)System.Enum.Parse(typeof(TEnum), listInt[i].ToString(), true));

		return listEnum;
	}

	static public TENUM ConvertEnum<TENUM>(this string strText, bool bIgnoreError = false)
	{
		TENUM eEnum = default(TENUM);
		try
		{
			eEnum = (TENUM)System.Enum.Parse(typeof(TENUM), strText);
		}
		catch
		{
			eEnum = default(TENUM);
			if(bIgnoreError == false)
				Debug.Log( typeof(TENUM).ToString() + " 에 " + strText + "이 존재하지 않습니다.", null);
		}

		return eEnum;
	}

	static public bool ConvertEnum<TENUM>(this string strText, out TENUM eEnum, UnityEngine.Object pObjectForDebug = null)
	{
		bool bSuccess = true;
		eEnum = default(TENUM);
		try
		{
			eEnum = (TENUM)System.Enum.Parse(typeof(TENUM), strText);
		}
		catch
		{
			bSuccess = false;
		}

		return bSuccess;
	}

	static public bool ConvertEnum_IgnoreError<TENUM>( this string strText, out TENUM eEnum)
	{
		bool bSuccess = true;
		eEnum = default( TENUM );
		try
		{
			eEnum = (TENUM)System.Enum.Parse( typeof( TENUM ), strText );
		}
		catch
		{
			bSuccess = false;
		}

		return bSuccess;
	}


	static public ENUM[] DoGetEnumArray<ENUM>(string strEnumName, int iIndexStart = 0, int iIndexEnd = 0)
	{
		int iLoopIndex = iIndexEnd - iIndexStart;
		if (iIndexStart == 0)
			iLoopIndex += 1;

		ENUM[] arrEnumArray = new ENUM[iLoopIndex];

		for (int i = 0; i < iLoopIndex; i++)
		{
			try
			{
				arrEnumArray[i] = (ENUM)System.Enum.Parse(typeof(ENUM), string.Format("{0}{1}", strEnumName, i));
			}
			catch
			{
				Debug.LogWarning(typeof(ENUM).ToString() + " 에 " + string.Format("{0}{1}", strEnumName, i) + "이 존재하지 않습니다.");
				break;
			}
		}

		return arrEnumArray;
	}

	static public T[] DoGetEnumType<T>()
	where T : System.IConvertible, System.IComparable
	{
		if (typeof(T).IsEnum == false)
			throw new System.ArgumentException("GetValues<T> can only be called for types derived from System.Enum", "T");

		return (T[])System.Enum.GetValues(typeof(T));
	}

	public static int GetEnumLength<TENUM>()
		where TENUM : System.IConvertible, System.IComparable
	{
		System.Array pArray = DoGetEnumType<TENUM>();

		return pArray.Length;
	}

	static Dictionary<System.Enum, List<System.Enum>> g_mapRandomTable = new Dictionary<System.Enum, List<System.Enum>>();
	public static void AddEnumGroup(this System.Enum eEnum, System.Enum eEnumGroupIn)
	{
		if (g_mapRandomTable.ContainsKey(eEnum) == false)
			g_mapRandomTable.Add(eEnum, new List<System.Enum>());

		g_mapRandomTable[eEnum].Add(eEnumGroupIn);
	}

	static Dictionary<System.Enum, Dictionary<System.Enum, List<System.Enum>>> g_mapDictionary = new Dictionary<System.Enum, Dictionary<System.Enum, List<System.Enum>>>();
	public static void AddEnumDictionary( this System.Enum eEnumTarget, System.Enum eEnumKey, System.Enum eEnumValue )
	{
		if (g_mapDictionary.ContainsKey( eEnumTarget) == false)
			g_mapDictionary[eEnumTarget] = new Dictionary<System.Enum, List<System.Enum>>();

		if (g_mapDictionary[eEnumTarget].ContainsKey( eEnumKey ) == false)
			g_mapDictionary[eEnumTarget].Add( eEnumKey, new List<System.Enum>() );

		g_mapDictionary[eEnumTarget][eEnumKey].Add( eEnumValue );
	}

	public static System.Enum GetEnumRandomValue( this System.Enum eEnumTarget, System.Enum eEnumKey )
	{
		System.Enum eEnumReturn = default(System.Enum);
		bool bSuccess = g_mapDictionary.ContainsKey( eEnumTarget );

		if (bSuccess)
		{
			List<System.Enum> listReturn;
			bSuccess = g_mapDictionary[eEnumTarget].TryGetValue( eEnumKey, out listReturn );

			int iIndex = Random.Range( 0, listReturn.Count );
			eEnumReturn = listReturn[iIndex];
		}

		if (bSuccess == false)
			Debug.LogWarning( "GetEnumValue Error - " + eEnumTarget + " 에" + eEnumKey + " 가 없다" );

		return eEnumReturn;
	}

	public static EnumItem GetRandomEnumInGroup<EnumItem>(this System.Enum eEnum)
		where EnumItem : struct, System.IConvertible, System.IComparable
	{
		bool bSuccess = g_mapRandomTable.ContainsKey(eEnum);
		EnumItem eEnumReturn = default(EnumItem);
		if (bSuccess)
		{
			List<System.Enum> listGroupEnum = g_mapRandomTable[eEnum];
			int iRandomIndex = Random.Range(0, listGroupEnum.Count);
			eEnumReturn = (EnumItem)System.Enum.Parse(typeof(EnumItem), listGroupEnum[iRandomIndex].ToString(), true);
		}
		else
			Debug.LogWarning("Fail Enum Get Random In Group Type : " + typeof(EnumItem) + "Enum : " + eEnum );

		return eEnumReturn;
	}

	public static bool GetRandomEnumInGroup(this System.Enum eEnum, out System.Enum eEnumRandom)
	{
		bool bSuccess = g_mapRandomTable.ContainsKey(eEnum);
		if (bSuccess)
		{
			List<System.Enum> listGroupEnum = g_mapRandomTable[eEnum];
			int iRandomIndex = Random.Range(0, listGroupEnum.Count);
			eEnumRandom = listGroupEnum[iRandomIndex];
		}
		else
			eEnumRandom = default(System.Enum);

		return bSuccess;
	}
}
