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

[System.Flags]
public enum EVolumeOff
{
    None = 0,
    SoundEffect = 1,
    BGM = 2
}

[System.Flags]
public enum EDebugLevel
{
    Debug = 1,
    Warning = 2,
    Error = 4,
    Developer_1 = 8,
    Developer_2 = 16,
    Developer_3 = 32,
    ALL = 63,
}

public enum EBuildType
{
    Editor,
    Develop,
    QA,
    Release
}

[System.Serializable]
public class SINI_UserSetting
{
    public string strLanguage = "Korean";
    public float fMainVolume = 1.0f;
    public EVolumeOff eVolumeOff = EVolumeOff.None;
    public int bVibrationFlag;
    public string ID;
    public string strNickName;
}

[System.Serializable]
public class SINI_DevelopSetting
{
    public bool bDebugMode = true;
    public EDebugLevel eDebugFilter = EDebugLevel.ALL;
	public string strTestID;
}

public interface IDB_Insert
{
	StringPair[] IDB_Insert_GetField();
}

public struct StringPair
{
    public string strKey;
    public string strValue;

    public StringPair(string strKey, string strValue)
    {
        this.strKey = strKey; this.strValue = strValue;
    }

    public StringPair(string strKey, object pValue)
    {
        this.strKey = strKey; this.strValue = pValue.ToString();
    }

	public StringPair(string strKey, System.Enum eEnum)
	{
		this.strKey = strKey; this.strValue = eEnum.GetHashCode().ToString();
	}
}

