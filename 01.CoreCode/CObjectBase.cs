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
using System.Reflection;

public class CObjectBase : MonoBehaviour
{
	private enum EKeyType
	{
		None,
		String,
		Enum,
		CustomKey,
	}

	protected bool _bIsExcuteAwake = false;	public bool p_bIsExcuteAwake {  get { return _bIsExcuteAwake; } }
	private bool _bIsExcuteEnable = false;

    protected Transform _pTransformCached; public Transform p_pTransCached { get { return _pTransformCached; } }
    protected GameObject _pGameObjectCached; public GameObject p_pGameObjectCached { get { return _pGameObjectCached; } }

	private Transform _pOldParent;

	public Vector3 p_vecPos
	{
		get { return _pTransformCached.position; }
		set { _pTransformCached.position = value; }
	}

	// ========================== [ Division ] ========================== //

	public void DoReturnToParent()
	{
		transform.parent = _pOldParent;
	}

	public void DoSetParent_Local(Transform pTrans_Parent)
	{
		transform.SetParent(pTrans_Parent);
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
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
			UpdateGetComponentAttribute();

			_pTransformCached = transform;
            _pGameObjectCached = gameObject;

			_pOldParent = _pTransformCached;

			_bIsExcuteAwake = true;
        }
	}

	virtual protected void OnEnableObject()
	{
		if (_bIsExcuteEnable == false)
		{
			_bIsExcuteEnable = true;
			return;
		}

		OnAfterEnableObject();
	}

	virtual protected void OnStart() { }
    virtual protected void OnUpdate() { }
	protected virtual void OnAfterEnableObject() { }
	virtual protected void OnDisableObject() { }

	protected virtual void LateUpdate() { }
	
	// ========================== [ Division ] ========================== //
	
	public GameObject GetGameObject<TObjectName>( TObjectName tGameObjName, bool bPrintError = true)
	{
		if (tGameObjName == null)
			return null;

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
			Debug.LogWarning(string.Format("{0}에 {1}이 없다", name, strGameObjName), this);

		return pFindGameObj;
	}
	
	public void EventOnAwake_Force()
	{
		OnAwake();
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
		pComponent = GetComponentInChildren<COMPONENT>(true);

		return pComponent != null;
	}

	public void GetComponentInChildren_InitEnumerator<KEY, COMPONENT>(Dictionary<KEY, COMPONENT> mapInitTarget)
		where COMPONENT : UnityEngine.Component
	{
		if (mapInitTarget == null)
			mapInitTarget = new Dictionary<KEY, COMPONENT>();
		else
			mapInitTarget.Clear();

		System.Type pType = typeof( KEY );
		EKeyType eKeyType = EKeyType.None;
		if (pType.Equals( typeof( string ) ))
			eKeyType = EKeyType.String;
		else if (pType.IsEnum)
			eKeyType = EKeyType.Enum;

		if(eKeyType == EKeyType.None)
		{
			Debug.LogWarning( name + " GetComponentInChildren_InitEnumerator eKeyType == EKeyType.None", this );
			return;
		}

		COMPONENT[] arrComponent = GetComponentsInChildren<COMPONENT>( true );
		for(int i = 0; i < arrComponent.Length; i++)
		{
			KEY Key = default( KEY);
			switch (eKeyType)
			{
				case EKeyType.CustomKey: break;
				case EKeyType.String: Key = (KEY)(object)arrComponent[i].name; break;
				case EKeyType.Enum:
					try
					{
						Key = (KEY)System.Enum.Parse( typeof( KEY ), arrComponent[i].name );
					}
					catch
					{
						Debug.LogWarning( name + " GetComponentInChildren_InitEnumerator Enum Parsing Error - " + arrComponent[i].name, this );
						continue;
					}
					break;
			}
			mapInitTarget.Add( Key, arrComponent[i] );
		}
	}

	public UnityEngine.Component GetComponentInChildren( string strObjectName, System.Type pComponentType )
	{
		GameObject pObjectFind = GetGameObject( strObjectName, false );
		UnityEngine.Component pComponentFind = null;
		if (pObjectFind != null)
			pComponentFind = pObjectFind.GetComponent( pComponentType );

		return pComponentFind;
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
        COMPONENT[] arrChildrenCompo = pTarget.GetComponentsInChildren<COMPONENT>(true);
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
        int iIndex = 0;
        COMPONENT[] arrCompoChildren = new COMPONENT[arrCompoAll.Length - 1];
        for (int i = 0; i < arrCompoAll.Length; i++)
        {
            if (arrCompoAll[i].transform != transform)
                arrCompoChildren[iIndex++] = arrCompoAll[i];
        }

        return arrCompoChildren;
    }

	protected void EventDelayExcuteCallBack(System.Action OnAfterDelayAction, float fDelaySec)
	{
		if(gameObject.activeInHierarchy)
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
		for (int i = 0; i < arrUIElements.Length; i++)
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

	public int GetSiblingIndex()
	{
		return p_pTransCached.GetSiblingIndex();
	}


    private void UpdateGetComponentAttribute()
    {
        FieldInfo[] arrFields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
		ProcUpdateComponentAttribute( arrFields );

		arrFields = GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Instance );
		ProcUpdateComponentAttribute( arrFields );

		arrFields = GetType().GetFields( BindingFlags.Public | BindingFlags.Static );
		ProcUpdateComponentAttribute( arrFields );

		arrFields = GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Static );
		ProcUpdateComponentAttribute( arrFields );
	}

	private void ProcUpdateComponentAttribute( FieldInfo[] arrFields )
	{
		for (int i = 0; i < arrFields.Length; i++)
		{
			FieldInfo pField = arrFields[i];
			object[] arrCustomAttributes = pField.GetCustomAttributes( true );

			for (int j = 0; j < arrCustomAttributes.Length; j++)
			{
				GetComponentAttributeBase pGetcomponentAttribute = arrCustomAttributes[j] as GetComponentAttributeBase;
				if (pGetcomponentAttribute == null) continue;

				bool bIsRequireSetValue = true;
				System.Type pTypeField = pField.FieldType;
				object pComponent = null;

				if (pTypeField.IsArray)
				{
					pComponent = pGetcomponentAttribute.GetComponent( this, pTypeField.GetElementType() );
				}
				else if (pTypeField.IsGenericType)
				{
					System.Type pTypeField_Generic = pTypeField.GetGenericTypeDefinition();
					if (pTypeField_Generic == typeof( List<> ))
					{
						bIsRequireSetValue = false;
						pComponent = pGetcomponentAttribute.GetComponent( this, pTypeField.GetGenericArguments()[0] );

						pField.SetValue( this, System.Activator.CreateInstance( pTypeField, pComponent ) );
					}
					//else if (pTypeField_Generic)
					//{
					//	bIsRequireSetValue = false;
					//	pComponent = pGetcomponentAttribute.GetComponent( this, pTypeField.GetGenericArguments()[0] );

					//	pField.SetValue( this, System.Activator.CreateInstance( pTypeField, pComponent ) );
					//}
				}
				else
				{
					if (pGetcomponentAttribute is GetComponentAttribute)
						pComponent = this.GetComponent( pTypeField );
					else if (pGetcomponentAttribute is GetComponentInChildrenAttribute)
					{
						GetComponentInChildrenAttribute pAttribute = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
						if (pAttribute.bSearch_By_ComponentName)
							pComponent = this.GetComponentInChildren( pAttribute.strComponentName, pTypeField );
						else
							pComponent = this.GetComponentInChildren( pTypeField, true );
					}
					else if (pGetcomponentAttribute is GetComponentInParentAttribute)
						pComponent = this.GetComponentInParent( pTypeField );
				}

				if (pComponent == null && pGetcomponentAttribute.bIsPrint_OnNotFound)
				{
					GetComponentInChildrenAttribute pAttribute = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
					if(pAttribute != null && pAttribute.bSearch_By_ComponentName)
						Debug.LogWarning( name + string.Format( ".{0}<{1}>({2}) Result == null", pGetcomponentAttribute.GetType().Name, pTypeField, pAttribute.strComponentName ), this );
					else
						Debug.LogWarning( name + string.Format( ".{0}<{1}> Result == null", pGetcomponentAttribute.GetType().Name, pTypeField ), this );
					continue;
				}

				if (bIsRequireSetValue)
					pField.SetValue( this, pComponent );
			}
		}
	}
}
