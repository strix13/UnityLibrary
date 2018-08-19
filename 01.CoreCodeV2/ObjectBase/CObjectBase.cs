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

using NUnit.Framework;
using UnityEngine.TestTools;

public class CObjectBase : MonoBehaviour, IUpdateAble
{
	protected bool _bIsExcuteAwake = false;
    private Coroutine _pCoroutineOnEnable;

    public Vector3 p_vecPosition
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

    public void EventOnAwake_Force()
    {
        _bIsExcuteAwake = false;
        OnAwake();
    }

    public void EventExcuteDelay(System.Action OnAfterDelayAction, float fDelaySec)
    {
        if (fDelaySec == 0f)
            OnAfterDelayAction();
        else
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
    }

    public void EventStop_ExcuteDelayAll()
    {
        foreach (var pCoroutine in _mapCoroutinePlaying.Values)
            StopCoroutine(pCoroutine);

        _mapCoroutinePlaying.Clear();
    }

    // ========================== [ Division ] ========================== //

    void Awake()
    {
        if(_bIsExcuteAwake == false)
            OnAwake();
    }

    void OnEnable()
    {
        Invoke("RegistUpdateObject", 1f);

        OnEnableObject();
        if (_pCoroutineOnEnable != null)
            StopCoroutine(_pCoroutineOnEnable);

        if (gameObject.activeSelf)
            _pCoroutineOnEnable = StartCoroutine(OnEnableObjectCoroutine());
    }

    private void RegistUpdateObject()
    {
        CManagerUpdateObject.instance.DoAddObject(this);
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
            _bIsExcuteAwake = true;
            SCManagerGetComponent.DoUpdateGetComponentAttribute(this);

            if(gameObject.activeSelf)
            {
                StopCoroutine("OnAwakeCoroutine");
                StartCoroutine("OnAwakeCoroutine");
            }
        }

    }

    virtual protected IEnumerator OnAwakeCoroutine() { yield break; }
    virtual protected void OnStart() { }
    virtual protected void OnEnableObject() {}

    virtual protected IEnumerator OnAwakeCoroutine() { yield break; }
    virtual protected IEnumerator OnEnableObjectCoroutine() { yield break; }
    virtual protected void OnDisableObject() { }

    public void OnUpdate() { bool bCheckUpdate = true; OnUpdate(ref bCheckUpdate); }
    /// <summary>
    /// Unity Update와 동일한 로직입니다.
    /// </summary>
    virtual public void OnUpdate(ref bool bCheckUpdateCount) { }

    // ========================== [ Division ] ========================== //

    Dictionary<System.Action, Coroutine> _mapCoroutinePlaying = new Dictionary<System.Action, Coroutine>();

	protected IEnumerator CoDelayAction( System.Action OnAfterDelayAction, float fDelaySec )
	{
		yield return SCManagerYield.GetWaitForSecond( fDelaySec );

		OnAfterDelayAction();
		_mapCoroutinePlaying.Remove( OnAfterDelayAction );
	}    
}

#region Test
[Category("StrixLibrary")]
public class CObjectBase_테스트 : CObjectBase
{
    [GetComponent]
    [HideInInspector]
    public CObjectBase pGetComponent;

    [GetComponentInParent]
    [HideInInspector]
    public CObjectBase pGetComponentParents;

    [UnityTest]
    public IEnumerator Test_ObjectBase_GetComponent_Attribute()
    {
        GameObject pObjectNew = new GameObject();
        CObjectBase_테스트 pTarget = pObjectNew.AddComponent<CObjectBase_테스트>();
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponent);
    }

    [UnityTest]
    public IEnumerator Test_ObjectBase_GetComponentInChildren_Attribute()
    {
        GameObject pObjectNew = new GameObject();
        CObjectBase_테스트 pTargetParents = pObjectNew.AddComponent<CObjectBase_테스트>();

        CObjectBase_테스트 pTarget = pObjectNew.AddComponent<CObjectBase_테스트>();
        pTarget.transform.SetParent(pTargetParents.transform);
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponentParents);
    }
}
#endregion