[RequireComponent(typeof(CCompoDontDestroyObj))]
public class CManagerFrameWorkBase<CLASS, ENUM_SOUND_NAME, ENUM_EFFECT_NAME, ENUM_SCENE_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER> : CSingletonBase<CLASS>
    where CLASS : CManagerFrameWorkBase<CLASS, ENUM_SOUND_NAME, ENUM_EFFECT_NAME, ENUM_SCENE_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>
    where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where ENUM_EFFECT_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where ENUM_SCENE_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where CLASS_EFFECT : CEffectBase<CLASS_EFFECT, ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_SOUNDPLAYER>
    where CLASS_SOUNDPLAYER : CSoundPlayerBase<ENUM_SOUND_NAME>
{
    private const string const_strLocalPath_INI = "/INI";

    public enum EINI_JSON_FileName
    {
        Sound,
        UserSetting,
        DevelopSetting,
    }

    // ===================================== //
    // public - Variable declaration         //
    // ===================================== //

    static public CManagerNetworkDB_Project p_pNetworkDB { get { return CManagerNetworkDB_Project.instance; } }
    static public SCManagerSound<ENUM_SOUND_NAME> p_pManagerSound { get { return _pManagerSound; } }
    static public SCManagerEffect<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER> p_pManagerEffect { get { return _pManagerEffect; } }
    static public SCSceneLoader<ENUM_SCENE_NAME> p_pManagerScene { get {  return _pManagerScene; } }
    static public SCManagerParserJson p_pManagerJsonINI { get { return _pJsonParser_Persistent; } }

	public static System.Action p_EVENT_OnFirstChangeLocalize;

	// ===================================== //
	// protected - Variable declaration      //
	// ===================================== //

	static protected string _strCallBackRequest_SceneName;
    static protected System.Action _OnFinishLoad_Scene;

	protected SINI_UserSetting _sSetting_User;          public SINI_UserSetting p_sUserSetting { get { return _sSetting_User; } }
    protected SINI_DevelopSetting _sSetting_Developer;  public SINI_DevelopSetting p_sSetting_Developer {  get { return _sSetting_Developer; } }

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    static protected SCManagerSound<ENUM_SOUND_NAME> _pManagerSound;
    static protected SCManagerEffect<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER> _pManagerEffect;
    static protected SCSceneLoader<ENUM_SCENE_NAME> _pManagerScene;
    static protected SCManagerParserJson _pJsonParser_Persistent;
    static protected SCManagerParserJson _pJsonParser_StreammingAssets;

	static private List<iTween> _listTween = new List<iTween>();

	protected string _strID;	public string p_strID {  get { return _strID; } }

	// ========================================================================== //

	// ===================================== //
	// public - [Do] Function                //
	// 외부 객체가 요청                      //
	// ===================================== //
	
	static public void DoNetworkDB_CheckCount_IsEqualOrGreater<StructDB>(string strFieldName, object iCheckFieldCount, System.Action<bool> OnResult)
    {
        instance.StartCoroutine(p_pNetworkDB.CoExcutePHP(instance._strID, EPHPName.Check_Count, typeof(StructDB).ToString(), OnResult, new StringPair(strFieldName, iCheckFieldCount.ToString())));
    }

    static public void DoNetworkDB_UpdateAdd_If_CheckCount_IsEqualOrGreater<StructDB>(string strFieldName, object iCheckFieldCount, int iAddFieldCount, System.Action<bool, string> OnResult)
    {
        instance.StartCoroutine(p_pNetworkDB.CoExcuteAndGetValue(instance._strID, EPHPName.CheckCount_AndUpdateAdd, typeof(StructDB).ToString(), OnResult, new StringPair(strFieldName, iCheckFieldCount.ToString()), new StringPair(strFieldName, iAddFieldCount.ToString())));
    }

    static public void DoNetworkDB_UpdateAdd<StructDB>(string strFieldName, object iFieldCount, System.Action<bool, string> OnResult, params StringPair[] arrParams)
    {
        if(arrParams.Length != 0)
        {
            StringPair[] arrNewParams = new StringPair[arrParams.Length + 1];
            arrNewParams[0] = new StringPair(strFieldName, iFieldCount);

            for (int i = 0; i < arrParams.Length; i++)
                arrNewParams[i + 1] = arrParams[i];

            instance.StartCoroutine(p_pNetworkDB.CoExcuteAndGetValue(instance._strID, EPHPName.Update_Add, typeof(StructDB).ToString(), OnResult, arrNewParams));
        }
        else
            instance.StartCoroutine(p_pNetworkDB.CoExcuteAndGetValue(instance._strID, EPHPName.Update_Add, typeof(StructDB).ToString(), OnResult, new StringPair(strFieldName, iFieldCount.ToString())));
    }


    static public void DoNetworkDB_GetRange_Orderby_HighToLow<StructDB>(string strFieldName, int iGetDataCount, System.Action<bool, StructDB[]> OnFinishLoad)
    {
        instance.StartCoroutine(p_pNetworkDB.CoLoadDataFromServer_Json_Array(instance._strID, EPHPName.Get_Range, OnFinishLoad, new StringPair(strFieldName, iGetDataCount)));
    }

    static public void DoNetworkDB_Get_Single<StructDB>(System.Action<bool, StructDB> OnFinishLoad, params StringPair[] arrParams)
    {
        instance.StartCoroutine(p_pNetworkDB.CoLoadDataFromServer_Json(instance._strID, EPHPName.Get, OnFinishLoad, arrParams));
    }

    static public void DoNetworkDB_GetOrInsert_Single<StructDB>(System.Action<bool, StructDB> OnFinishLoad, params StringPair[] arrParams)
    {
        instance.StartCoroutine(p_pNetworkDB.CoLoadDataFromServer_Json(instance._strID, EPHPName.Get_OrInsert, OnFinishLoad, arrParams));
    }

    static public void DoNetworkDB_GetRandomKey(string strCheckOverlapTableName, System.Action<bool, string> OnFinishLoad, params StringPair[] arrParams)
    {
        instance.StartCoroutine(p_pNetworkDB.CoExcuteAndGetValue(null, EPHPName.Get_RandomKey, strCheckOverlapTableName, OnFinishLoad, arrParams));
    }

    static public void DoNetworkDB_Get_Array<StructDB>(System.Action<bool, StructDB[]> OnFinishLoad, params StringPair[] arrParams)
    {
        instance.StartCoroutine(p_pNetworkDB.CoLoadDataFromServer_Json_Array(instance._strID, EPHPName.Get, OnFinishLoad, arrParams));
    }

    /// <summary>
    /// DB에 Generic에 있는 필드 값을 덮어 씌운다.
    /// </summary>
    /// <typeparam name="StructDB"></typeparam>
    /// <param name="strFieldName">Generic에 있는 필드 명</param>
    /// <param name="strSetFieldValue">Generic에 있는 필드에 덮어씌울 값</param>
    /// <param name="OnResult">결과 함수</param>
    static public void DoNetworkDB_Update_Set<StructDB>(string strFieldName, object strSetFieldValue, System.Action<bool> OnResult)
    {
        instance.StartCoroutine(p_pNetworkDB.CoExcutePHP(instance._strID, EPHPName.Update_Set, typeof(StructDB).ToString(), OnResult, new StringPair(strFieldName, strSetFieldValue)));
    }

    static public void DoNetworkDB_Update_Set_Multi<StructDB>(System.Action<bool> OnResult, params StringPair[] arrParam)
    {
        instance.StartCoroutine(p_pNetworkDB.CoExcutePHP(instance._strID, EPHPName.Update_Set, typeof(StructDB).ToString(), OnResult, arrParam));
    }

    static public void DoNetworkDB_Update_Set_ServerTime<StructDB>(string strFieldName, System.Action<bool> OnResult)
    {
        instance.StartCoroutine(p_pNetworkDB.CoExcutePHP(instance._strID, EPHPName.Update_Set_ServerTime, typeof(StructDB).ToString(), OnResult, new StringPair(strFieldName, "")));
    }

	static public void DoNetworkDB_Insert<StructDB>(System.Action<bool> OnResult, StructDB pStructDB)
		where StructDB : IDB_Insert
	{
		instance.StartCoroutine(p_pNetworkDB.CoExcutePHP(instance._strID, EPHPName.Insert, typeof(StructDB).ToString(), OnResult, pStructDB.IDB_Insert_GetField()));
	}

	static public void DoNetworkDB_Delete<StructDB>(params StringPair[] arrParam)
	{
		if (instance._strID != null)
			instance.StartCoroutine(p_pNetworkDB.CoExcutePHP(instance._strID, EPHPName.DeleteInfo, typeof(StructDB).ToString(), null, arrParam));
		else
			Debug.Log("Delete는 strID에 null이오면 안됩니다.");
	}

	static public void DoNetworkDB_Insert<StructDB>(System.Action<bool> OnResult, params StringPair[] arrParam)
    {
        instance.StartCoroutine(p_pNetworkDB.CoExcutePHP(instance._strID, EPHPName.Insert, typeof(StructDB).ToString(), OnResult, arrParam));
    }

    public void DoShakeMobile()
    {
        if (Application.isPlaying && _sSetting_User.bVibrationFlag != 0)
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

	static public void DoSetTimeScale(float fTimeScale)
	{
		// ITween에서 흔들리는 모션을 TimeScale에 조종하기 위해..
		if (Time.timeScale != 0f && fTimeScale == 0f) // 플레이 중에 멈출때
		{
			iTween[] arrTween = FindObjectsOfType<iTween>();
			_listTween.Clear();
			for (int i = 0; i < arrTween.Length; i++)
			{
				_listTween.Add(arrTween[i]);
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

	static public void DoLoadScene(ENUM_SCENE_NAME eSceneName, LoadSceneMode eLoadSceneMode, System.Action OnFinishLoading = null)
    {
        SceneManager.LoadScene(eSceneName.ToString(), eLoadSceneMode);
        if(_OnFinishLoad_Scene == null && OnFinishLoading != null)
        {
            _OnFinishLoad_Scene = OnFinishLoading;
            _strCallBackRequest_SceneName = eSceneName.ToString();
        }
    }

    static public void DoLoadSceneAsync(ENUM_SCENE_NAME eSceneName, LoadSceneMode eLoadSceneMode, System.Action OnFinishLoading = null)
    {
        SceneManager.LoadSceneAsync(eSceneName.ToString(), eLoadSceneMode);
        if (_OnFinishLoad_Scene == null && OnFinishLoading != null)
        {
            _OnFinishLoad_Scene = OnFinishLoading;
            _strCallBackRequest_SceneName = eSceneName.ToString();
        }
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

	public void EventOnSlotPlayClip(CSoundSlot pSlot)
    {
        _pManagerSound.EventOnSlotPlayClip(pSlot);
    }

    public void EventOnSlotFinishClip(CSoundSlot pSlot)
    {
        _pManagerSound.EventOnSlotFinishClip(pSlot);
    }

    // ========================================================================== //

    // ===================================== //
    // protected - abstract & virtual        //
    // ===================================== //

    virtual protected void OnSceneLoaded(UnityEngine.SceneManagement.Scene pScene, UnityEngine.SceneManagement.LoadSceneMode eLoadMode) { }
	virtual protected void OnLoadFinish_INI_PlayerSetting( bool bSuccess, SINI_UserSetting sUserrSetting ) { }
	virtual protected void OnLoadFinish_INI_Developer( bool bSuccess) { }

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        MakeGameDataObject();

        _pJsonParser_Persistent = SCManagerParserJson.DoMakeClass(this, "", SCManagerResourceBase<SCManagerParserJson, string, TextAsset>.EResourcePath.PersistentDataPath);
        _pJsonParser_StreammingAssets = SCManagerParserJson.DoMakeClass(this, const_strLocalPath_INI, SCManagerResourceBase<SCManagerParserJson, string, TextAsset>.EResourcePath.StreamingAssets);
        _pManagerScene = new SCSceneLoader<ENUM_SCENE_NAME>();
        _pManagerScene.p_EVENT_OnSceneLoaded += ProcOnSceneLoaded;


		bool bParsingResult = _pJsonParser_Persistent.DoReadJson( EINI_JSON_FileName.UserSetting, out _sSetting_User );
		if (bParsingResult)
        {
            _pManagerSound.DoSetVolume(_sSetting_User.fMainVolume);
            _pJsonParser_StreammingAssets.DoStartCo_GetStreammingAssetResource_Array<SINI_Sound>(EINI_JSON_FileName.Sound.ToString(), OnParseComplete_SoundSetting);
        }
        else
        {
            _sSetting_User = new SINI_UserSetting();
            _pJsonParser_Persistent.DoWriteJson(EINI_JSON_FileName.UserSetting, _sSetting_User);
            if(Application.isEditor)
                _pJsonParser_StreammingAssets.DoWriteJsonArray(EINI_JSON_FileName.Sound, new SINI_Sound[] { new SINI_Sound(), new SINI_Sound() });
		}
		OnLoadFinish_INI_PlayerSetting( bParsingResult, _sSetting_User );

		_pJsonParser_StreammingAssets.DoStartCo_GetStreammingAssetResource<SINI_DevelopSetting>(EINI_JSON_FileName.DevelopSetting.ToString(), OnParseComplete_DevelopSetting);

		if (CManagerUILocalize.instance == null)
			CManagerUILocalize.EventMakeSingleton();

        CManagerUILocalize.instance.DoStartParse_Locale(CUIManagerLocalize_p_EVENT_OnChangeLocalize);
        //_pJsonParserINI.DoStartCo_GetStreammingAssetResource<SINI_UserSetting>(EINI_JSON_FileName.UserSetting.ToString(), OnParseComplete_UserSetting);
    }

    private void CUIManagerLocalize_p_EVENT_OnChangeLocalize()
    {
		SystemLanguage eCurLanguage = SystemLanguage.Unknown; // Application.systemLanguage
		try
        {
			eCurLanguage = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), _sSetting_User.strLanguage);
        }
        catch
        {
			List<SystemLanguage> listLocale = CManagerUILocalize.p_listLocale;

			if (listLocale.Contains(eCurLanguage) == false)
				eCurLanguage = SystemLanguage.English;

			Debug.LogWarning("UserSetting에서 Enum 파싱에 실패해서 기본언어로 바꿨다" + _sSetting_User.strLanguage);

            _sSetting_User.strLanguage = eCurLanguage.ToString();
            _pJsonParser_Persistent.DoWriteJson(EINI_JSON_FileName.UserSetting, _sSetting_User);
        }

		Debug.LogWarning("언어 설정 " + eCurLanguage);

        CManagerUILocalize.instance.DoSet_Localize(eCurLanguage);
		if (p_EVENT_OnFirstChangeLocalize != null)
			p_EVENT_OnFirstChangeLocalize();
	}

    private void ProcOnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
    {
        if (CManagerUILocalize.instance != null)
			CManagerUILocalize.instance.DoSetLocalize_CurrentScene();

        p_pManagerScene.EventCheckIsLoadComplete();

        OnSceneLoaded(arg0, arg1);
        if (_OnFinishLoad_Scene != null && _strCallBackRequest_SceneName != null && _strCallBackRequest_SceneName.CompareTo(arg0.name) == 0)
        {
            _strCallBackRequest_SceneName = "";
            System.Action OnFinishCurrentScene = _OnFinishLoad_Scene;
            _OnFinishLoad_Scene = null;
            OnFinishCurrentScene();
        }
    }

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

    private void MakeGameDataObject()
    {
        SCManagerGameData<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>.DoMakeClass(this);
        _pManagerSound = SCManagerGameData<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>.p_ManagerSound;
        _pManagerEffect = SCManagerGameData<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>.p_ManagerEffect;
    }

    private void OnParseComplete_SoundSetting(bool bSuccess, SINI_Sound[] arrSound)
    {
        if (bSuccess)
            _pManagerSound.EventSetINI(arrSound, _sSetting_User.fMainVolume, (EVolumeOff)_sSetting_User.eVolumeOff);
    }
    private void OnParseComplete_DevelopSetting(bool bSuccess, SINI_DevelopSetting sDeveloperSetting)
    {
        if (bSuccess)
        {
            _sSetting_Developer = sDeveloperSetting;

            //Debug.Log("Success DebugMode : " + _sSetting_Developer.bDebugMode + " DebugFilter : " + _sSetting_Developer.eDebugFilter);
            Debug.Log("Success DebugMode : " + _sSetting_Developer.bDebugMode + " Test ID : " + _sSetting_Developer.strTestID);

            //if (_sSetting_Developer.bDebugMode)
               // Application.logMessageReceived += CManagerUIShared.OnHandleLog;
        }
        else
        {
            _sSetting_Developer = new SINI_DevelopSetting();
            Debug.Log("Fail DebugMode : " + _sSetting_Developer.bDebugMode + "DebugFilter : " + _sSetting_Developer.eDebugFilter);
            if (Application.isEditor)
                _pJsonParser_StreammingAssets.DoWriteJson(EINI_JSON_FileName.DevelopSetting, _sSetting_Developer);
        }

		OnLoadFinish_INI_Developer( bSuccess);
	}

}
