using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-18 오후 5:30:33
// Description : 
// Edit Log    : 주의) 파싱할 struct와 sql 데이터 테이블의 이름이 같아야 합니다.
// ============================================ 

public enum EPHPName
{
    Get,
    Get_OrInsert,
    Get_Range,
    Get_RandomKey,

    Update_Set,
    Update_Add,
    Update_Set_ServerTime,

    Check_Value,
    Check_Count,
    Check_Time,
    CheckCount_AndUpdateAdd,
    CheckValue_AndUpdateSet,

	DeleteInfo,

    Insert,
}

[System.Serializable]
public class SDataGame
{
	public string strKey;
	public string strValue;

	static private Dictionary<string, string> _mapDataConfig_String = new Dictionary<string, string>();
	static private Dictionary<string, int> _mapDataConfig_Int = new Dictionary<string, int>();
	static private Dictionary<string, float> _mapDataConfig_Float = new Dictionary<string, float>();

	static public void SetData( SDataGame[] arrData )
	{
		for (int i = 0; i < arrData.Length; i++)
			_mapDataConfig_String.Add( arrData[i].strKey, arrData[i].strValue );
	}

	static public int GetInt<Enum_FieldName>( Enum_FieldName eFieldName )
	{
		string strFieldName = eFieldName.ToString();
		if (_mapDataConfig_Int.ContainsKey( strFieldName ))
			return _mapDataConfig_Int[strFieldName];
		else
		{
			if (_mapDataConfig_String.ContainsKey( strFieldName ))
			{
				int iValue = int.Parse( _mapDataConfig_String[strFieldName] );
				_mapDataConfig_Int.Add( strFieldName, iValue );
				return iValue;
			}
			else
			{
				Debug.LogWarning( "SGameConfig에 " + strFieldName + " 이 존재하지 않습니다." );
				return -1;
			}
		}
	}

	static public float GetFloat<Enum_FieldName>( Enum_FieldName eFieldName )
	{
		string strFieldName = eFieldName.ToString();
		if (_mapDataConfig_Float.ContainsKey( strFieldName ))
			return _mapDataConfig_Float[strFieldName];
		else
		{
			if (_mapDataConfig_String.ContainsKey( strFieldName ))
			{
				float fValue = float.Parse( _mapDataConfig_String[strFieldName] );
				_mapDataConfig_Float.Add( strFieldName, fValue );
				return fValue;
			}
			else
			{
				Debug.LogWarning( "SGameConfig에 " + strFieldName + " 이 존재하지 않습니다." );
				return -1;
			}
		}
	}
}

[System.Serializable]
abstract public class SDataBase<TKey> : IDictionaryItem<TKey>
{
	public string strKey;
	public string strValue;

	static private Dictionary<TKey, Dictionary<string, string>> _mapDataConfig_String = new Dictionary<TKey, Dictionary<string, string>>();
	static private Dictionary<TKey, Dictionary<string, int>> _mapDataConfig_Int = new Dictionary<TKey, Dictionary<string, int>>();
	static private Dictionary<TKey, Dictionary<string, float>> _mapDataConfig_Float = new Dictionary<TKey, Dictionary<string, float>>();

	static public void SetData( SDataBase<TKey>[] arrData )
	{
		for (int i = 0; i < arrData.Length; i++)
		{
			TKey tKey = arrData[i].IDictionaryItem_GetKey();
			if(_mapDataConfig_String.ContainsKey( tKey ) == false)
				_mapDataConfig_String.Add( tKey, new Dictionary<string, string>() );

			_mapDataConfig_String[tKey].Add( arrData[i].strKey, arrData[i].strValue );
		}
	}

	static public int GetInt<Enum_FieldName>( TKey tKey, Enum_FieldName eFieldName )
	{
		return GetValue( tKey, eFieldName.ToString(), _mapDataConfig_Int, ConvertStringToInt );
	}

	static public float GetFloat<Enum_FieldName>( TKey tKey, Enum_FieldName eFieldName )
	{
		return GetValue( tKey, eFieldName.ToString(), _mapDataConfig_Float, ConvertStringToFloat );
	}


