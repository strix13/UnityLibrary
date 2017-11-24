using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 3:46:33
// Description : 
// Edit Log    : 
// ============================================ 

public enum ELogWriter
{
	Strix,
	KJH,
}

[System.Flags]
public enum EVolumeOff
{
	None = 0,
	SoundEffect = 1,
	BGM = 2,
	All,
}

[System.Serializable]
public class SINI_UserSetting
{
	public string strLanguage = "Korean";
	public float fVolumeEffect = 0.5f;
	public float fVolumeBGM = 0.5f;
	public EVolumeOff eVolumeOff = EVolumeOff.None;
	public bool bVibration;
	public string ID;
}

[System.Serializable]
public class SINI_DeveloperSetting
{
	public string strDeveloperName;
}

[System.Serializable]
public class SINI_ApplicationSetting
{
	public int iBundleCode;
	public string[] arrLogIgnore_Level;
	public string[] arrLogIgnore_Writer;
}

public interface IDB_Insert
{
	StringPair[] IDB_Insert_GetField();
}

public struct StringPair
{
	public string strKey;
	public string strValue;

	public StringPair( string strKey, string strValue )
	{
		this.strKey = strKey; this.strValue = strValue;
	}

	public StringPair( string strKey, object pValue )
	{
		this.strKey = strKey; this.strValue = pValue.ToString();
	}

	public StringPair( string strKey, System.Enum eEnum )
	{
		this.strKey = strKey; this.strValue = eEnum.GetHashCode().ToString();
	}
}

