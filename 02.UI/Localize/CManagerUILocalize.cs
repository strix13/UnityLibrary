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
public class CManagerUILocalize : CSingletonMonoBase<CManagerUILocalize>
{
	/* const & readonly declaration             */
	private const string const_strLocalePath = "Locale";
	private const string const_strLocaleFileExtension = ".loc";

	private static readonly char[] const_arrChrTrim = new char[] { '\t', '\r', ' ' };


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

	private int _iParsingFinishCount = 0;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */
    public void DoStartParse_Locale(System.Action OnFinishParse)
	{
		_eCurrentLocalize = SystemLanguage.Unknown;

		p_EVENT_OnFinishParse_LocFile += OnFinishParse;
		_iParsingFinishCount = 0;
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
		pCompo.EventOnAwake();

		string strKey = pCompo.name;
		if (_mapCompoLocalizeData.ContainsKey(strKey) == false)
			_mapCompoLocalizeData.Add(strKey, new List<CUICompoLocalize>());

		_mapCompoLocalizeData[strKey].Add(pCompo);
	}

	public void DoSetLocalize_CurrentScene()
	{
		if(_bIsFinishParse == false)
		{
			p_EVENT_OnFinishParse_LocFile += DoSetLocalize_CurrentScene;
			return;
		}

		// 우선 클리어 해준다
		_mapCompoLocalizeData.Clear();

		// 루트 트랜트폼만 찾는다 그후 하위 컴포넌트 탐색
		Transform[] arrTransform = FindObjectsOfType<Transform>();

		int iLen = arrTransform.Length;
		for (int i = 0; i < iLen; i++)
		{
			Transform pTrans = arrTransform[i];
			if (pTrans.parent != null) continue;

			CUICompoLocalize[] arrCompoLocalize = pTrans.transform.GetComponentsInChildren<CUICompoLocalize>(true);
			int iLen_CompoLocalize = arrCompoLocalize.Length;
			for (int j = 0; j < iLen_CompoLocalize; j++)
				DoRegist_CompoLocalize(arrCompoLocalize[j]);
		}

		//Debug.Log( "DoSetLocalize_CurrentScene : " + _eCurrentLocalize );

		if (_eCurrentLocalize != SystemLanguage.Unknown)
			DoSet_Localize( _eCurrentLocalize );
		else
			Debug.LogWarning( "현재 지정된 언어를 알수없습니다." );
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
            //bool bExistLabel = true;

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
					pValueCurrent.DoChangeText(DoGetCurrentLocalizeValue(strKey));
					//if (pValueCurrent.p_pUILabel == null)
					//{
					//	pValueCurrent.EventOnAwake();
					//	if (pValueCurrent.p_pUILabel == null)
					//		bExistLabel = false;
					//}

					//if (bExistLabel)
					//	pValueCurrent.DoChangeText(DoGetCurrentLocalizeValue(strKey));
					//else
					//	Debug.LogWarning("UILabel이 없다.." + pValueCurrent.name, pValueCurrent);
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

	public static string DoGetLocalizeRandomText_OrNull(string strKey)
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

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent<CCompoDontDestroyObj>()._bIsSingleton = true;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private IEnumerator CoProcParse_Locale(SystemLanguage eLocale)
	{
		_pStrBuilder.Length = 0;
		
#if UNITY_EDITOR
		_pStrBuilder.Append("file://");
#endif
		_pStrBuilder.Append(Application.streamingAssetsPath).Append("/").Append(const_strLocalePath).Append("/")
					.Append(eLocale.ToString()).Append(const_strLocaleFileExtension);
		
		WWW pReader = new WWW(_pStrBuilder.ToString());
		yield return pReader;
		_iParsingFinishCount++;

		if (_iParsingFinishCount >= (int)SystemLanguage.Unknown + 1)
		{
			_bIsFinishParse = true;

			Debug.Log( "로컬라이징 파싱이 완료됨." );

			if (p_EVENT_OnFinishParse_LocFile != null)
			{
				p_EVENT_OnFinishParse_LocFile();
				p_EVENT_OnFinishParse_LocFile = null;
			}
		}

		if (pReader.error != null || pReader.bytes.Length == 0)
			yield break;

		string strText = pReader.text;
		if (pReader.bytes.Length >= 3 && pReader.bytes[0] == 239 && pReader.bytes[1] == 187 && pReader.bytes[2] == 191)   // UTF8 코드 확인
			strText = Encoding.UTF8.GetString(pReader.bytes, 3, pReader.bytes.Length - 3);

		string[] arrStr = strText.Split('\n');

		int iLen = arrStr.Length;
		for (int i = 0; i < iLen; i++)
		{
			string strLine = arrStr[i];
			if (string.IsNullOrEmpty(strLine) || strLine.StartsWith("//")) continue;

			StringPair pStringPair = DoSplitText_OrEmpty( strLine );
			if(StringPair.IsEmpty(pStringPair) == false)
				DoRegist_Localize(eLocale, pStringPair.strKey, pStringPair .strValue);
		}
	}

	static public StringPair DoSplitText_OrEmpty(string strText, char chSplit = '=')
	{
		string[] arrLang = strText.Split( '=' );
		if (arrLang.Length < 2) return StringPair.Empty;

		string strLocKey = arrLang[0].TrimStart( const_arrChrTrim ).TrimEnd( const_arrChrTrim );
		string strLocValue = arrLang[1].TrimStart( const_arrChrTrim ).TrimEnd( const_arrChrTrim ).Replace( "\\n", "\n" );

		return new StringPair( strLocKey, strLocValue );
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