	static public T GetValue<T>(TKey tKey, string strFieldName, Dictionary<TKey, Dictionary<string,T>> mapData, System.Func<string, T> OnConvert )
	{
		if(_mapDataConfig_String.ContainsKey(tKey) == false)
		{
			Debug.Log( tKey + strFieldName + "은 아직 DB에서 로딩을 안했습니다." );
			return default( T );
		}

		if (mapData.ContainsKey(tKey) && mapData[tKey].ContainsKey( strFieldName ))
			return mapData[tKey][strFieldName];
		else
		{
			if (mapData.ContainsKey( tKey ) == false)
				mapData.Add( tKey, new Dictionary<string, T>() );

			if (_mapDataConfig_String[tKey].ContainsKey( strFieldName ))
			{
				T tValue = OnConvert( _mapDataConfig_String[tKey][strFieldName] );
				mapData[tKey].Add( strFieldName, tValue );
				return tValue;
			}
			else
			{
				Debug.LogWarning( strFieldName + " 이 존재하지 않습니다." );
				return default(T);
			}
		}
	}

	static int ConvertStringToInt(string strValue)
	{
		return int.Parse( strValue );
	}

	static float ConvertStringToFloat( string strValue )
	{
		return float.Parse( strValue );
	}

	public abstract TKey IDictionaryItem_GetKey();
}

public class CManagerNetworkDB_Common : CManagerNetworkDB_Base<CManagerNetworkDB_Common>
{
    protected override void OnMakeSingleton()
    {
        base.OnMakeSingleton();

        _strURLPrefix = CDefineMap.const_strPHPPrefix;
        _strDBName = CDefineMap.const_strDBName_Common;
    }
}

public class CManagerNetworkDB_Project : CManagerNetworkDB_Base<CManagerNetworkDB_Project>
{
    protected override void OnMakeSingleton()
    {
        base.OnMakeSingleton();

        _strURLPrefix = CDefineMap.const_strPHPPrefix;
        _strDBName = CDefineMap.const_strDBName_Project;
    }
}

