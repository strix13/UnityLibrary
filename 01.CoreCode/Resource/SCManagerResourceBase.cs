using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 3:25:00
// Description : Core 레벨에서 사용할 수 있도록 Monobehaviour로 안되있는 Resource 매니져. 싱글톤.
// Edit Log    : 
// ============================================ 

public class SCManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>
    where CLASS : SCManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>, new()
    where ENUM_RESOURCE_NAME : System.IConvertible, System.IComparable
    where RESOURCE : UnityEngine.Object
{
    // ===================================== //
    // public - Variable declaration         //
    // ===================================== //

    public enum EResourcePath
    {
        Resources,
        StreamingAssets,
        PersistentDataPath
    }

    static public CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, RESOURCE> p_mapResourceOrigin { get { return _mapResourceOrigin; } }
    static public CLASS instance {  get { return _pInstance; } }

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    static protected CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, RESOURCE> _mapResourceOrigin = new CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, RESOURCE>();
    static protected CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, List<RESOURCE>> _mapResourceOrigin_Multiple = new CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, List<RESOURCE>>();

    static protected MonoBehaviour _pBase;

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    static private CLASS _pInstance;

    protected string _strResourceLocalPath = null;
    protected EResourcePath _eResourcePath;
    protected string _strFolderPath;
    private StringBuilder _pStrBuilder = new StringBuilder();

    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //

    static public CLASS DoMakeClass(MonoBehaviour pBaseClass, string strFolderPath, EResourcePath eResourcePath = EResourcePath.Resources)
    {
		if(pBaseClass == null)
		{
			Debug.LogWarning("Base Class가 없습니다. " + typeof(CLASS).ToString());
			return null;
		}

        _pInstance = new CLASS();
        _pBase = pBaseClass;
        _pInstance._eResourcePath = eResourcePath;
        _pInstance._strResourceLocalPath = strFolderPath;
        bool bIsMultipleResource = false;
        _pInstance.OnMakeClass(pBaseClass, ref bIsMultipleResource);

        if (eResourcePath == EResourcePath.Resources)
        {
            if (bIsMultipleResource)
                _pInstance.InitResourceOrigin_Multiple();
            else
                _pInstance.InitResourceOrigin();
            _pInstance.OnMakeClass_AfterInitResource(pBaseClass);
        }

        return _pInstance;
    }

    public void DoStartCo_GetStreammingAssetResource<TResource>(ENUM_RESOURCE_NAME eResourceName, System.Action<bool, TResource> OnGetResource)
    {
        _pBase.StartCoroutine(CoGetResource_StreammingAsset(eResourceName.ToString(), OnGetResource));
    }

    public void DoStartCo_GetStreammingAssetResource_Array<TResource>(ENUM_RESOURCE_NAME eResourceName, System.Action<bool, TResource[]> OnGetResource)
    {
        _pBase.StartCoroutine(CoGetResource_StreammingAsset_Array(eResourceName.ToString(), OnGetResource));
    }

    // ===================================== //
    // public - [Getter And Setter] Function //
    // ===================================== //

    public RESOURCE DoGetResource_Origin(ENUM_RESOURCE_NAME eResourceName)
    {
        RESOURCE pFindResource;
        if (_mapResourceOrigin.TryGetValue(eResourceName, out pFindResource) == false)
            Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eResourceName));

        return pFindResource;
    }

    static public List<RESOURCE> DoGetResource_Multiple(ENUM_RESOURCE_NAME eResourceName)
    {
        List<RESOURCE> pFindResource;
        if (_mapResourceOrigin_Multiple.TryGetValue(eResourceName, out pFindResource) == false)
            Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eResourceName));

        return pFindResource;
    }

	// ========================================================================== //

	// ===================================== //
	// protected - [Event] Function          //
	// 프랜드 객체가 요청                    //
	// ===================================== //

	protected virtual void OnMakeClass(MonoBehaviour pBaseClass, ref bool bIsMultipleResource) { }
    protected virtual void OnMakeClass_AfterInitResource(MonoBehaviour pBaseClass) { }
    protected virtual bool OnWWWToResource<TResource>(WWW www, ref TResource pResource) { return false; }
    protected virtual bool OnWWWToResource_Array<TResource>(WWW www, ref TResource[] arrResource) { return false; }
    protected virtual string OnGetFileExtension() { return ""; }

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    protected void InitResourceOrigin()
    {
		_mapResourceOrigin.Clear();
		RESOURCE[] arrResources = Resources.LoadAll<RESOURCE>(_strResourceLocalPath + "/");
        for (int i = 0; i < arrResources.Length; i++)
        {
			ENUM_RESOURCE_NAME eResourceName = default( ENUM_RESOURCE_NAME );
			if(arrResources[i].name.ConvertEnum(out eResourceName))
				_mapResourceOrigin.Add(eResourceName, arrResources[i]);
        }
    }

    protected void InitResourceOrigin_Multiple()
    {
        RESOURCE[] arrResources = Resources.LoadAll<RESOURCE>(_strResourceLocalPath + "/");
        ENUM_RESOURCE_NAME[] arrResourceName = PrimitiveHelper.GetEnumArray<ENUM_RESOURCE_NAME>();

        for (int i = 0; i < arrResources.Length; i++)
        {
            bool bSuccess = false;
            ENUM_RESOURCE_NAME eResourceName = default(ENUM_RESOURCE_NAME);

            for(int j = 0; j < arrResourceName.Length; j++)
            {
                if(arrResources[i].name.Contains(arrResourceName[j].ToString()))
                {
                    eResourceName = arrResourceName[j];
                    bSuccess = true;
                    break;
                }
            }

            if (bSuccess)
            {
                if (_mapResourceOrigin_Multiple.ContainsKey(eResourceName) == false)
                    _mapResourceOrigin_Multiple[eResourceName] = new List<RESOURCE>();

                _mapResourceOrigin_Multiple[eResourceName].Add(arrResources[i]);
            }
            else
                Debug.LogWarning(string.Format("{0} 을 파싱에 실패했습니다.", arrResources[i].name));
        }
    }

    private IEnumerator CoGetResource_StreammingAsset<TResource>(string strResourceName, System.Action<bool, TResource> OnGetResource)
    {
        _pStrBuilder.Length = 0;
        if (Application.isEditor)
            _pStrBuilder.Append("file://");

        _pStrBuilder.Append(_strFolderPath);
        _pStrBuilder.Append("/");
        _pStrBuilder.Append(strResourceName + OnGetFileExtension());

        WWW www = new WWW(_pStrBuilder.ToString());
        yield return www;

        if (www.error != null && www.error.Length != 0)
        {
            Debug.LogWarning(www.error);
            OnGetResource(false, default(TResource));
        }
        else
        {
            TResource pResource = default(TResource);
            if (OnWWWToResource(www, ref pResource))
                OnGetResource(true, pResource);
            else
                Debug.LogWarning(string.Format("{0}이 {1}을 WWW To Resource 변환 중 에러가 났다.", GetType().ToString(), strResourceName));
        }

        yield break;
    }

    private IEnumerator CoGetResource_StreammingAsset_Array<TResource>(string strResourceName, System.Action<bool, TResource[]> OnGetResource)
    {
        _pStrBuilder.Length = 0;
        if (Application.isEditor)
            _pStrBuilder.Append("file://");

        _pStrBuilder.Append(_strFolderPath);
        _pStrBuilder.Append("/");
        _pStrBuilder.Append(strResourceName + OnGetFileExtension());

        //Debug.Log("Path : " + _pStrBuilder.ToString());

        WWW www = new WWW(_pStrBuilder.ToString());
        yield return www;

        if (www.error != null && www.error.Length != 0)
        {
            Debug.LogWarning(www.error);
            OnGetResource(false, null);
        }

        TResource[] arrResource = null;
        if (OnWWWToResource_Array(www, ref arrResource))
            OnGetResource(true, arrResource);
        else
            Debug.LogWarning(string.Format("{0}이 {1}을 WWW To Resource 변환 중 에러가 났다.", GetType().ToString(), strResourceName));

        yield break;
    }

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //


}
