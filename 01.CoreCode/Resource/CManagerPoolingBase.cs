using UnityEngine;
using System.Collections.Generic;

public class CManagerPoolingBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE> : CManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>
    where CLASS : CManagerPoolingBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>
    where ENUM_RESOURCE_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where RESOURCE : Component
{
    /// <summary>
    /// 풀링하기 위해 리소스를 Wrapping한 내부 클래스
    /// </summary>
    protected class SPoolingObject
    {
        public bool bEnable;
        public RESOURCE pResource;

        public SPoolingObject(RESOURCE pResource)
        {
            bEnable = false; this.pResource = pResource;
        }
    }
    
    [SerializeField]
    private int _iPoolingCount = 1;
    [SerializeField]
    private bool _bAutoPoolingOnAwake = true;

    protected CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, List<SPoolingObject>> _mapPoolingInstance = new CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, List<SPoolingObject>>();
    protected List<SPoolingObject> _listInstanceAll = new List<SPoolingObject>();

    // ========================== [ Division ] ========================== //

    /// <summary>
    /// 현재 사용하지 않는 오브젝트를 얻습니다. 주의) 사용후 반드시 Return 바람
    /// </summary>
    /// <param name="eResourceName">Enum형태의 리소스 이름</param>
    /// <returns></returns>
    public RESOURCE DoGetResource_Disable(ENUM_RESOURCE_NAME eResourceName, bool bGameObjectActive = true)
    {
        RESOURCE pFindResource = null;
        if (_mapPoolingInstance.ContainsKey(eResourceName) == false)
            _mapPoolingInstance.Add(eResourceName, new List<SPoolingObject>());

        List<SPoolingObject> listPoolingObject = _mapPoolingInstance[eResourceName];
        for (int i = 0; i < listPoolingObject.Count; i++)
        {
            if (listPoolingObject[i].bEnable == false)
            {
                listPoolingObject[i].bEnable = true;
                pFindResource = listPoolingObject[i].pResource;
                break;
            }
        }

        if (pFindResource == null)
        {
            pFindResource = MakeResource(eResourceName);
            pFindResource.name += listPoolingObject.Count;
            SPoolingObject pPoolingObj = new SPoolingObject(pFindResource);
            pPoolingObj.bEnable = true;
            listPoolingObject.Add(pPoolingObj);
            _listInstanceAll.Add(pPoolingObj);
        }

        OnGetResource_Disable(eResourceName, ref pFindResource);

        pFindResource.gameObject.SetActive(bGameObjectActive);

        return pFindResource;
    }

    /// <summary>
    /// 현재 사용하지 않는 오브젝트를 얻습니다. 주의) 사용후 반드시 Return 바람
    /// </summary>
    /// <typeparam name="type">as 형변환 할 변수</typeparam>
    /// <param name="eResourceName">Enum형태의 리소스 이름</param>
    /// <returns></returns>
    public type DoGetResource_Disable<type>(ENUM_RESOURCE_NAME eResourceName)
        where type : RESOURCE
    {
        return DoGetResource_Disable(eResourceName) as type;
    }

    /// <summary>
    /// 사용했던 오브젝트를 반환합니다. 자동으로 GameObject가 Disable 됩니다.
    /// </summary>
    /// <param name="eResourceName">Enum형태의 리소스 이름</param>
    /// <param name="pResource">사용한 리소스</param>
    public void DoReturnResource(ENUM_RESOURCE_NAME eResourceName, RESOURCE pResource, bool bIgnoreSuccess = false)
    {
		bool bSuccess = false;
        List<SPoolingObject> listPoolingInstance = _mapPoolingInstance[eResourceName];
        for (int i = 0; i < listPoolingInstance.Count; i++)
        {
            if(listPoolingInstance[i].pResource == pResource)
            {
                bSuccess = true;
                ProcReturnResource(listPoolingInstance[i]);
                break;
            }
        }

        if(bIgnoreSuccess == false && bSuccess == false)
            Debug.LogWarning(string.Format("{0}을 반환에 실패하였습니다!", eResourceName), pResource);
    }

    /// <summary>
    /// Enum형태의 리소스 이름의 List에 있는 오브젝트만 풀링을 위해 오브젝트를 새로 생성합니다. 
    /// </summary>
    /// <param name="listRequestPooling">풀링할 Enum 형태의 리소스 리스트</param>
    public void DoStartPooling(List<ENUM_RESOURCE_NAME> listRequestPooling)
    {
        for (int i = 0; i < listRequestPooling.Count; i++)
        {
            List<SPoolingObject> listPoolingInstance = new List<SPoolingObject>();
            ENUM_RESOURCE_NAME eResourceName = listRequestPooling[i];
            int iPoolingCount = OnGetPoolingCount(eResourceName);
            for (int j = 0; j < iPoolingCount; j++)
            {
                RESOURCE pResource = MakeResource(eResourceName);
                pResource.name += j;
                SPoolingObject pPoolingObj = new SPoolingObject(pResource);
                listPoolingInstance.Add(pPoolingObj);
                _listInstanceAll.Add(pPoolingObj);
            }
            _mapPoolingInstance.Add(eResourceName, listPoolingInstance);
        }

        OnInitManager();
    }

	/// <summary>
	/// 사용한 모든 오브젝트를 강제로 리턴시킵니다. 리턴시 GameObject가 Disable 됩니다.
	/// </summary>
	public void DoReturnResourceAll()
    {
        for (int i = 0; i < _listInstanceAll.Count; i++)
            ProcReturnResource(_listInstanceAll[i]);
    }

    // 후크 함수 테이블
    // ========================== [ Division ] ========================== //

    virtual protected void OnInitManager() { }
    virtual protected void OnMakeResource(ENUM_RESOURCE_NAME eResourceName, ref RESOURCE pMakeResource) { }
    virtual protected void OnGetResource_Disable(ENUM_RESOURCE_NAME eResourceName, ref RESOURCE pFindResource) { }
    virtual protected void OnReturnResource(ref RESOURCE pReturnResource) { }
    virtual protected int OnGetPoolingCount(ENUM_RESOURCE_NAME eResourceName) { return _iPoolingCount; }
	
    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        if (_bAutoPoolingOnAwake == false) return;

        List<int> listResourceNameInt = _mapResourceOrigin.Keys.ToList();
        List<ENUM_RESOURCE_NAME> listResourceName = listResourceNameInt.ConvertEnumList< ENUM_RESOURCE_NAME>();
        for (int i = 0; i < _mapResourceOrigin.Count; i++)
        {
            List<SPoolingObject> listPoolingInstance = new List<SPoolingObject>();
            ENUM_RESOURCE_NAME eResourceName = listResourceName[i];
            int iPoolingCount = OnGetPoolingCount(eResourceName);
            for (int j = 0; j < iPoolingCount; j++)
            {
                RESOURCE pResource = MakeResource(eResourceName);
                pResource.name += j;
                SPoolingObject pPoolingObj = new SPoolingObject(pResource);
                listPoolingInstance.Add(pPoolingObj);
                _listInstanceAll.Add(pPoolingObj);
            }
            _mapPoolingInstance.Add(eResourceName, listPoolingInstance);
        }

        OnInitManager();
    }

    protected override void OnInitResourceOrigin()
    {
        base.OnInitResourceOrigin();

        List<RESOURCE> listResourceOrigin = _mapResourceOrigin.Values.ToList();
        for(int i = 0; i < listResourceOrigin.Count; i++)
            listResourceOrigin[i].gameObject.SetActive(false);
    }

    // ========================== [ Division ] ========================== //

    private RESOURCE MakeResource(ENUM_RESOURCE_NAME eResourceName)
    {
        RESOURCE pObjectMake = Object.Instantiate<RESOURCE>(GetResource_Origin(eResourceName));
        Transform pTransMake = pObjectMake.transform;

        pTransMake.SetParent(transform);
        //pTransMake.localRotation = Quaternion.identity;
        //pTransMake.localScale = Vector3.one;
        pObjectMake.gameObject.SetActive(false);
        
        OnMakeResource(eResourceName, ref pObjectMake);
        return pObjectMake;
    }

    private void ProcReturnResource(SPoolingObject sPoolingObj)
    {
        RESOURCE pResource = sPoolingObj.pResource;
        OnReturnResource(ref pResource);
        pResource.gameObject.SetActive(false);
        sPoolingObj.bEnable = false;

        if (pResource.transform.parent != transform)
            pResource.transform.SetParent(transform);
    }
}
