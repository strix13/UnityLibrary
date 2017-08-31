using UnityEngine;
using System.Collections;
using System;

abstract public class CManagerResourceBase<CLASS, ENUM_RES_NAME, RESOURCE> : CSingletonBase<CLASS>
    where CLASS : CManagerResourceBase<CLASS, ENUM_RES_NAME, RESOURCE>
    where ENUM_RES_NAME : System.IConvertible, System.IComparable
    where RESOURCE : UnityEngine.Component
{
    [SerializeField]
    private string _ResourceLocalPath = null;

    protected CDictionary_ForEnumKey<ENUM_RES_NAME, RESOURCE> _mapResourceOrigin = new CDictionary_ForEnumKey<ENUM_RES_NAME, RESOURCE>();

    // ========================== [ Division ] ========================== //

    public RESOURCE GetResource_Origin(ENUM_RES_NAME eResourceName)
    {
        RESOURCE pFindResource;
        if (_mapResourceOrigin.TryGetValue(eResourceName, out pFindResource) == false)
            Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eResourceName), this);

        return pFindResource;
    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        InitResourceOrigin();
    }

    virtual protected void OnInitResourceOrigin() { }
    virtual protected void OnGetResource_Origin(ENUM_RES_NAME eResourceName, RESOURCE pResource) { }

    // ========================== [ Division ] ========================== //

    protected void InitResourceOrigin()
    {
        GameObject[] arrResources = Resources.LoadAll<GameObject>(_ResourceLocalPath + "/");
        for (int i = 0; i < arrResources.Length; i++)
        {
            bool bSuccess = true;
            ENUM_RES_NAME eResourceName = default(ENUM_RES_NAME);

            try
            {
                eResourceName = (ENUM_RES_NAME)System.Enum.Parse(typeof(ENUM_RES_NAME), arrResources[i].name);
            }
            catch
            {
                bSuccess = false;
                Debug.LogWarning(string.Format("{0} 이 {1}에 존재하지 않습니다.", arrResources[i].name, typeof(ENUM_RES_NAME), this));
            }

            if (bSuccess)
            {
                RESOURCE pResource = arrResources[i].GetComponent<RESOURCE>();
                if (pResource == null)
                    pResource = arrResources[i].AddComponent<RESOURCE>();

                _mapResourceOrigin.Add(eResourceName, pResource);
                OnGetResource_Origin(eResourceName, pResource);
            }
        }

        OnInitResourceOrigin();
    }
}
