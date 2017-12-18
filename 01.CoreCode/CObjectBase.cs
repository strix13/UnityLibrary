#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : Strix
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CObjectBase : MonoBehaviour
{
    protected bool _bIsExcuteAwake = false;	public bool p_bIsExcuteAwake {  get { return _bIsExcuteAwake; } }

    protected Transform _pTransformCached; public Transform p_pTransCached { get { return _pTransformCached; } }
    protected GameObject _pGameObjectCached; public GameObject p_pGameObjectCached { get { return _pGameObjectCached; } }

	public Vector3 p_vecPos
	{
		get { return _pTransformCached.position; }
		set { _pTransformCached.position = value; }
	}

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

	protected virtual void LateUpdate() { }
	
	// ========================== [ Division ] ========================== //
	
	public GameObject GetGameObject<TObjectName>( TObjectName tGameObjName, bool bPrintError = true)
	{
		string strGameObjName = tGameObjName.ToString();
		GameObject pFindGameObj = null;

		Transform[] arrCompo = GetComponentsInChildrenOnly<Transform>();
		for (int i = 0; i < arrCompo.Length; i++)
		{
			if (arrCompo[i].name.Equals(strGameObjName))
			{
				pFindGameObj = arrCompo[i].gameObject;
				break;
			}
		}

		if (pFindGameObj == null && bPrintError)
			Debug.Log(string.Format("{0}에 {1}이 없다", name, strGameObjName, this));

		return pFindGameObj;
	}
	
	public void EventOnAwake()
	{
		if (_bIsExcuteAwake == false)
			OnAwake();
	}

	public void EventShowAnd_AutoDisable(GameObject pObjectTarget, float fDelaySec)
	{
		pObjectTarget.SetActive( true );
		StartCoroutine( CoDisableObject( pObjectTarget, fDelaySec ) );
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

	public bool GetComponentInChildren<COMPONENT>( string strObjectName, out COMPONENT pComponent )
		where COMPONENT : UnityEngine.Component
	{
		GameObject pObjectFind = GetGameObject( strObjectName, false );
		if (pObjectFind != null)
			pComponent = pObjectFind.GetComponent<COMPONENT>();
		else
			pComponent = null;

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
#if UNITY_EDITOR
		if(Application.isEditor && gameObject.activeInHierarchy)
#endif
			StartCoroutine(CoDelayAction(OnAfterDelayAction, fDelaySec));
	}

	protected UI_ELEMENT FindUIElement<UI_ELEMENT>( Dictionary<string, UI_ELEMENT> mapUIElements, string strUIElement, bool bIgnoreError = false )
		where UI_ELEMENT : Component
	{
		if (mapUIElements == null)
			mapUIElements = new Dictionary<string, UI_ELEMENT>();

		if (mapUIElements.ContainsKey( strUIElement ))
			return mapUIElements[strUIElement];

		UI_ELEMENT[] arrUIElements = GetComponentsInChildren<UI_ELEMENT>( true );
		int iLen = arrUIElements.Length;
		for (int i = 0; i < iLen; i++)
		{
			UI_ELEMENT pUIElement = arrUIElements[i];
			string strElementName = pUIElement.name;

			if (strElementName.Equals( strUIElement ))
			{
				if (mapUIElements.ContainsKey( strElementName ) == false)
					mapUIElements.Add( strUIElement, pUIElement );
				else
					Debug.Log( strElementName + " 키 값이 중복되었습니다.", this );

				return pUIElement;
			}
		}

		if(bIgnoreError == false)
			Debug.Log( strUIElement + " 를 찾을 수 없습니다... ", this );

		return null;
	}

	protected IEnumerator CoDelayAction( System.Action OnAfterDelayAction, float fDelaySec )
	{
		yield return SCManagerYield.GetWaitForSecond( fDelaySec );

		OnAfterDelayAction();
	}

	protected IEnumerator CoDisableObject( GameObject pObjectDisable, float fDelaySec )
	{
		yield return SCManagerYield.GetWaitForSecond( fDelaySec );

		pObjectDisable.SetActive( false );
	}
}
