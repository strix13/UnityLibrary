using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-24 오후 9:29:13
   Description : 
   Edit Log    : 
   ============================================ */

public class SCManagerPoolingBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE> : SCManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>
    where CLASS : SCManagerPoolingBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>, new()
    where ENUM_RESOURCE_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where RESOURCE : UnityEngine.Component
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */
    protected class SPoolingObject
    {
        public bool bEnable;
        public RESOURCE pResource;

        public SPoolingObject(RESOURCE pResource)
        {
            bEnable = false; this.pResource = pResource;
        }
    }
    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    protected CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, List<SPoolingObject>> _mapPoolingInstance = new CDictionary_ForEnumKey<ENUM_RESOURCE_NAME, List<SPoolingObject>>();
    protected CList<SPoolingObject> _listInstanceAll = new CList<SPoolingObject>();

    /* private - Variable declaration           */
    
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public RESOURCE GetResource_Disable(ENUM_RESOURCE_NAME eResourceName)
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
		else
		{
			if (pFindResource.transform.parent != _pBase.transform)
				pFindResource.transform.SetParent(_pBase.transform);
		}

		OnGetResource_Disable(ref pFindResource);

        return pFindResource;
    }

    public void DoStartPooling(List<ENUM_RESOURCE_NAME> listRequestPooling, int iPoolingCount)
    {
        for(int i = 0; i < listRequestPooling.Count; i++)
        {
            List<SPoolingObject> listPoolingInstance = new List<SPoolingObject>();
            ENUM_RESOURCE_NAME eResourceName = listRequestPooling[i];
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

    public void DoReturnResource(ENUM_RESOURCE_NAME eResourceName, RESOURCE pResource)
    {
        bool bSuccess = false;
		if (_mapPoolingInstance.ContainsKey(eResourceName) == false)
			return;

        List<SPoolingObject> listPoolingInstance = _mapPoolingInstance[eResourceName];
        for (int i = 0; i < listPoolingInstance.Count; i++)
        {
            if (listPoolingInstance[i].pResource == pResource)
            {
                bSuccess = true;
                ProcReturnResource(listPoolingInstance[i]);
                break;
            }
        }

        if (bSuccess == false && Application.isEditor)
            Debug.LogWarning(string.Format("{0}을 반환에 실패하였습니다!", eResourceName), pResource);
    }

    public void DoReturnResourceAll()
    {
        for (int i = 0; i < _listInstanceAll.Count; i++)
            ProcReturnResource(_listInstanceAll[i]);
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    virtual protected void OnInitManager() { }
    virtual protected void OnMakeResource(ENUM_RESOURCE_NAME eResourceName, ref RESOURCE pMakeResource) { }
    virtual protected void OnGetResource_Disable(ref RESOURCE pFindResource) { }
    virtual protected void OnReturnResource(ref RESOURCE pReturnResource) { }

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */
    
    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private RESOURCE MakeResource(ENUM_RESOURCE_NAME eResourceName)
    {
        RESOURCE pObjectMake = Object.Instantiate<RESOURCE>(DoGetResource_Origin(eResourceName));
        Transform pTransMake = pObjectMake.transform;

        pTransMake.SetParent(_pBase.transform);
        //pTransMake.localRotation = Quaternion.identity;
        //pTransMake.localScale = Vector3.one;
        OnMakeResource(eResourceName, ref pObjectMake);

        pObjectMake.gameObject.SetActive(false);

        return pObjectMake;
    }

    private void ProcReturnResource(SPoolingObject sPoolingObj)
    {
        RESOURCE pResource = sPoolingObj.pResource;
        OnReturnResource(ref pResource);
        sPoolingObj.bEnable = false;

		pResource.gameObject.SetActive(false);
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