public class CManagerNetworkDB_Base<CLASS> : CSingletonBase_Not_UnityComponent<CLASS>
    where CLASS : CManagerNetworkDB_Base<CLASS>, new()
{
    // ===================================== //
    // public - Variable declaration         //
    // ===================================== //

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    protected string _strURLPrefix = null;            // Example : http://URL/{0}.php
    protected string _strDBName = null;               // Example : ProjectName

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //

    public IEnumerator CoExcutePHP<ENUM_PHP_NAME>(string hID, ENUM_PHP_NAME ePHPName, string strTableName, System.Action<bool> OnFinishLoad = null, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoExcutePHP(hID, ePHPName.ToString(), strTableName, OnFinishLoad, arrParameter);
    }

    public IEnumerator CoExcuteAndGetValue<ENUM_PHP_NAME>(string hID, ENUM_PHP_NAME ePHPName, string strTableName, System.Action<bool, string> OnFinishLoad = null, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoExcuteAndGetValue(hID, ePHPName.ToString(), strTableName, OnFinishLoad, arrParameter);
    }

    public IEnumerator CoLoadDataFromServer_Json<ENUM_PHP_NAME, T>(string hID, ENUM_PHP_NAME ePHPName, System.Action<bool, T> OnFinishLoad, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoLoadDataFromServer<T>(hID, ePHPName.ToString(), OnFinishLoad, arrParameter);
    }

    public IEnumerator CoLoadDataFromServer_Json_Array<ENUM_PHP_NAME, T>(string hID, ENUM_PHP_NAME ePHPName, System.Action<bool, T[]> OnFinishLoad, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoLoadDataListFrom_Array<T>(hID, ePHPName.ToString(), OnFinishLoad, arrParameter);
    }

    // ===================================== //
    // public - [Getter And Setter] Function //
    // ===================================== //

    // ========================================================================== //

    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    protected IEnumerator CoExcutePHP(string hID, string strPHPName, string strTableName, System.Action<bool> OnFinishLoad = null, params StringPair[] arrParameter)
    {
        bool bSuccess = true;
        WWW www = GetWWW(hID, strPHPName, strTableName, arrParameter);
        yield return www;

        bSuccess = www.error == null && (www.text.Contains("false") == false);

        //Debug.Log(strPHPName + " result : " + www.text);

        if (bSuccess == false)
            Debug.LogWarning("DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTableName);

        if (OnFinishLoad != null)
            OnFinishLoad(bSuccess);

        yield break;
    }

    protected IEnumerator CoExcuteAndGetValue(string hID, string strPHPName, string strTableName, System.Action<bool, string> OnFinishLoad = null, params StringPair[] arrParameter)
    {
        bool bSuccess = true;
        WWW www = GetWWW(hID, strPHPName, strTableName, arrParameter);
        yield return www;

        bSuccess = www.error == null && (www.text.Contains("false") == false);

        //Debug.Log(strPHPName + " result : " + www.text);

        if (bSuccess == false)
            Debug.LogWarning("DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTableName);

        if (OnFinishLoad != null)
            OnFinishLoad(bSuccess, www.text);

        yield break;
    }

    protected IEnumerator CoLoadDataFromServer<T>(string hID, string strPHPName, System.Action<bool, T> OnFinishLoad, params StringPair[] arrParameter)
    {
        bool bSuccess = true;
        T pData = default(T);
        string strTypeName = typeof(T).ToString();
        WWW www = GetWWW(hID, strPHPName, strTypeName, arrParameter);
        yield return www;

        //Debug.Log(strPHPName + " result : " + www.text);

        List<T> listOutData = new List<T>();
        bSuccess = www.error == null;
        try
        {
            bSuccess = SCManagerParserJson.DoReadJsonArray<T>(www, ref listOutData);
            if (listOutData.Count > 0)
                pData = listOutData[0];
        }
        catch
        {
            bSuccess = false;
        }

        if (bSuccess == false)
            Debug.LogWarning("DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTypeName);

        if (OnFinishLoad != null)
            OnFinishLoad(bSuccess, pData);

        yield break;
    }

    public IEnumerator CoLoadDataListFrom_Array<T>(string hID, string strPHPName, System.Action<bool, T[]> OnFinishLoad, params StringPair[] arrParameter)
    {
        bool bSuccess = true;
        string strTypeName = typeof(T).ToString();
        WWW www = GetWWW(hID, strPHPName, strTypeName, arrParameter);
        yield return www;

        T[] arrOutData = null;
        bSuccess = www.error == null;
        try
        {
            bSuccess = SCManagerParserJson.DoReadJsonArray<T>(www, out arrOutData);
        }
        catch
        {
            bSuccess = false;
        }

        if (bSuccess == false)
            Debug.LogWarning("DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTypeName);

		if (arrOutData == null)
			arrOutData = new T[0];

		if (OnFinishLoad != null)
            OnFinishLoad(bSuccess, arrOutData);

        yield break;
    }



    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

    private WWW GetWWW(string hID, string strPHPName, string strTableName, params StringPair[] arrParameter)
    {
        WWWForm form = new WWWForm();
        if((hID == null || hID.Length == 0) == false)
			form.AddField("id", hID);

		form.AddField("dbname", _strDBName);
        form.AddField("table", strTableName);
        form.AddField("paramcount", arrParameter.Length);
        
        for (int i = 0; i < arrParameter.Length; i++)
        {
			if (arrParameter[i].strKey == null)
				Debug.LogWarning(string.Format("Error key {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName));
			else
				form.AddField("key" + i, arrParameter[i].strKey);

			if (arrParameter[i].strValue == null)
				Debug.LogWarning(string.Format("Error value {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName));
			else
				form.AddField("value" + i, arrParameter[i].strValue);
        }

        return new WWW(string.Format(_strURLPrefix, strPHPName), form);
    }
}
