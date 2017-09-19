using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-19 오후 1:53:44
   Description : 
   Edit Log    : 
   ============================================ */

public class CObjectBase : MonoBehaviour
{
    protected bool _bIsExcuteAwake = false;	public bool p_bIsExcuteAwake {  get { return _bIsExcuteAwake; } }

    protected Transform _pTransformCached; public Transform p_pTransCached { get { return _pTransformCached; } }
    protected GameObject _pGameObjectCached; public GameObject p_pGameObjectCached { get { return _pGameObjectCached; } }

	protected Dictionary<string, GameObject> _mapGameObject;

	// ========================== [ Division ] ========================== //

	void Awake()
    {
        if(_bIsExcuteAwake == false)
            OnAwake();
    }

    void Start() { OnStart(); }
    void OnEnable() { OnEnableObject(); }
    void Update() { OnUpdate(); }
    void OnDisable() { OnDisableObject(); }

    // ========================== [ Division ] ========================== //

    virtual protected void OnAwake()
    {
        if (_bIsExcuteAwake == false)
        {
			_pTransformCached = transform;
            _pGameObjectCached = gameObject;
			_bIsExcuteAwake = true;
        }
	}

	virtual protected void OnStart() { }
    virtual protected void OnEnableObject() { }
    virtual protected void OnUpdate() { }
    virtual protected void OnDisableObject() { }

	// ========================== [ Division ] ========================== //

	public GameObject GetGameObject<Enum_ObjectName>( Enum_ObjectName eObjectName )
	{
		return GetGameObject( eObjectName.ToString() );
	}
	
	public GameObject GetGameObject(string strGameObjName)
	{
		GameObject pFindGameObj = null;

		if (_mapGameObject == null)
			_mapGameObject = new Dictionary<string, GameObject>();

		if (_mapGameObject.ContainsKey(strGameObjName) == false)
		{
			Transform[] arrCompo = GetComponentsInChildrenOnly<Transform>();
			for (int i = 0; i < arrCompo.Length; i++)
			{
				if (arrCompo[i].name.Equals(strGameObjName))
				{
					pFindGameObj = arrCompo[i].gameObject;
					break;
				}
			}

			if (pFindGameObj == null)
				Debug.LogWarning(string.Format("{0}에 {1}이 없다", name, strGameObjName, this));
			else
				_mapGameObject[strGameObjName] = pFindGameObj;
		}
		else
			pFindGameObj = _mapGameObject[strGameObjName];

		return pFindGameObj;
	}

	public void EventOnAwake()
	{
		if (_bIsExcuteAwake == false)
			OnAwake();
	}

	public void EventShowAnd_AutoDisable(GameObject pObjectDisable, float fDelaySec)
	{
		pObjectDisable.SetActive( true );
		StartCoroutine( CoDisableObject( pObjectDisable, fDelaySec ) );
	}

	public bool GetComponent<COMPONENT>( out COMPONENT pComponent )
		where COMPONENT : UnityEngine.Component
	{
		pComponent = GetComponent<COMPONENT>();

		return pComponent != null;
	}

	public bool GetComponentInChildren<COMPONENT>( out COMPONENT pComponent )
		where COMPONENT : UnityEngine.Component
	{
		pComponent = GetComponentInChildren<COMPONENT>();

		return pComponent != null;
	}

	static public COMPONENT GetComponentInChildrenOnly<COMPONENT>(MonoBehaviour pTarget) 
		where COMPONENT : UnityEngine.Component
    {
        COMPONENT pFindCompo = null;
        COMPONENT[] arrChildrenCompo = pTarget.GetComponentsInChildren<COMPONENT>();
        for (int i = 0; i < arrChildrenCompo.Length; i++)
        {
            if (arrChildrenCompo[i].transform != pTarget.transform)
            {
                pFindCompo = arrChildrenCompo[i];
                break;
            }
        }

		return pFindCompo;
    }
	
    public COMPONENT[] GetComponentsInChildrenOnly<COMPONENT>() where COMPONENT : UnityEngine.Component
    {
        COMPONENT[] arrCompoAll = GetComponentsInChildren<COMPONENT>(true);

        int iCount = 0;
        for (int i = 0; i < arrCompoAll.Length; i++)
        {
            if (arrCompoAll[i].transform != transform)
                iCount++;
        }

        // Array는 동적으로 Item 추가가 안되기 때문에 카운트를 센 후 Init후 사용
        int iIndex = 0;
        COMPONENT[] arrCompoChildren = new COMPONENT[iCount];
        for (int i = 0; i < arrCompoAll.Length; i++)
        {
            if (arrCompoAll[i].transform != transform)
                arrCompoChildren[iIndex++] = arrCompoAll[i];
        }

        return arrCompoChildren;
    }

	protected void EventDelayExcuteCallBack(System.Action OnAfterDelayAction, float fDelaySec)
	{
		StartCoroutine( CoDelayAction( OnAfterDelayAction, fDelaySec ));
	}

	protected IEnumerator CoDelayAction( System.Action OnAfterDelayAction, float fDelaySec )
	{
		yield return new WaitForSeconds( fDelaySec );

		OnAfterDelayAction();
	}

	protected IEnumerator CoDisableObject( GameObject pObjectDisable, float fDelaySec )
	{
		yield return new WaitForSeconds( fDelaySec );

		pObjectDisable.SetActive( false );
	}
}
