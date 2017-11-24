using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-04-05 오후 1:24:58
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(CCompoDontDestroyObj))]
public class CManagerUILocalize : CSingletonBase<CManagerUILocalize>
{
	/* const & readonly declaration             */
	private const string const_strLocalePath = "Locale";
	private const string const_strLocaleFileExtension = ".loc";

    /* enum & struct declaration                */

    /* public - Variable declaration            */
    public static Dictionary<SystemLanguage, Dictionary<string, List<string>>> p_mapLocaleData { get { return _mapLocaleData; } }
    public static List<SystemLanguage> p_listLocale { get { return _listLocale; } }

    public static SystemLanguage p_eCurrentLocalize { get { return _eCurrentLocalize; } }

    public static event System.Action p_EVENT_OnFinishParse_LocFile;
    public static event System.Action p_EVENT_OnChangeLocalize;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	private static Dictionary<SystemLanguage, Dictionary<string, List<string>>> _mapLocaleData = new Dictionary<SystemLanguage, Dictionary<string, List<string>>>();
    private static Dictionary<string, List<CUICompoLocalize>> _mapCompoLocalizeData = new Dictionary<string, List<CUICompoLocalize>>();

    private static Dictionary<string, List<string>> _mapLocaleDataCurrent;
    private static List<SystemLanguage> _listLocale = new List<SystemLanguage>();  
    private static SystemLanguage _eCurrentLocalize;
    private static bool _bIsFinishParse = false;    public static bool p_bIsFinishParse {  get { return _bIsFinishParse; } }

    private StringBuilder _pStrBuilder = new StringBuilder();
    private System.Action _OnFinishParse_First;

	private int _iAutoIndex;
	private bool _bAutoIndex;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */
    public void DoStartParse_Locale(System.Action OnFinishParse)
	{
		_eCurrentLocalize = SystemLanguage.Unknown;

		_OnFinishParse_First = OnFinishParse;
        for (int i = 0; i <= (int)SystemLanguage.Unknown; i++)
            StartCoroutine(CoProcParse_Locale((SystemLanguage)i));

		//System.IO.Directory.GetFiles(path)
	}

	public void DoRegist_Localize(SystemLanguage eLocalize, string strKey, string strValue)
    {
		if (_mapLocaleData.ContainsKey(eLocalize) == false)
        {
            _listLocale.Add(eLocalize);
            _mapLocaleData.Add(eLocalize, new Dictionary<string, List<string>>());
        }

		if (_mapLocaleData[eLocalize].ContainsKey(strKey) == false)
			_mapLocaleData[eLocalize].Add(strKey, new List<string>());

		_mapLocaleData[eLocalize][strKey].Add(strValue);
	}

    public void DoRegist_CompoLocalize(CUICompoLocalize pCompo)
    {
		string strKey = pCompo.p_strLangKey;
		if (_mapCompoLocalizeData.ContainsKey(strKey) == false)
			_mapCompoLocalizeData.Add(strKey, new List<CUICompoLocalize>());

		_mapCompoLocalizeData[strKey].Add(pCompo);
	}

	public void DoSetLocalize_CurrentScene()
	{
		// 우선 클리어 해준다
		_mapCompoLocalizeData.Clear();

		// 단일 UI Root에서 다수로 변경

#if NGUI
		UIRoot[] arrRoot = FindObjectsOfType<UIRoot>();

		int iLen = arrRoot.Length;
		for (int i = 0; i < iLen; i++)
		{
			CUICompoLocalize[] arrCompoLocalize = arrRoot[i].transform.GetComponentsInChildren<CUICompoLocalize>(true);

			int iLen_CompoLocalize = arrCompoLocalize.Length;
			for (int j = 0; j < iLen_CompoLocalize; j++)
				DoRegist_CompoLocalize(arrCompoLocalize[j]);
		}

		DebugCustom.Log_ForCore( EDebugFilterDefault.System, "DoSetLocalize_CurrentScene : " + _eCurrentLocalize );

		if (_eCurrentLocalize != SystemLanguage.Unknown)
			DoSet_Localize( _eCurrentLocalize );
		else
			DebugCustom.Log_ForCore( EDebugFilterDefault.Warning_Core, "현재 지정된 언어를 알수없습니다." );
#endif
	}

