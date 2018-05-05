#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	작성자 : Strix
 *	
 *	기능 : 
 *	GetComponentAttribute를 지원합니다. - GetComponentAttribute.cs 필요
 *	Update 퍼포먼스가 향상됩니다.        - CManagerUpdateObject.cs 필요
 *	Awake, Enable 코루틴을 지원합니다.
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CObjectBase : MonoBehaviour, IUpdateAble
{
	protected bool _bIsExcuteAwake = false;
    private Coroutine _pCoroutineOnAwake;
    private Coroutine _pCoroutineOnEnable;

	public Vector3 p_vecPos
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

    // ========================== [ Division ] ========================== //

    public void EventOnAwake()
    {
        if (_bIsExcuteAwake == false)
            OnAwake();
    }

    // ========================== [ Division ] ========================== //

    void Awake()
    {
        if(_bIsExcuteAwake == false)
            OnAwake();
    }

    void OnEnable()
    {
        CManagerUpdateObject.instance.DoAddObject(this);

        OnEnableObject();
        if (_pCoroutineOnEnable != null)
            StopCoroutine(_pCoroutineOnEnable);
        _pCoroutineOnEnable = StartCoroutine(OnEnableObjectCoroutine());
    }

    void OnDisable()
    {
        CManagerUpdateObject.instance.DoRemoveObject(this);
        OnDisableObject();
    }

    void Start() { OnStart(); }

    // ========================== [ Division ] ========================== //

    virtual protected void OnAwake()
    {
		if (_bIsExcuteAwake == false)
        {
            SCManagerGetComponent.DoUpdateGetComponentAttribute(this);
			_bIsExcuteAwake = true;

            if(isActiveAndEnabled)
            {
                if (_pCoroutineOnAwake != null)
                    StopCoroutine(OnAwakeCoroutine());
                _pCoroutineOnAwake = StartCoroutine(OnAwakeCoroutine());
            }
        }
	}

    virtual protected IEnumerator OnAwakeCoroutine() { yield break; }
    virtual protected void OnStart() { }
    virtual protected void OnEnableObject() {}
    virtual protected IEnumerator OnEnableObjectCoroutine() { yield break; }
    virtual public void OnUpdate() { }
    virtual protected void OnDisableObject() { }
	
	// ========================== [ Division ] ========================== //

	Dictionary<System.Action, Coroutine> _mapCoroutinePlaying = new Dictionary<System.Action, Coroutine>();
	protected void EventDelayExcuteCallBack(System.Action OnAfterDelayAction, float fDelaySec)
	{
        if (this != null && gameObject.activeInHierarchy)
        {
            if (_mapCoroutinePlaying.ContainsKey(OnAfterDelayAction))
                StopCoroutine(_mapCoroutinePlaying[OnAfterDelayAction]);

            Coroutine pCoroutine = StartCoroutine(CoDelayAction(OnAfterDelayAction, fDelaySec));
            if (_mapCoroutinePlaying.ContainsKey(OnAfterDelayAction) == false)
                _mapCoroutinePlaying.Add(OnAfterDelayAction, pCoroutine);
        }
    }

	protected IEnumerator CoDelayAction( System.Action OnAfterDelayAction, float fDelaySec )
	{
		yield return SCManagerYield.GetWaitForSecond( fDelaySec );

		OnAfterDelayAction();
		_mapCoroutinePlaying.Remove( OnAfterDelayAction );
	}
}

#region Test
#if UNITY_EDITOR

public class Test_CObjectBase : CObjectBase
{
    [GetComponent]
    [HideInInspector]
    public CObjectBase pGetComponent;

    [GetComponentInParent]
    [HideInInspector]
    public CObjectBase pGetComponentParents;

    [UnityTest]
    [Category("StrixLibrary")]
    public IEnumerator Test_ObjectBase_GetComponent_Attribute()
    {
        GameObject pObjectNew = new GameObject();
        Test_CObjectBase pTarget = pObjectNew.AddComponent<Test_CObjectBase>();
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponent);
    }

    [UnityTest]
    [Category("StrixLibrary")]
    public IEnumerator Test_ObjectBase_GetComponentInChildren_Attribute()
    {
        GameObject pObjectNew = new GameObject();
        Test_CObjectBase pTargetParents = pObjectNew.AddComponent<Test_CObjectBase>();

        Test_CObjectBase pTarget = pObjectNew.AddComponent<Test_CObjectBase>();
        pTarget.transform.SetParent(pTargetParents.transform);
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponentParents);
    }
}

#endif
#endregion Test