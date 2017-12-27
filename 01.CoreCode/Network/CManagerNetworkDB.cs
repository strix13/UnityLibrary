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

	Update_Set_ID,
	Update_Set_ID_DoubleKey,
	Update_Set_Custom,

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

public class CManagerNetworkDB_Common : CManagerNetworkDB_Base<CManagerNetworkDB_Common>
{
}

public class CManagerNetworkDB_Project : CManagerNetworkDB_Base<CManagerNetworkDB_Project>
{
}

public class CManagerNetworkDB_Base<CLASS_Driven> : CSingletonBase_Not_UnityComponent<CLASS_Driven>
    where CLASS_Driven : CManagerNetworkDB_Base<CLASS_Driven>, new()
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

	public void DoSetNetworkAddress(string strURLPrefix , string strDBName)
	{
		_strURLPrefix = strURLPrefix;
		_strDBName = strDBName;
	}

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
        yield return CoLoadDataFromServer(hID, ePHPName.ToString(), OnFinishLoad, arrParameter);
    }

    public IEnumerator CoLoadDataFromServer_Json_Array<ENUM_PHP_NAME, T>(string hID, ENUM_PHP_NAME ePHPName, System.Action<bool, T[]> OnFinishLoad, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoLoadDataListFrom_Array(hID, ePHPName.ToString(), OnFinishLoad, arrParameter);
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

        // Debug.Log(strPHPName + " result : " + www.text);

        if (bSuccess == false)
			Debug.Log( "DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTableName);

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
		{
			Debug.Log( "DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTableName );
			for (int i = 0; i < arrParameter.Length; i++)
				Debug.Log( string.Format( "Key{0} : {1}, Value{2} : {3} ", i, arrParameter[i].strKey, i, arrParameter[i].strValue ));
		}

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
			Debug.Log( "DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTypeName);

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
			Debug.Log( "DBParser Warning " + www.text + " php : " + strPHPName + " TableName : " + strTypeName, null);

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
			{
				form.AddField( "key" + i, "" );
				//Debug.Log( string.Format( "Error key {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName ) );
			}
			else
				form.AddField("key" + i, arrParameter[i].strKey);

			if (arrParameter[i].strValue == null)
			{
				form.AddField( "value" + i, "" );
				//Debug.Log( string.Format( "Error value {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName ) );
			}
			else
				form.AddField("value" + i, arrParameter[i].strValue);
        }

        return new WWW(string.Format(_strURLPrefix, strPHPName), form);
    }
}