	public void DoSet_Localize(SystemLanguage eLocalize)
	{
        if (_mapLocaleData.ContainsKey(eLocalize) == false)
		{
			Debug.LogWarning("_mapLocaleData 에 " + eLocalize + " 키가 없습니다.");
            return;
		}

		_eCurrentLocalize = eLocalize;

		_mapLocaleDataCurrent = _mapLocaleData[_eCurrentLocalize];
        IEnumerator<KeyValuePair<string, List<CUICompoLocalize>>> pIter = _mapCompoLocalizeData.GetEnumerator();
        while(pIter.MoveNext())
        {
            bool bExistLabel = true;

            KeyValuePair<string, List<CUICompoLocalize>> pCurrent = pIter.Current;
			string strKey = pCurrent.Key;

			// 중복 로컬라이징 컴포넌트가 있을 수 있으므로 리스트로 변경후 세팅
			List<CUICompoLocalize> listValue = pCurrent.Value;
			if (_mapLocaleDataCurrent.ContainsKey(strKey))
            {
				int iCount = listValue.Count;
				for (int i = 0; i < iCount; i++)
				{
					CUICompoLocalize pValueCurrent = listValue[i];
#if NGUI
					if (pValueCurrent.p_pUILabel == null)
					{
						pValueCurrent.EventOnAwake();
						if (pValueCurrent.p_pUILabel == null)
							bExistLabel = false;
					}

					if (bExistLabel)
						pValueCurrent.p_strText = DoGetCurrentLocalizeValue(strKey);
					else
						Debug.LogWarning("UILabel이 없다.." + pValueCurrent.name, pValueCurrent);
#endif
				}
            }
        }

		if (p_EVENT_OnChangeLocalize != null)
			p_EVENT_OnChangeLocalize();
	}

	public static List<string> DoGetLocalizeValueContains(string strKey)
	{
		if (ProcCheckValidLocalizeValue(strKey) == false) return null;

		return _mapLocaleData[_eCurrentLocalize][strKey];
	}

	public static string DoGetCurrentLocalizeValue(string strKey, int iIndex = 0)
	{
		if (ProcCheckValidLocalizeValue(strKey) == false) return null;

		return _mapLocaleData[_eCurrentLocalize][strKey][iIndex];
	}

	public static string DoGetLocalizeRandomText(string strKey)
	{
		List<string> listLocalizeValue = DoGetLocalizeValueContains(strKey);
		if (listLocalizeValue == null)
			return null;

		int iLen = listLocalizeValue.Count;
		int iRand = Random.Range(0, iLen);

		return listLocalizeValue[iRand];
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public static void EventAddCallBack_OnFinishParse(System.Action OnFinishParse)
	{
		if (_bIsFinishParse)
			OnFinishParse();
		else
			p_EVENT_OnFinishParse_LocFile += OnFinishParse;
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private IEnumerator CoProcParse_Locale(SystemLanguage eLocale)
	{
		_pStrBuilder.Length = 0;

        if (Application.isEditor)
    		_pStrBuilder.Append("file://");

        _pStrBuilder.Append(Application.streamingAssetsPath).Append("/").Append(const_strLocalePath).Append("/")
					.Append(eLocale.ToString()).Append(const_strLocaleFileExtension);

		WWW pReader = new WWW(_pStrBuilder.ToString());
		yield return pReader;

        if (pReader.error != null && pReader.error.Length != 0)
        {
			// Unknown이 Language List의 마지막이기 때문에 완료 함수를 호출
			if (eLocale == SystemLanguage.Unknown)
            {
                _bIsFinishParse = true;

				Debug.Log( "로컬라이징 파싱이 완료됨.");

                _OnFinishParse_First();

                if (p_EVENT_OnFinishParse_LocFile != null)
				{
					p_EVENT_OnFinishParse_LocFile();
					p_EVENT_OnFinishParse_LocFile = null;
				}
			}
            yield break;
        }

		if (pReader.bytes.Length == 0)
		{
			yield break;
		}

		string strText = pReader.text;
		if (pReader.bytes.Length >= 3 && pReader.bytes[0] == 239 && pReader.bytes[1] == 187 && pReader.bytes[2] == 191)   // UTF8 코드 확인
			strText = Encoding.UTF8.GetString(pReader.bytes, 3, pReader.bytes.Length - 3);

		string[] arrStr = strText.Split('\n');
		char[] arrChrTrim = new char[] { '\t', '\r', ' ' };

		int iLen = arrStr.Length;
		for (int i = 0; i < iLen; i++)
		{
			string strLine = arrStr[i];
			if (string.IsNullOrEmpty(strLine) || strLine.StartsWith("//")) continue;

			string[] arrLang = strLine.Split('=');
			if (arrLang.Length < 2) continue;

			string strLocKey = arrLang[0].TrimStart(arrChrTrim).TrimEnd(arrChrTrim);
			string strLocValue = arrLang[1].TrimStart(arrChrTrim).TrimEnd(arrChrTrim).Replace("\\n", "\n");

			DoRegist_Localize(eLocale, strLocKey, strLocValue);
		}
	}

	private static bool ProcCheckValidLocalizeValue(string strKey)
	{
		return (_mapLocaleData.ContainsKey(_eCurrentLocalize) &&
			_mapLocaleData[_eCurrentLocalize].ContainsKey(strKey) &&
			_mapLocaleData[_eCurrentLocalize][strKey].Count > 0);
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
