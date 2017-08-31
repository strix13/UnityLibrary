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
    public static Dictionary<SystemLanguage, Dictionary<string, string>> p_mapLocaleData { get { return _mapLocaleData; } }
    public static List<SystemLanguage> p_listLocale { get { return _listLocale; } }

    public static SystemLanguage p_eCurrentLocalize { get { return _eCurrentLocalize; } }

    public static List<EventDelegate> p_OnFinishParse_LocFile = new List<EventDelegate>();
    public static event System.Action p_EVENT_OnChangeLocalize = null;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	private static Dictionary<SystemLanguage, Dictionary<string, string>> _mapLocaleData = new Dictionary<SystemLanguage, Dictionary<string, string>>();
    private static Dictionary<string, CUICompoLocalize> _mapCompoLocalizeData = new Dictionary<string, CUICompoLocalize>();

    private static Dictionary<string, string> _mapLocaleDataCurrent;
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
	}

	public void DoRegist_Localize(SystemLanguage eLocalize, string strKey, string strValue)
    {
		if (_mapLocaleData.ContainsKey(eLocalize) == false)
        {
            _listLocale.Add(eLocalize);
            _mapLocaleData.Add(eLocalize, new Dictionary<string, string>());
        }

		if (_mapLocaleData[eLocalize].ContainsKey(strKey) == false)
			_mapLocaleData[eLocalize].Add(strKey, strValue);
		else
			Debug.LogWarning(string.Format("이미 로컬라이즈 데이터 랭귀지 : {0}에 키값{1}은 등록되있다.", eLocalize, strKey));
	}

	// 중복키 수정 해야함
    public void DoRegist_CompoLocalize(CUICompoLocalize pCompo)
    {
		string strKey = pCompo.p_strLangKey;
        if (_mapCompoLocalizeData.ContainsKey(strKey) == false)
            _mapCompoLocalizeData.Add(strKey, pCompo);
    }

	public void DoSetLocalize_CurrentScene()
	{
		// 우선 클리어 해준다
		_mapCompoLocalizeData.Clear();

		// 단일 UI Root에서 다수로 변경
		UIRoot[] arrRoot = FindObjectsOfType<UIRoot>();

		int iLen = arrRoot.Length;
		for (int i = 0; i < iLen; i++)
		{
			CUICompoLocalize[] arrCompoLocalize = arrRoot[i].transform.GetComponentsInChildren<CUICompoLocalize>(true);

			int iLen_CompoLocalize = arrCompoLocalize.Length;
			for (int j = 0; j < iLen_CompoLocalize; j++)
				DoRegist_CompoLocalize(arrCompoLocalize[j]);
		}

		Debug.LogWarning("DoSetLocalize_CurrentScene + " + _eCurrentLocalize);

		if (_eCurrentLocalize != SystemLanguage.Unknown)
			DoSet_Localize(_eCurrentLocalize);
		else
			Debug.LogWarning("현재 지정된 언어를 알수없습니다.");
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
        IEnumerator< KeyValuePair < string, CUICompoLocalize>> pIter = _mapCompoLocalizeData.GetEnumerator();
        while(pIter.MoveNext())
        {
            bool bExistLabel = true;
            KeyValuePair<string, CUICompoLocalize> pCurrent = pIter.Current;
            if(_mapLocaleDataCurrent.ContainsKey(pCurrent.Key))
            {
                // NGUI Label 버그로 인해 \\n을 변경해야 한다. 그래야 다음줄로 변경됨
                if (pCurrent.Value.p_pUILabel == null)
                {
                    pCurrent.Value.EventOnAwake();
                    if (pCurrent.Value.p_pUILabel == null)
                        bExistLabel = false;
                }

				if (bExistLabel)
					pCurrent.Value.p_strText = _mapLocaleDataCurrent[pCurrent.Key];
				else
					Debug.LogWarning("UILabel이 없다.." + pCurrent.Value.name, pCurrent.Value);
            }
        }

		if (p_EVENT_OnChangeLocalize != null)
			p_EVENT_OnChangeLocalize();
	}

	public static string DoGetCurrentLocalizeValue(string strLocKey)
	{
		return (_mapLocaleData.ContainsKey(_eCurrentLocalize) && _mapLocaleData[_eCurrentLocalize].ContainsKey(strLocKey)) ?
				_mapLocaleData[_eCurrentLocalize][strLocKey] : string.Empty;
	}

	public static List<string> DoGetLocalizeKeyContains(string strLocKey)
	{
		List<string> listLocKeys = new List<string>();

		if (_mapLocaleData.ContainsKey(_eCurrentLocalize))
		{
			IEnumerator<KeyValuePair<string, string>> pIter = _mapLocaleData[_eCurrentLocalize].GetEnumerator();
			while (pIter.MoveNext())
			{
				KeyValuePair<string, string> pCurrent = pIter.Current;

				string strKey = pCurrent.Key;

				if (strKey.Contains(strLocKey))
					listLocKeys.Add(strKey);
			}
		}

		return listLocKeys;
	}

	public static List<string> DoGetLocalizeValueContains(string strLocKey)
	{
		List<string> listLocValue = new List<string>();

		if (_mapLocaleData.ContainsKey(_eCurrentLocalize))
		{
			IEnumerator<KeyValuePair<string, string>> pIter = _mapLocaleData[_eCurrentLocalize].GetEnumerator();
			while (pIter.MoveNext())
			{
				KeyValuePair<string, string> pCurrent = pIter.Current;

				string strKey = pCurrent.Key;

				if (strKey.Contains(strLocKey))
					listLocValue.Add(pCurrent.Value);
			}
		}

		return listLocValue;
	}


	public static UILabel DoSetCurrentLocalize_Label(string strLocKey, params object[] arrStrFormat)
	{
        if (_mapCompoLocalizeData.ContainsKey(strLocKey) && _mapCompoLocalizeData[strLocKey].p_pUILabel != null &&
            _mapLocaleDataCurrent != null && _mapLocaleDataCurrent.ContainsKey(strLocKey))
		{
            _mapCompoLocalizeData[strLocKey].p_pUILabel.text = string.Format(_mapLocaleData[_eCurrentLocalize][strLocKey], arrStrFormat).Replace("\\n", "\n");
            return _mapCompoLocalizeData[strLocKey].p_pUILabel;
		}

        Debug.LogWarning(strLocKey + " 가 _mapLocaleData 에 없습니다.");
        return null;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public static void EventAddCallBack_OnFinishParse(EventDelegate.Callback OnFinishParse)
	{
		if (_bIsFinishParse)
			OnFinishParse();
		else
			EventDelegate.Add(p_OnFinishParse_LocFile, OnFinishParse, true);
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

        if(Application.isEditor)
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

				Debug.LogWarning("로컬라이징 파싱이 완료됨.");

                _OnFinishParse_First();
                EventDelegate.Execute(p_OnFinishParse_LocFile);
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
			if (arrStr[i] == null || arrStr[i].Length <= 2 || arrStr[i].StartsWith("//")) continue;

			if (arrStr[i].StartsWith("/auto"))
			{
				_iAutoIndex = 0;
				_bAutoIndex = true;
				continue;
			}
			else if (arrStr[i].StartsWith("/endauto"))
			{
				_bAutoIndex = false;
				continue;
			}

			string[] arrLang = arrStr[i].Split('=');

			string strLocKey = arrLang[0].TrimStart(arrChrTrim).TrimEnd(arrChrTrim);
			string strLocValue = arrLang[1].TrimStart(arrChrTrim).TrimEnd(arrChrTrim).Replace("\\n", "\n");

			if (_bAutoIndex)
				strLocKey = string.Format("{0}_{1}", strLocKey, _iAutoIndex++);

			DoRegist_Localize(eLocale, strLocKey, strLocValue);
		}
	}
	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