[RequireComponent( typeof( CCompoDontDestroyObj ) )]
public class CManagerFrameWorkBase<CLASS_Framework, ENUM_SCENE_NAME, ENUM_SOUND_NAME, CLASS_SOUNDPLAYER> : CSingletonBase<CLASS_Framework>
	where CLASS_Framework : CManagerFrameWorkBase<CLASS_Framework, ENUM_SCENE_NAME, ENUM_SOUND_NAME, CLASS_SOUNDPLAYER>
	where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
	where ENUM_SCENE_NAME : System.IFormattable, System.IConvertible, System.IComparable
	where CLASS_SOUNDPLAYER : CSoundPlayerBase<ENUM_SOUND_NAME>
{
	private const string const_strLocalPath_INI = "/INI";

	public enum EINI_JSON_FileName
	{
		Sound,
		UserSetting,
		DeveloperSetting,
		ApplicationSetting,
	}

	public enum ESceneLoadState
	{
		None,
		SceneLoadStart,
		SceneLoadFinish,
	}

	// ===================================== //
	// public - Variable declaration         //
	// ===================================== //

	static public CManagerNetworkDB_Project p_pNetworkDB { get { return CManagerNetworkDB_Project.instance; } }
	static public SCManagerSound<ENUM_SOUND_NAME> p_pManagerSound { get { return _pManagerSound; } }
	static public SCSceneLoader<ENUM_SCENE_NAME> p_pManagerScene { get { return _pManagerScene; } }
	static public SCManagerParserJson p_pManagerJsonINI { get { return _pJsonParser_Persistent; } }

	public static event System.Action<float> p_EVENT_OnLoadSceneProgress;
	public static event System.Action p_EVENT_OnStartLoadScene;
	public static event System.Action p_EVENT_OnFinishLoadScene;

	public static System.Action p_EVENT_OnLoadFinish_LocalData;

	private bool _bSuccessLoadScene; public bool p_bSuccessLoadScene { get { return _bSuccessLoadScene; } }

	// ===================================== //
	// protected - Variable declaration      //
	// ===================================== //

	static protected string _strCallBackRequest_SceneName;
	static protected System.Action _OnFinishLoad_Scene;

	protected SINI_UserSetting _sSetting_User; public SINI_UserSetting p_sUserSetting { get { return _sSetting_User; } }
	protected SINI_DeveloperSetting _sSetting_Developer; public SINI_DeveloperSetting p_sSetting_Developer { get { return _sSetting_Developer; } }
	protected SINI_ApplicationSetting _sSetting_App; public SINI_ApplicationSetting p_sSetting_App { get { return _sSetting_App; } }

	// ===================================== //
	// private - Variable declaration        //
	// ===================================== //

	static protected SCManagerSound<ENUM_SOUND_NAME> _pManagerSound;
	static protected SCSceneLoader<ENUM_SCENE_NAME> _pManagerScene;
	static protected SCManagerParserJson _pJsonParser_Persistent;
	static protected SCManagerParserJson _pJsonParser_StreammingAssets;

	static private List<iTween> _listTween = new List<iTween>();

	protected string _strID; public string p_strID { get { return _strID; } }

	static private List<AsyncOperation> _listAsyncLoadScene = new List<AsyncOperation>();
	static private ESceneLoadState _eSceneLoadState = ESceneLoadState.None;

	static private bool _bManualCall_EventOnFinishLoadScene;
	static private int _iMaxLoadScene;
	static private float _fStackProgress = 0f;

	static private int _iLocalDataLoadingCount_Request;
	static private int _iLocalDataLoadingCount_Finish;

	// ========================================================================== //

	// ===================================== //
	// public - [Do] Function                //
	// 외부 객체가 요청                      //
	// ===================================== //

	static public void DoNetworkDB_CheckCount_IsEqualOrGreater<StructDB>( string strFieldName, object iCheckFieldCount, System.Action<bool> OnResult )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Check_Count, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, iCheckFieldCount.ToString() ) ) );
	}

	static public void DoNetworkDB_UpdateAdd_If_CheckCount_IsEqualOrGreater<StructDB>( string strFieldName, object iCheckFieldCount, int iAddFieldCount, System.Action<bool, string> OnResult )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( instance._strID, EPHPName.CheckCount_AndUpdateAdd, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, iCheckFieldCount.ToString() ), new StringPair( strFieldName, iAddFieldCount.ToString() ) ) );
	}

	static public void DoNetworkDB_UpdateAdd<StructDB>( string strFieldName, object iFieldCount, System.Action<bool, string> OnResult, params StringPair[] arrParams )
	{
		CheckIsContainField<StructDB>( strFieldName );

		if (arrParams.Length != 0)
		{
			StringPair[] arrNewParams = new StringPair[arrParams.Length + 1];
			arrNewParams[0] = new StringPair( strFieldName, iFieldCount );

			for (int i = 0; i < arrParams.Length; i++)
				arrNewParams[i + 1] = arrParams[i];

			instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( instance._strID, EPHPName.Update_Add, typeof( StructDB ).ToString(), OnResult, arrNewParams ) );
		}
		else
			instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( instance._strID, EPHPName.Update_Add, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, iFieldCount.ToString() ) ) );
	}


	static public void DoNetworkDB_GetRange_Orderby_HighToLow<StructDB>( string strFieldName, int iGetDataCount, System.Action<bool, StructDB[]> OnFinishLoad )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json_Array( instance._strID, EPHPName.Get_Range, OnFinishLoad, new StringPair( strFieldName, iGetDataCount ) ) );
	}

	static public void DoNetworkDB_Get_Single<StructDB>( System.Action<bool, StructDB> OnFinishLoad, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json( instance._strID, EPHPName.Get, OnFinishLoad, arrParams ) );
	}

	static public void DoNetworkDB_GetOrInsert_Single<StructDB>( System.Action<bool, StructDB> OnFinishLoad, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json( instance._strID, EPHPName.Get_OrInsert, OnFinishLoad, arrParams ) );
	}

	static public void DoNetworkDB_GetRandomKey( string strCheckOverlapTableName, System.Action<bool, string> OnFinishLoad, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( null, EPHPName.Get_RandomKey, strCheckOverlapTableName, OnFinishLoad, arrParams ) );
	}

	static public void DoNetworkDB_Get_Array<StructDB>( System.Action<bool, StructDB[]> OnFinishLoad, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json_Array( instance._strID, EPHPName.Get, OnFinishLoad, arrParams ) );
	}

	/// <summary>
	/// DB에 Generic에 있는 필드 값을 덮어 씌운다.
	/// </summary>
	/// <typeparam name="StructDB"></typeparam>
	/// <param name="strFieldName">Generic에 있는 필드 명</param>
	/// <param name="strSetFieldValue">Generic에 있는 필드에 덮어씌울 값</param>
	/// <param name="OnResult">결과 함수</param>
	static public void DoNetworkDB_Update_Set<StructDB>( string strFieldName, object strSetFieldValue, System.Action<bool> OnResult )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Update_Set_ID, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, strSetFieldValue ) ) );
	}

	static public void DoNetworkDB_Update_Set_DoubleKey<StructDB>( string strFieldName, object strSetFieldValue, System.Action<bool> OnResult, StringPair pDoubleKey )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Update_Set_ID_DoubleKey, typeof( StructDB ).ToString(), OnResult,
			new StringPair[2] { pDoubleKey, new StringPair( strFieldName, strSetFieldValue ) } ) );
	}

	static public void DoNetworkDB_Update_Set_Custom<StructDB>( string strFieldName, object strSetFieldValue, System.Action<bool> OnResult, params StringPair[] arrField )
	{
		CheckIsContainField<StructDB>( strFieldName );

		StringPair[] arrPair = new StringPair[arrField.Length + 1];
		for (int i = 0; i < arrField.Length; i++)
			arrPair[i] = arrField[i];

		arrPair[arrField.Length] = new StringPair( strFieldName, strSetFieldValue.ToString() );

		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( null, EPHPName.Update_Set_Custom, typeof( StructDB ).ToString(), OnResult, arrPair ) );
	}

	static public void DoNetworkDB_Update_Set_Multi<StructDB>( System.Action<bool> OnResult, params StringPair[] arrParam )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Update_Set_ID, typeof( StructDB ).ToString(), OnResult, arrParam ) );
	}

	static public void DoNetworkDB_Update_Set_ServerTime<StructDB>( string strFieldName, System.Action<bool> OnResult )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Update_Set_ServerTime, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, "" ) ) );
	}

	static public void DoNetworkDB_Insert<StructDB>( System.Action<bool> OnResult, StructDB pStructDB )
		where StructDB : IDB_Insert
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Insert, typeof( StructDB ).ToString(), OnResult, pStructDB.IDB_Insert_GetField() ) );
	}

	static public void DoNetworkDB_Delete<StructDB>( System.Action<bool> OnResult, params StringPair[] arrParam )
	{
		if (instance._strID != null)
			instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.DeleteInfo, typeof( StructDB ).ToString(), OnResult, arrParam ) );
		else
			Debug.Log( "Delete는 strID에 null이오면 안됩니다." );
	}

	static public void DoNetworkDB_Insert<StructDB>( System.Action<bool> OnResult, params StringPair[] arrParam )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Insert, typeof( StructDB ).ToString(), OnResult, arrParam ) );
	}

	public void DoShakeMobile()
	{
		if (Application.isPlaying && _sSetting_User.bVibration)
			Handheld.Vibrate();
	}

	public void DoSetTestMode()
	{
		_strID = "Test";
	}

	// ===================================== //
	// public - [Event] Function             //
	// 프랜드 객체가 요청                    //
	// ===================================== //

	static public void DoSetTimeScale( float fTimeScale )
	{
		// ITween에서 흔들리는 모션을 TimeScale에 조종하기 위해..
		if (Time.timeScale != 0f && fTimeScale == 0f) // 플레이 중에 멈출때
		{
			iTween[] arrTween = FindObjectsOfType<iTween>();
			_listTween.Clear();
			for (int i = 0; i < arrTween.Length; i++)
			{
				_listTween.Add( arrTween[i] );
				arrTween[i].isRunning = false;
			}
		}
		else if (Time.timeScale != 0f) // 중지상태에서 플레이 할 때
		{
			for (int i = 0; i < _listTween.Count; i++)
				_listTween[i].isRunning = true;
		}

		Time.timeScale = fTimeScale;
	}

	static public void DoLoadScene( string strSceneName, LoadSceneMode eLoadSceneMode, System.Action OnFinishLoading = null )
	{
		SceneManager.LoadScene( strSceneName, eLoadSceneMode );
		if (_OnFinishLoad_Scene == null && OnFinishLoading != null)
		{
			_OnFinishLoad_Scene = OnFinishLoading;
			_strCallBackRequest_SceneName = strSceneName;
		}
	}

	static public void DoLoadScene( ENUM_SCENE_NAME eSceneName, LoadSceneMode eLoadSceneMode, System.Action OnFinishLoading = null )
	{
		SceneManager.LoadScene( eSceneName.ToString(), eLoadSceneMode );
		if (_OnFinishLoad_Scene == null && OnFinishLoading != null)
		{
			_OnFinishLoad_Scene = OnFinishLoading;
			_strCallBackRequest_SceneName = eSceneName.ToString();
		}
	}

	public void DoLoadSceneAsync_Manual( params ENUM_SCENE_NAME[] arrSceneName )
	{
		ProcStart_Loading( arrSceneName, true );
	}

	public void DoLoadSceneAsync_Manual( System.Action OnFinishLoadScene, params ENUM_SCENE_NAME[] arrSceneName )
	{
		p_EVENT_OnFinishLoadScene = OnFinishLoadScene;
		ProcStart_Loading( arrSceneName, true );
	}

	public void DoLoadSceneAsync( params ENUM_SCENE_NAME[] arrSceneName )
	{
		StartCoroutine(CoProcLoadSceneAsync(arrSceneName, false));
		//ProcStart_Loading( arrSceneName, false );
	}

	public void DoLoadSceneAsync( System.Action OnFinishLoadScene, params ENUM_SCENE_NAME[] arrSceneName )
	{
		p_EVENT_OnFinishLoadScene = OnFinishLoadScene;
		//ProcStart_Loading( arrSceneName, false );

		StartCoroutine(CoProcLoadSceneAsync(arrSceneName, false));
	}

	private IEnumerator CoProcLoadSceneAsync(ENUM_SCENE_NAME[] arrSceneName, bool bManualCall_EventOnFinishLoadScene)
	{
		yield return new WaitForSecondsRealtime(0.25f);

		// 로딩전 빈씬으로 메모리를 비워준다. 다음 로딩씬의 메모리가 너무 커서 프리징 걸릴수도있기때문에...
		AsyncOperation pAsyncOperation_Empty = SceneManager.LoadSceneAsync("Empty");
		yield return pAsyncOperation_Empty;

		System.GC.Collect();

		yield return new WaitForSecondsRealtime(0.25f);

		if (p_EVENT_OnStartLoadScene != null)
			p_EVENT_OnStartLoadScene();

		List<AsyncOperation> listAsyncLoadScene = new List<AsyncOperation>();

		int iMaxLoadScene = arrSceneName.Length;
		for (int i = 0; i < iMaxLoadScene; i++)
		{
			AsyncOperation pAsyncOperation = SceneManager.LoadSceneAsync(arrSceneName[i].ToString(), LoadSceneMode.Additive);
			pAsyncOperation.allowSceneActivation = false;

			listAsyncLoadScene.Add(pAsyncOperation);
		}

		float fStackProgress = 0f;
		while (fStackProgress < 0.9f * iMaxLoadScene)
		{
			float fTotalProgress = 0f;
			for (int i = 0; i < iMaxLoadScene; i++)
				fTotalProgress += listAsyncLoadScene[i].progress;

			if (fStackProgress < fTotalProgress)
				fStackProgress += Time.unscaledDeltaTime * iMaxLoadScene;

			if (p_EVENT_OnLoadSceneProgress != null)
				p_EVENT_OnLoadSceneProgress(fStackProgress / iMaxLoadScene);

			yield return null;
		}

		for (int i = 0; i < iMaxLoadScene; i++)
		{
			AsyncOperation pAsyncOperation = listAsyncLoadScene[i];
			pAsyncOperation.allowSceneActivation = true;

			fStackProgress += 0.1f;

			if (p_EVENT_OnLoadSceneProgress != null)
				p_EVENT_OnLoadSceneProgress(fStackProgress / iMaxLoadScene);
			
			yield return pAsyncOperation;
		}

		// 로딩 끝, 자연스러운 연출을 위해 0.25초 대기
		yield return new WaitForSecondsRealtime(0.25f);

		if (bManualCall_EventOnFinishLoadScene == false)
			EventCall_OnFinishLoadScene();

		p_EVENT_OnLoadSceneProgress = null;
		p_EVENT_OnStartLoadScene = null;

		System.GC.Collect();
	}

	private void ProcStart_Loading( ENUM_SCENE_NAME[] arrSceneName, bool bManualCall_EventOnFinishLoadScene )
	{
		_fStackProgress = 0f;
		_iMaxLoadScene = arrSceneName.Length;
		_bManualCall_EventOnFinishLoadScene = bManualCall_EventOnFinishLoadScene;
		_eSceneLoadState = ESceneLoadState.SceneLoadStart;

		Time.timeScale = 0;
		Scene pLastScene = SceneManager.GetActiveScene();

		//GameObject[] arrGameObjects = pLastScene.GetRootGameObjects();
		//int iLenObj = arrGameObjects.Length;
		//for (int i = 0; i < iLenObj; i++)
		//{
		//	GameObject pGameObject = arrGameObjects[i];
		//	if (pGameObject.hideFlags == HideFlags.DontSave)
		//		Destroy(pGameObject);
		//}

		if (p_EVENT_OnStartLoadScene != null)
			p_EVENT_OnStartLoadScene();

		LoadSceneMode eLoadSceneMode = LoadSceneMode.Single; // 처음 로딩하는 씬은 무조건 Single 로
		_listAsyncLoadScene.Clear();
		for (int i = 0; i < _iMaxLoadScene; i++)
		{
			AsyncOperation pAsyncOperation = SceneManager.LoadSceneAsync( arrSceneName[i].ToString(), eLoadSceneMode );
			pAsyncOperation.allowSceneActivation = false;
			pAsyncOperation.priority = i;

			_listAsyncLoadScene.Add( pAsyncOperation );
			eLoadSceneMode = LoadSceneMode.Additive;
		}
	}

	private IEnumerator ProcFinish_Loading()
	{
		_eSceneLoadState = ESceneLoadState.SceneLoadFinish;

		for (int i = 0; i < _iMaxLoadScene; i++)
		{
			AsyncOperation pAsyncOperation = _listAsyncLoadScene[i];
			pAsyncOperation.allowSceneActivation = true;

			_fStackProgress += 0.1f;

			if (p_EVENT_OnLoadSceneProgress != null)
				p_EVENT_OnLoadSceneProgress( _fStackProgress / _iMaxLoadScene );

			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSecondsRealtime(0.2f);

		Time.timeScale = 1;

		if (_bManualCall_EventOnFinishLoadScene == false)
			EventCall_OnFinishLoadScene();

		p_EVENT_OnLoadSceneProgress = null;
		p_EVENT_OnStartLoadScene = null;

		_eSceneLoadState = ESceneLoadState.None;
	}

	static public void DoLoadScene_FadeInOut( ENUM_SCENE_NAME eSceneName, float fFadeDuration, Color pColor, System.Action OnFinishLoading = null )
	{
		AutoFade.LoadLevel( eSceneName.ToString(), fFadeDuration * 0.5f, fFadeDuration * 0.5f, pColor );
		if (_OnFinishLoad_Scene == null && OnFinishLoading != null)
		{
			_OnFinishLoad_Scene = OnFinishLoading;
			_strCallBackRequest_SceneName = eSceneName.ToString();
		}
	}

	public void EventOnSlotPlayClip( CSoundSlot pSlot )
	{
		_pManagerSound.EventOnSlotPlayClip( pSlot );
	}

	public void EventOnSlotFinishClip( CSoundSlot pSlot )
	{
		_pManagerSound.EventOnSlotFinishClip( pSlot );
	}

	public void EventCall_OnFinishLoadScene()
	{
		if (p_EVENT_OnFinishLoadScene == null) return;

		p_EVENT_OnFinishLoadScene();
		p_EVENT_OnFinishLoadScene = null;
	}

	// ========================================================================== //

	// ===================================== //
	// protected - abstract & virtual        //
	// ===================================== //

	virtual protected void OnSceneLoaded( UnityEngine.SceneManagement.Scene pScene, UnityEngine.SceneManagement.LoadSceneMode eLoadMode ) { }
	virtual protected void OnLoadFinish_INI_PlayerSetting( bool bSuccess, SINI_UserSetting sUserrSetting ) { }
	virtual protected void OnLoadFinish_INI_Developer( bool bSuccess ) { }

	// ===================================== //
	// protected - Unity API                 //
	// ===================================== //

	protected override void OnAwake()
	{
		base.OnAwake();

		_iLocalDataLoadingCount_Request = 0;
		_iLocalDataLoadingCount_Finish = 0;
		_pJsonParser_Persistent = SCManagerParserJson.DoMakeClass( this, "", SCManagerResourceBase<SCManagerParserJson, string, TextAsset>.EResourcePath.PersistentDataPath );
		_pJsonParser_StreammingAssets = SCManagerParserJson.DoMakeClass( this, const_strLocalPath_INI, SCManagerResourceBase<SCManagerParserJson, string, TextAsset>.EResourcePath.StreamingAssets );
		_pManagerScene = new SCSceneLoader<ENUM_SCENE_NAME>();
		_pManagerScene.p_EVENT_OnSceneLoaded += ProcOnSceneLoaded;
		_pManagerSound = SCManagerSound<ENUM_SOUND_NAME>.DoMakeClass( this, "Sound", SCManagerResourceBase<SCManagerSound<ENUM_SOUND_NAME>, ENUM_SOUND_NAME, AudioClip>.EResourcePath.Resources );

		ProcParse_UserSetting();
		ProcParse_DevelopSetting();

		_pJsonParser_StreammingAssets.DoStartCo_GetStreammingAssetResource_Array<SINI_Sound>( EINI_JSON_FileName.Sound.ToString(), OnParseComplete_SoundSetting );
		_iLocalDataLoadingCount_Request++;

		_pJsonParser_StreammingAssets.DoStartCo_GetStreammingAssetResource<SINI_ApplicationSetting>( EINI_JSON_FileName.ApplicationSetting.ToString(), OnFinishParse_AppSetting );
		_iLocalDataLoadingCount_Request++;

		if (CManagerUILocalize.instance == null)
			CManagerUILocalize.EventMakeSingleton();

		CManagerUILocalize.instance.DoStartParse_Locale( CUIManagerLocalize_p_EVENT_OnChangeLocalize );
		_iLocalDataLoadingCount_Request++;
		//_pJsonParserINI.DoStartCo_GetStreammingAssetResource<SINI_UserSetting>(EINI_JSON_FileName.UserSetting.ToString(), OnParseComplete_UserSetting);
	}

	private void ProcParse_UserSetting()
	{
		if (_pJsonParser_Persistent.DoReadJson( EINI_JSON_FileName.UserSetting, out _sSetting_User ))
		{
			Debug.Log( "UserInfo - bParsingResult Is Success " + _sSetting_User.ID );

			_pManagerSound.DoSetVolumeEffect( _sSetting_User.fVolumeEffect );
			_pManagerSound.DoSetVolumeBGM( _sSetting_User.fVolumeBGM );
			OnLoadFinish_INI_PlayerSetting( true, _sSetting_User );
		}
		else
		{
			Debug.Log( "UserInfo - bParsingResult Is Fail" );

			_sSetting_User = new SINI_UserSetting();
			_pJsonParser_Persistent.DoWriteJson( EINI_JSON_FileName.UserSetting, _sSetting_User );
			if (Application.isEditor)
				_pJsonParser_StreammingAssets.DoWriteJsonArray( EINI_JSON_FileName.Sound, new SINI_Sound[] { new SINI_Sound(), new SINI_Sound() } );

			OnLoadFinish_INI_PlayerSetting( false, _sSetting_User );
		}
	}

	private void ProcParse_DevelopSetting()
	{
		if (_pJsonParser_Persistent.DoReadJson( EINI_JSON_FileName.DeveloperSetting, out _sSetting_Developer ) == false)
			_pJsonParser_Persistent.DoWriteJson(EINI_JSON_FileName.DeveloperSetting, new SINI_DeveloperSetting() );

		if (_pJsonParser_Persistent.DoReadJson( EINI_JSON_FileName.DeveloperSetting, out _sSetting_Developer ))
			OnParseComplete_DevelopSetting( true, _sSetting_Developer );
	}

	private void CUIManagerLocalize_p_EVENT_OnChangeLocalize()
	{
		SystemLanguage eCurLanguage = SystemLanguage.Unknown; // Application.systemLanguage
		try
		{
			eCurLanguage = (SystemLanguage)System.Enum.Parse( typeof( SystemLanguage ), _sSetting_User.strLanguage );
		}
		catch
		{
			List<SystemLanguage> listLocale = CManagerUILocalize.p_listLocale;

			if (listLocale.Contains( eCurLanguage ) == false)
				eCurLanguage = SystemLanguage.English;

			Debug.LogWarning( "UserSetting에서 Enum 파싱에 실패해서 기본언어로 바꿨다" + _sSetting_User.strLanguage );

			_sSetting_User.strLanguage = eCurLanguage.ToString();
			_pJsonParser_Persistent.DoWriteJson( EINI_JSON_FileName.UserSetting, _sSetting_User );
		}

		Debug.Log( "언어 설정 " + eCurLanguage );

		CManagerUILocalize.instance.DoSet_Localize( eCurLanguage );
		CheckLoadFinish_LocalDataAll();
	}

	static private void ProcOnSceneLoaded( UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1 )
	{
		if (CManagerUILocalize.instance != null)
			CManagerUILocalize.instance.DoSetLocalize_CurrentScene();

		p_pManagerScene.EventCheckIsLoadComplete();

		instance.OnSceneLoaded( arg0, arg1 );
		if (_OnFinishLoad_Scene != null && _strCallBackRequest_SceneName != null && _strCallBackRequest_SceneName.CompareTo( arg0.name ) == 0)
		{
			_strCallBackRequest_SceneName = "";
			System.Action OnFinishCurrentScene = _OnFinishLoad_Scene;
			_OnFinishLoad_Scene = null;
			OnFinishCurrentScene();
		}
	}


	protected override void OnUpdate()
	{
		base.OnUpdate();

		float fTotalProgress = 0f;

		if (_eSceneLoadState == ESceneLoadState.SceneLoadStart)
		{
			for (int i = 0; i < _iMaxLoadScene; i++)
				fTotalProgress += _listAsyncLoadScene[i].progress;

			if (p_EVENT_OnLoadSceneProgress != null)
				p_EVENT_OnLoadSceneProgress( _fStackProgress / _iMaxLoadScene );

			if (_fStackProgress > 0.9f * _iMaxLoadScene)
				StartCoroutine( ProcFinish_Loading() );
		}

		if (_fStackProgress < fTotalProgress)
			_fStackProgress += Time.unscaledDeltaTime * _iMaxLoadScene;
	}

	// ========================================================================== //

	// ===================================== //
	// private - [Proc] Function             //
	// 중요 로직을 처리                      //
	// ===================================== //

	static private void CheckLoadFinish_LocalDataAll()
	{
		if (_iLocalDataLoadingCount_Request <= ++_iLocalDataLoadingCount_Finish)
		{
			if (p_EVENT_OnLoadFinish_LocalData != null)
				p_EVENT_OnLoadFinish_LocalData();
		}
	}

	static private void CheckIsContainField<Struct>( string strFieldName )
	{
#if UNITY_EDITOR
		System.Type pType = typeof( Struct );
		System.Reflection.FieldInfo pField = pType.GetField( strFieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
		if (pField == null)
			Debug.LogError( "DB Error - Not Contain Field : " + strFieldName );
#endif
	}

	// ===================================== //
	// private - [Other] Function            //
	// 찾기, 계산 등의 비교적 단순 로직      //
	// ===================================== //

	private void OnParseComplete_SoundSetting( bool bSuccess, SINI_Sound[] arrSound )
	{
		if (bSuccess)
			_pManagerSound.EventSetINI( arrSound, _sSetting_User.fVolumeEffect, _sSetting_User.eVolumeOff );

		ENUM_SOUND_NAME[] arrSoundName = PrimitiveHelper.GetEnumArray<ENUM_SOUND_NAME>();
		if (Application.isEditor && arrSound == null || arrSound.Length < arrSoundName.Length)
		{
			Debug.Log( "Sound INI의 내용과 Enum SoundName과 길이가 맞지 않아 재조정" );

			List<SINI_Sound> listINISound = arrSound == null ? new List<SINI_Sound>() : arrSound.ToList();
			Dictionary<string, SINI_Sound> mapINISound = new Dictionary<string, SINI_Sound>();
			mapINISound.DoAddItem( arrSound );

			for (int i = 0; i < arrSoundName.Length; i++)
			{
				string strSoundName = arrSoundName[i].ToString();
				if (mapINISound.ContainsKey( strSoundName ) == false)
					listINISound.Add( new SINI_Sound( strSoundName, 0.5f ) );
			}

			_pJsonParser_StreammingAssets.DoWriteJsonArray( EINI_JSON_FileName.Sound, listINISound.ToArray() );
		}

		CheckLoadFinish_LocalDataAll();
	}

	private void OnParseComplete_DevelopSetting( bool bSuccess, SINI_DeveloperSetting sDeveloperSetting )
	{
		if (bSuccess)
		{
			_sSetting_Developer = sDeveloperSetting;

			//if (_sSetting_Developer.bDebugMode)
			// Application.logMessageReceived += CManagerUIShared.OnHandleLog;
		}
		else
		{
			_sSetting_Developer = new SINI_DeveloperSetting();
			Debug.Log( "DevelopSetting Parsing Fail - Check your DevelpSetting" );

			if (Application.isEditor)
				_pJsonParser_Persistent.DoWriteJson( EINI_JSON_FileName.DeveloperSetting, _sSetting_Developer );
		}

		OnLoadFinish_INI_Developer( bSuccess );
	}

	private void OnFinishParse_AppSetting( bool bResult, SINI_ApplicationSetting sAppSetting )
	{
		if(bResult == false)
		{
			Debug.Log("Error, AppSetting Is Null");
			_pJsonParser_StreammingAssets.DoWriteJson( EINI_JSON_FileName.ApplicationSetting, new SINI_ApplicationSetting() );
			return;
		}

		//_sSetting_App = sAppSetting;
		//System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
		//for (int i = 0; i < _sSetting_App.arrLogIgnore_Writer.Length; i++)
		//{
		//	pStrBuilder.Append( _sSetting_App.arrLogIgnore_Writer[i] );
		//	DebugCustom.AddIgnore_LogWriterList( _sSetting_App.arrLogIgnore_Writer[i].ConvertEnum<ELogWriter>() );

		//	if (i != _sSetting_App.arrLogIgnore_Writer.Length - 1)
		//		pStrBuilder.Append( ", " );
		//}
		//if (Application.isEditor)
		//	Debug.Log( "OnFinishParse_AppSetting - Debug Ignore Writer List : " + pStrBuilder.ToString() );
		//pStrBuilder.Length = 0;
		//for (int i = 0; i < _sSetting_App.arrLogIgnore_Level.Length; i++)
		//{
		//	pStrBuilder.Append( _sSetting_App.arrLogIgnore_Level[i] );
		//	DebugCustom.AddIgnore_LogLevel( _sSetting_App.arrLogIgnore_Level[i] );

		//	if (i != _sSetting_App.arrLogIgnore_Level.Length - 1)
		//		pStrBuilder.Append( ", " );
		//}

#if UNITY_EDITOR
		int iBundleCode = UnityEditor.PlayerSettings.Android.bundleVersionCode;
		if (_sSetting_App.iBundleCode != iBundleCode)
		{
			_sSetting_App.iBundleCode = iBundleCode;
			_pJsonParser_StreammingAssets.DoWriteJson( EINI_JSON_FileName.ApplicationSetting, _sSetting_App );
		}
#endif

		CheckLoadFinish_LocalDataAll();
	}
}
