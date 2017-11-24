using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-05-31 오후 11:06:49
   Description : 
   Edit Log    : 
   ============================================ */

public class CManagerPoolingExtendBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE> : CManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>
    where CLASS : CManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>
    where ENUM_RESOURCE_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where RESOURCE : UnityEngine.Component
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected - Field declaration         */

    /* private - Field declaration           */

    [SerializeField]
    private int _iPoolingCount = 0;
    [SerializeField]
    private bool _bExcuteAwake_OnMake = false;

    protected CDictionary_ForEnumKey<int, RESOURCE> _mapPoolingInstance = new CDictionary_ForEnumKey<int, RESOURCE>();
    protected Dictionary<int, ENUM_RESOURCE_NAME> _mapPoolingResourceType = new Dictionary<int, ENUM_RESOURCE_NAME>();
    protected Dictionary<ENUM_RESOURCE_NAME, Queue<RESOURCE>> _queuePoolingDisable = new Dictionary<ENUM_RESOURCE_NAME, Queue<RESOURCE>>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /// <summary>
    /// 현재 사용하지 않는 오브젝트를 얻습니다. 주의) 사용후 반드시 Return 바람
    /// </summary>
    /// <param name="eResourceName">Enum형태의 리소스 이름</param>
    /// <returns></returns>
    public RESOURCE DoGetResource_Disable(ENUM_RESOURCE_NAME eResourceName, bool bGameObjectActive = true)
    {
        RESOURCE pFindResource = null;
        if (_queuePoolingDisable[eResourceName].Count == 0)
            pFindResource = MakeResource(eResourceName);
        else
            pFindResource = _queuePoolingDisable[eResourceName].Dequeue();
        
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
    /// <param name="pResource">사용한 리소스</param>
    public void DoReturnResource(RESOURCE pResource)
    {
        ProcReturnResource(pResource);
    }

    /// <summary>
    /// Enum형태의 리소스 이름의 List에 있는 오브젝트만 풀링을 위해 오브젝트를 새로 생성합니다. 
    /// </summary>
    /// <param name="listRequestPooling">풀링할 Enum 형태의 리소스 리스트</param>
    public void DoStartPooling(List<ENUM_RESOURCE_NAME> listRequestPooling)
    {
        for (int i = 0; i < listRequestPooling.Count; i++)
        {
            ENUM_RESOURCE_NAME eResourceName = listRequestPooling[i];
            _queuePoolingDisable.Add(eResourceName, new Queue<RESOURCE>());

            for (int j = 0; j < _iPoolingCount; j++)
            {
                RESOURCE pResource = MakeResource(eResourceName);
                ProcReturnResource(pResource);
            }
        }
    }

    /// <summary>
    /// 사용한 모든 오브젝트를 강제로 리턴시킵니다. 리턴시 GameObject가 Disable 됩니다.
    /// </summary>
    public void DoReturnResourceAll()
    {
        List<RESOURCE> listValue = _mapPoolingInstance.Values.ToList();
        for (int i = 0; i < listValue.Count; i++)
            ProcReturnResource(listValue[i]);
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    virtual protected void OnMakeResource(ENUM_RESOURCE_NAME eResourceName, ref RESOURCE pMakeResource) { }
    virtual protected void OnGetResource_Disable(ENUM_RESOURCE_NAME eResourceName, ref RESOURCE pFindResource) { }
    virtual protected void OnReturnResource(ENUM_RESOURCE_NAME eResourceName, ref RESOURCE pReturnResource) { }

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnInitResourceOrigin()
    {
        base.OnInitResourceOrigin();
        
        List<RESOURCE> listOrigin = _mapResourceOrigin.Values.ToList();
        for(int i = 0; i < listOrigin.Count; i++)
            listOrigin[i].gameObject.SetActive(_bExcuteAwake_OnMake);

        List<int> listResourceNameInt = _mapResourceOrigin.Keys.ToList();
        List<ENUM_RESOURCE_NAME> listResourceName = listResourceNameInt.ConvertEnumList<ENUM_RESOURCE_NAME>();
        DoStartPooling(listResourceName);
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private RESOURCE MakeResource(ENUM_RESOURCE_NAME eResourceName)
    {
        RESOURCE pObjectMake = Object.Instantiate<RESOURCE>(GetResource_Origin(eResourceName));
        Transform pTransMake = pObjectMake.transform;

        pTransMake.SetParent(transform);
        pObjectMake.name += _queuePoolingDisable[eResourceName].Count;

        int hInstanceID = pObjectMake.GetInstanceID();
        _mapPoolingInstance.Add(hInstanceID, pObjectMake);
        _mapPoolingResourceType.Add(hInstanceID, eResourceName);
        
        OnMakeResource(eResourceName, ref pObjectMake);
        return pObjectMake;
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

    private void ProcReturnResource(RESOURCE pResource)
    {
        int hInstanceID = pResource.GetInstanceID();
		
		if (_mapPoolingInstance.ContainsKey(hInstanceID) == false ||
            _mapPoolingResourceType.ContainsKey(hInstanceID) == false)
        {
            Debug.LogWarning(pResource.name + " 리턴 실패!!", this);
            return;
        }

        ENUM_RESOURCE_NAME eResourceName = _mapPoolingResourceType[hInstanceID];

        OnReturnResource(eResourceName, ref pResource);
        pResource.gameObject.SetActive(false);
        
        if(_queuePoolingDisable[eResourceName].Contains(pResource))
        {
            //Debug.LogWarning(pResource.name + "은 이미 반환되어있다..");
            return;
        }

        _queuePoolingDisable[eResourceName].Enqueue(pResource);
    }
}
